using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mjosc.SimpleLMS.Entities.Models;

namespace Mjosc.SimpleLMS.RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly LmsDbContext db;

        public StudentsController(LmsDbContext context)
        {
            db = context;
        }

        // GET: api/students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetStudents()
        {
            return await db.User.ToListAsync();
        }

        //// GET: api/students/1
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Student>> GetStudent(int id)
        //{
        //    var student = await db.Student.FindAsync(id);
        //    if (student == null)
        //    {
        //        return NotFound();
        //    }
        //    return student;
        //}
    }
}
