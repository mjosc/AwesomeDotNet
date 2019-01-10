using System;
using System.Collections.Generic;
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
    [Route("api/[Controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly LmsDbContext db;

        public CoursesController(LmsDbContext context)
        {
            db = context;
        }

        // GET: api/courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCourses()
        {
            // -----------------------------------------------------------
            // The nested query does not need to check whether the role is
            // teacher since Course does not contain student ids.
            // -----------------------------------------------------------

            return await db.Course
                .Select(c => new
                {
                    c.CourseId,
                    c.CourseName,
                    c.CreditHours,
                    Teacher = db.User

                        .Where(u => u.UserId == c.TeacherId)
                        .Select(u => new
                        {
                            u.FirstName,
                            u.LastName
                        })
                })
                .ToListAsync();
        }

        // GET: api/courses/3
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetCourse(long id)
        {
            // -----------------------------------------------------------
            // Note the Teacher field is taking advantage of the implicit
            // joins within the entity framework.
            // -----------------------------------------------------------

            var result = await db.Course
                .Where(c => c.CourseId == id)
                .Select(c => new
                {
                    c.CourseId,
                    c.CourseName,
                    c.CreditHours,
                    Teacher = $"{c.Teacher.FirstName} {c.Teacher.LastName}"
                })
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        // GET: api/courses/name/chemistry
        [HttpGet("name/{courseName}")]
        public async Task<ActionResult<object>> GetCourse(string courseName)
        {
            // -----------------------------------------------------------
            // Note the Teacher field is taking advantage of the implicit
            // joins within the entity framework.
            // -----------------------------------------------------------

            var result = await db.Course
                .Where(c => c.CourseName == courseName)
                .Select(c => new
                {
                    c.CourseId,
                    c.CourseName,
                    c.CreditHours,
                    Teacher = $"{c.Teacher.FirstName} {c.Teacher.LastName}"
                })
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        // -------------------------------------------------------------------
        // TODO: Is there a better way to handle the transaction contained
        // within the Create method? Is db.SaveChanges() actually sufficient?
        //
        // See https://docs.microsoft.com/en-us/ef/core/saving/transactions
        // --------------------------------------------------------------------

        // POST: api/courses
        [HttpPost("add")]
        public async Task<ActionResult> Create([FromBody] Course course)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (await db.Course.AnyAsync(c => c.CourseName == course.CourseName))
                    {
                        // Note: This could be enforced by the SQL database by catching
                        // exceptions thrown via the uniqueness constraint on the
                        // courseName column when attempting to insert a course by the
                        // same name.
                        return BadRequest("Course names must be unique.");
                    }
                    else
                    {
                        await db.AddAsync(course);
                        await db.SaveChangesAsync();
                    }

                    transaction.Commit();
                    return Ok(new
                    {
                        course.CourseId,
                        course.CourseName,
                        course.CreditHours,
                        course.TeacherId
                    });
                }
                catch (Exception)
                {
                    // TODO: Provide a meaningful message if possible.
                    return BadRequest();
                }
            }
        }

        // -------------------------------------------------------------------
        // TODO: See previous note about transactions. According the docs, the
        // default behavior of SaveChanges is sufficient for most use cases,
        // the concern is the use of the word "change". Because the following
        // delete method and the previous Create method require a read before
        // determining whether to delete, there may be a need for
        // Database.BeginTransaction().
        //
        // See https://docs.microsoft.com/en-us/ef/core/saving/transactions
        // --------------------------------------------------------------------

        // DELETE:: api/courses/1
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> Delete(long id)
        {
            var course = await db.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            // This operation will only be successful if no students are enrolled
            // in the specified course (enforced via foreign key).
            db.Course.Remove(course);
            await db.SaveChangesAsync();

            return new
            {
                course.CourseId,
                course.CourseName,
                course.CreditHours,
                course.TeacherId
            };
        }
    }
}
