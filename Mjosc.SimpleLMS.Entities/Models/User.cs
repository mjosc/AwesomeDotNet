using System;
using System.Collections.Generic;

namespace Mjosc.SimpleLMS.Entities.Models
{
    public partial class User
    {
        public User()
        {
            Course = new HashSet<Course>();
            Enrollment = new HashSet<Enrollment>();
        }

        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Role { get; set; }

        public virtual ICollection<Course> Course { get; set; }
        public virtual ICollection<Enrollment> Enrollment { get; set; }
    }
}
