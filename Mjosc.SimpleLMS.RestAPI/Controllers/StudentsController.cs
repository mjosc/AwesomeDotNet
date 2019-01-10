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
    // to Student entities.
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
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly LmsDbContext db;

        public StudentsController(LmsDbContext context)
        {
            db = context;
        }

        // GET: api/students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetStudents()
        {
            // -----------------------------------------------------------
            // LINQ method syntax equivalent the following SQl query:
            // 
            // TODO: Attempt the corresponding LINQ method syntax with
            // pure SQL syntax outside the context of this source code.
            // 
            // The nested query does not need to check whether the role
            // is student since Enrollment does not contain Teacher ids.
            //
            // Note that the nested query is returning a list of courses
            // to which each student is enrolled.
            // -----------------------------------------------------------

            return await db.User
                .Where(u => u.Role == "Student")
                .Select(u => new
                {
                    u.UserId,
                    u.FirstName,
                    u.LastName,
                    DOB = u.DateOfBirth,
                    Courses = db.User

                        .Join(db.Enrollment,
                            user => user.UserId,
                            enrollment => enrollment.StudentId,
                            (user, enrollment) => new { user, enrollment })

                        .Join(db.Course,
                            ue => ue.enrollment.CourseId,
                            course => course.CourseId,
                            (ue, course) => new { ue.user, ue.enrollment, course })

                        .Where(uec => uec.user.UserId == u.UserId)
                        .Select(uec => uec.course.CourseName)
                })
                .ToListAsync();
        }

        // GET: api/students/3
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetStudent(long id)
        {
            // -----------------------------------------------------------
            // LINQ method syntax equivalent the following SQl query:
            // 
            // Select User.FirstName, User.LastName, User.DateOfBirth
            //      where User.UserId = id;
            // -----------------------------------------------------------
            return await db.User
                .Where(u => u.UserId == id && u.Role == "Student")
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    DOB = u.DateOfBirth
                })
                .FirstOrDefaultAsync();
        }

        // GET: api/students/courses/chemistry
        [HttpGet("courses/{courseName}")]
        public async Task<ActionResult<object>> GetStudentsInCourse(string courseName)
        {
            // -----------------------------------------------------------
            // LINQ method syntax equivalent the following SQl query:
            // 
            // Select User.UserId, User.FirstName, User.LastName
            //      from User join Enrollment
            //      on User.UserId = Enrollment.StudentId
            //      join Course on Enrollment.CourseId = Course.CourseId
            //      where Course.CourseName = courseName;
            //
            // We don't need to check whether the role is 'Student' due to
            // the fact that Enrollment will not contain Teacher ids.
            // -----------------------------------------------------------

            return await db.User
                .Join(db.Enrollment,
                    user => user.UserId,
                    enrollment => enrollment.StudentId,
                    (user, enrollment) => new { user, enrollment })

                .Join(db.Course, 
                    ue => ue.enrollment.CourseId,
                    course => course.CourseId,
                    (ue, course) => new { ue.user, ue.enrollment, course })

                .Where(uec => uec.course.CourseName == courseName)
                .Select(uec => new
                {
                    uec.user.UserId,
                    uec.user.FirstName,
                    uec.user.LastName,
                })
                .ToListAsync();
        }

        // GET: api/students/grades/a
        [HttpGet("grades/{grade}")]
        public async Task<ActionResult<object>> GetStudentsWithGrade(string grade)
        {
            // -----------------------------------------------------------
            // LINQ method syntax equivalent the following SQl query:
            // 
            // Select User.UserId, User.FirstName, User.LastName
            //      from User join Enrollment
            //      on User.UserId = Enrollment.StudentId
            //      where Enrollment.Grade = grade;
            //
            // We don't need to check whether the role is 'Student' due to
            // the fact that Enrollment will not contain Teacher ids.
            // -----------------------------------------------------------

            return await db.User
                .Join(db.Enrollment,
                    user => user.UserId,
                    enrollment => enrollment.StudentId,
                    (user, enrollment) => new { user, enrollment })
                .Where(ue => ue.enrollment.Grade == grade)
                .Select(ue => new
                {
                    ue.user.UserId,
                    ue.user.FirstName,
                    ue.user.LastName
                })
                .ToListAsync();
        }
    }
}
