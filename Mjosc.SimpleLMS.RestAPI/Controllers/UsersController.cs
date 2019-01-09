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
        private readonly string jwtSecretKey;

        public UsersController(IUserService userService, IOptions<AuthenticationStrings> authStrings)
        {
            _userService = userService;
            jwtSecretKey = authStrings.Value.JwtSecretKey;
        }

        // POST: users/authenticate
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public ActionResult Authenticate([FromBody] UserDTO userDTO)
        {
            User user = _userService.Authenticate(userDTO.Username, userDTO.Password);

            if (user == null)
            {
                Console.WriteLine("No user in database.");
            }
            user = new User
            {
                UserId = 1
            };

            return Ok(new
            {
                id = "awesome",
                username = "awesome",
                token = SecurityUtil.CreateJsonWebToken(jwtSecretKey, user)
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public ActionResult Register([FromBody]UserDTO userDTO)
        {
            // TODO: Manual mapping between User models can be replaced with AutoMapper.
            // The current implementation, however, is useful due to the fact that fields
            // uninitialized from the request body are more apparent (e.g. PasswordHash
            // and PasswordSalt are assigned from within the UserService dependency.
            User user = new User
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Username = userDTO.Username,
                DateOfBirth = userDTO.DateOfBirth,
                Role = userDTO.Role
            };

            try
            {
                // user.UserId is assigned by the database. The id is used within the
                // CreateJsonWebToken utility method.
                user = _userService.Create(user, userDTO.Password);
                return Ok(new
                {
                    username = user.Username,
                    token = SecurityUtil.CreateJsonWebToken(jwtSecretKey, user)
                });
            }
            catch (ApplicationException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
