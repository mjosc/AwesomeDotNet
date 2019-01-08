using System;
using System.Collections.Generic;

namespace Mjosc.SimpleLMS.Entities.Models
{
    public partial class Course
    {
        public Course()
        {
            Enrollment = new HashSet<Enrollment>();
        }

        public int CourseId { get; set; }
        public short CreditHours { get; set; }
        public string CourseName { get; set; }
        public int TeacherId { get; set; }

        public virtual Teacher Teacher { get; set; }
        public virtual ICollection<Enrollment> Enrollment { get; set; }
    }
}
