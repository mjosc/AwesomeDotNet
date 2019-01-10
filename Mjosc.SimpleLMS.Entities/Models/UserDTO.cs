using System;

namespace Mjosc.SimpleLMS.Entities.Models
{
    public class UserDTO
    {
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Role { get; set; }
    }
}
