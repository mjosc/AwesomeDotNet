﻿using System;
using System.Collections.Generic;

namespace Mjosc.SimpleLMS.Entities.Models
{
    public partial class Enrollment
    {
        public long StudentId { get; set; }
        public long CourseId { get; set; }
        public string Grade { get; set; }

        public virtual Course Course { get; set; }
        public virtual User Student { get; set; }
    }
}
