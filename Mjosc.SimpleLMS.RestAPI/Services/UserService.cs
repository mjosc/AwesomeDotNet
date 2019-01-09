using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            User user = db.User.SingleOrDefault(u => u.Username == username);
            if (user == null || !SecurityUtil.Verify(password, user.PasswordSalt, user.PasswordHash))
            {
                if (user == null)
                {
                    Console.WriteLine("USER IS NULL");
                }
                else
                {
                    Console.WriteLine("FAILED PASSWORD VERIFICATION");
                }

                return null;
            }

            return user;
        }

        public User Create(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ApplicationException("Invalid password.");
            }

            // TODO: How are transactions handled within LINQ method syntax? The following
            // should technically be in a transaction due to the multi-part read/write.

            if (db.User.Any(u => u.Username == user.Username))
            {
                throw new ApplicationException($"Username: {user.Username} is already taken");
            }

            byte[] salt, hash;
            SecurityUtil.Hash(password, out salt, out hash);

            user.PasswordSalt = salt;
            user.PasswordHash = hash;

            // This should not fail if the username is available and the username check is
            // completed in the same transaction as the row insertion.
            db.User.Add(user);
            db.SaveChanges();

            return user;
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
