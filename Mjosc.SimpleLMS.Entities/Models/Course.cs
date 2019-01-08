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

        public long CourseId { get; set; }
        public short CreditHours { get; set; }
        public string CourseName { get; set; }
        public long TeacherId { get; set; }

        public virtual User Teacher { get; set; }
        public virtual ICollection<Enrollment> Enrollment { get; set; }
    }
}
