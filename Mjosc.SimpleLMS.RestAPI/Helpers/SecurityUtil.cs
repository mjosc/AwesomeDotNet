using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Mjosc.SimpleLMS.Entities.Models;

namespace Mjosc.SimpleLMS.RestAPI.Services
{
    // -------------------------------------------------------------------
    // A utility class for all security-related methods. This class
    // contains methods for both password hashing and verification as well
    // as the underlying JWT implementation used by .NET Core's
    // authentication framework.
    //
    // The hashing and verification code is derived
    // from https://goo.gl/4td77q.
    // The JWT code is derived from https://goo.gl/T5Aiet.
    // -------------------------------------------------------------------

    public static class SecurityUtil
    {
        public static void Hash(string password, out byte[] salt, out byte[] hash)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("The password must consist of non-whitespace characters.");
            }

            using (var hmac = new HMACSHA512())
            {
                // Required in order to store the salt to the database.
                // The two byte arrays are passed by reference.
                salt = hmac.Key;
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        // Verifies whether the hash of the newly provided password matches that
        // of a previously computed hash (originating from the database).
        public static bool Verify(string password, byte[] salt, byte[] hash)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            // Don't worry about checking if null or whitespace here. If the passwords
            // don't match, they simply don't match. No need for extra checking.

            using (var hmac = new HMACSHA512(salt))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(hash);
            }
        }

        // Generates a unique web token based on the user's id and role.
        public static string CreateJsonWebToken(string secretKey, User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            // Header.Payload.Secret
            SecurityToken token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
