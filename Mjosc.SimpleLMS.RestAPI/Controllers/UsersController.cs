using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Mjosc.SimpleLMS.Entities.Models;
using Mjosc.SimpleLMS.RestAPI.Helpers;
using Mjosc.SimpleLMS.RestAPI.Services;

namespace Mjosc.SimpleLMS.RestAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AuthenticationStrings _authStrings;

        public UsersController(IUserService userService, IOptions<AuthenticationStrings> authStrings)
        {
            _userService = userService;
            _authStrings = authStrings.Value;
        }

        // POST: users/authenticate
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public ActionResult Authenticate([FromBody] UserDTO userDTO)
        {
            Console.WriteLine("TEST");
            Console.WriteLine(userDTO.ToString());
            Console.WriteLine(userDTO.Password);
            Console.WriteLine(userDTO.Username);

            User user = _userService.Authenticate(userDTO.Username, userDTO.Password);


            if (user == null)
            {
                Console.WriteLine("No user in database.");
                //return Unauthorized();
            }
            user = new User
            {
                UserId = 1
            };


            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authStrings.JwtSecretKey);
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
            string tokenString = jwtTokenHandler.WriteToken(token);

            return Ok(new
            {
                id = "awesome",
                username = "awesome",
                token = tokenString
            });
        }
    }
}
