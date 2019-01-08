using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mjosc.SimpleLMS.Entities.Models;

namespace Mjosc.SimpleLMS.RestAPI.Controllers
{
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
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            return await db.Course.ToListAsync();
        }

        // GET: api/courses/3
        [HttpGet("{id}")]
        public async Task<ActionResult<Object>> GetCourse(int id)
        {
            var result = await db.Course.FindAsync(id);

            //var result = await db.Course
                //.Where(c => c.CourseId == id)
                //.Select(c => new
                //{
                //    id = c.CourseId,
                //    name = c.CourseName,
                //    credit = c.CreditHours,
                //    teacher = $"{c.Teacher.FirstName} {c.Teacher.LastName}"
                //})
                //.FirstAsync();

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        // GET: api/courses/chemistry
        //[HttpGet("{courseName")]
        //public async Task<ActionResult<Course>> GetCourse(string courseName)
        //{
        //    //var course = await db.Course
        //    //.Where(course => )
        //    return null;
        //}
    }
}
