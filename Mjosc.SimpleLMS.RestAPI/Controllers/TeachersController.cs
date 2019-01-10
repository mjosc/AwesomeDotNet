using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mjosc.SimpleLMS.Entities.Models;

namespace Mjosc.SimpleLMS.RestAPI.Controllers
{
    // -------------------------------------------------------------------
    // A simple API controller responsible for accessing data pertaining
    // to Teacher entities.
    //
    // Unlike UsersController, this controller makes use of asyncronous
    // methods.
    //
    // Role-based authentication is not configured. Though this data is
    // private from non-authenticated users, both students and teachers
    // are provided access once registered.
    //
    // The use of anonymous types is obvious. This could be minimized
    // outside the context of the LINQ queries with additional DTOs 
    // defined in the referenced Entities project. However, for a
    // controller not dependent on any underlying service (such as that
    // implemented in Services/UserService and
    // Controllers/UsersController) a return type of object is sufficient
    // for readability.
    // -------------------------------------------------------------------

    [Authorize]
    [Route("api/[controller]")]
    public class TeachersController : ControllerBase
    {
        private readonly LmsDbContext db;

        public TeachersController(LmsDbContext context)
        {
            db = context;
        }

        // -----------------------------------------------------------
        // Note that in the following queries, the returned object
        // often contains a list of the courses taught by each
        // teacher.
        // -----------------------------------------------------------

        // GET: api/teachers
        [HttpGet]
        public async Task<ActionResult<object>> GetTeachers()
        {
            return await db.User
                .Where(u => u.Role == "Teacher")
                .Select(u => new
                {
                    u.UserId,
                    u.FirstName,
                    u.LastName,
                    DOB = u.DateOfBirth,
                    Courses = db.User

                        .Join(db.Course,
                            user => user.UserId,
                            course => course.TeacherId,
                            (user, course) => new { user, course })
                        .Where(uc => uc.user.UserId == u.UserId)
                        .Select(uc => uc.course.CourseName)
                })
                .ToListAsync();
        }

        // GET: api/teachers/2
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetTeacher(long id)
        {
            var result = await db.User
                .Where(u => u.UserId == id && u.Role == "Teacher")
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    DOB = u.DateOfBirth,
                    Courses = db.User

                        .Join(db.Course,
                            user => user.UserId,
                            course => course.TeacherId,
                            (user, course) => new { user, course })
                        .Where(uc => uc.user.UserId == u.UserId)
                        .Select(uc => uc.course.CourseName)
                })
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        // GET: api/teachers/age/4
        [HttpGet("age/{age}")]
        public async Task<ActionResult<object>> GetTeachersOlderThan(int age)
        {
            // This is not a perfect calculation. There are edge cases to
            // be considered if this were a production application.
            DateTime target = DateTime.UtcNow.AddYears(-age);

            return await db.User
                .Where(u => u.Role == "Teacher" && u.DateOfBirth < target)
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    DOB = u.DateOfBirth,
                    Courses = db.User

                        .Join(db.Course,
                            user => user.UserId,
                            course => course.TeacherId,
                            (user, course) => new { user, course })
                        .Where(uc => uc.user.UserId == u.UserId)
                        .Select(uc => uc.course.CourseName)
                })
                .ToListAsync();
        }

        // DELETE: api/teachers/4
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> Delete(long id)
        {

            // TODO: See the comments in CoursesController
            // regarding transactions.

            var teacher = await db.User
                .Where(u => u.UserId == id && u.Role == "Teacher")
                .FirstOrDefaultAsync();

            if (teacher == null)
            {
                return NotFound();
            }
          
            db.User.Remove(teacher);
            await db.SaveChangesAsync();

            return new
            {
                teacher.UserId,
                Name = $"{teacher.FirstName} {teacher.LastName}",
                teacher.DateOfBirth
            };
        }
    }
}
