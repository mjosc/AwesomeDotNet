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
    // A helper class for hashing and comparing hashed passwords. Many of the 
    // implementation details are borrowed from https://goo.gl/4td77q.
    public static class SecurityService
    {
        public static void Hash(string password, out byte[] salt, out byte[] hash)
        {
            // The following two conditions should be checked server-side but outside
            // the context of this method. These methods should be primarily for 
            // development purposes.

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
                salt = hmac.Key;
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

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

        public static string CreateJsonWebToken(string secretKey, User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
