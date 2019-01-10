using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Mjosc.SimpleLMS.Entities.Models;
using Mjosc.SimpleLMS.RestAPI.Helpers;
using Mjosc.SimpleLMS.RestAPI.Services;

namespace Mjosc.SimpleLMS.RestAPI.Controllers
{
    // -------------------------------------------------------------------
    // A simple API authorization controller which also includes generic
    // routes for retrieving lists of all users and/or a user by a
    // specific id.
    //
    // See Controllers/StudentController and Controllers/TeacherController
    // for routes specific to those roles (note that role-based
    // authentication is not implemented; both teachers and students have
    // access to all controllers once authenticated here).
    //
    // Unlike StudentController and TeacherController, this controller does
    // not implement async/await. This is in part due to the desire to
    // practice with both and in part to having yet to integrate the async
    // methods within the underlying UserService class upon which this
    // controller depends for database IO.
    //
    // Note the use of anonymous types. This could be minimized with the
    // addition of more DTOs (as defined in the referenced Entities
    // project). The AutoMapper library has been added to this project
    // but the implementation has not yet been created.
    // --------------------------------------------------------------------

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly string jwtSecretKey;

        // TODO: To be implemented. See Helpers/AutoMapperUser.cs.
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IOptions<AuthenticationStrings> authStrings,
            IMapper mapper)
        {
            _userService = userService;
            jwtSecretKey = authStrings.Value.JwtSecretKey;
            _mapper = mapper;
        }


        // --------------------------------------------------------------------
        // Anonymous authentication routes
        //
        // Implements JWT. See Helpers/SecurityUtil.cs for JWT implementation.
        //
        // TODO: It would be useful to create a wrapper class aroundn HTTP
        // responses in order to reduce a bit of the duplicate code found in
        // the following two methods.
        // --------------------------------------------------------------------


        // POST: users/authenticate
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public ActionResult Authenticate([FromBody] UserDTO userDTO)
        {
            // The User class does not contain a password field. The id is 
            // provided by the database (auto-incremented).
            User user = _userService.Authenticate(userDTO.Username, userDTO.Password);

            if (user == null)
            {
                return BadRequest("Invalid username or password");
            }

            return Ok(new
            {
                user.Username,
                Token = SecurityUtil.CreateJsonWebToken(jwtSecretKey, user)
            });
        }

        // POST: users/register
        [AllowAnonymous]
        [HttpPost("register")]
        public ActionResult Register([FromBody] UserDTO userDTO)
        {
            // TODO: AutoMapper will be helpful here.
            var user = new User
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Username = userDTO.Username,
                DateOfBirth = userDTO.DateOfBirth,
                Role = userDTO.Role
            };

            try
            {
                // The id provided by the database is required for JWT generation.
                user = _userService.Create(user, userDTO.Password);
                return Ok(new
                {
                    user.Username,
                    Token = SecurityUtil.CreateJsonWebToken(jwtSecretKey, user)
                });
            }
            catch (ApplicationException e)
            {
                // TODO: ServerResponse wrapper with custom error messages
                // will be helpful here.
                return BadRequest(new { message = e.Message });
            }
        }

        // --------------------------------------------------------------------
        // Private routes
        //
        // More complex use cases are found in the other controllers.
        // --------------------------------------------------------------------

        // GET: /users
        [HttpGet]
        public ActionResult<IEnumerable<object>> GetAll()
        {
            return Ok(_userService.GetAll());
        }

        // GET: /users/3
        [HttpGet("{id}")]
        public ActionResult<object> GetUser(long id)
        {
            return _userService.GetUser(id);
        }
    }
}
