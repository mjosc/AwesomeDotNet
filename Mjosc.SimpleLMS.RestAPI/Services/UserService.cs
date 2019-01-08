using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mjosc.SimpleLMS.Entities.Models;

namespace Mjosc.SimpleLMS.RestAPI.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetUser(long id);
        User Create(User user, string password);
    }

    public class UserService : IUserService
    {
        private readonly LmsDbContext db;

        public UserService(LmsDbContext context)
        {
            db = context;
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) 
            {
                return null;
            }

            User user = db.User.SingleOrDefault();
            if (user == null || !CryptoService.Verify(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        public User Create(User user, string password)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            return null;
        }

        public User GetUser(long id)
        {
            throw new NotImplementedException();
        }
    }
}
