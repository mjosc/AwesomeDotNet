using System;
using System.Collections.Generic;

namespace Mjosc.SimpleLMS.Entities.Models
{
    public partial class Enrollment
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public string Grade { get; set; }

        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }
}
