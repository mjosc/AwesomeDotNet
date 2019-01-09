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
    // Defines methods for the manipulation of User objects. Note the use of Object as the
    // return type in order to use anonymous types. Ideally, these would be replaced by custom
    // DTO classes within Mjosc.SimpleLMS.Entities.
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<object> GetAll();
        object GetUser(long id);
        User Create(User user, string password);
    }

    // A service providing authenticated database access to the User entity. Again, the use of
    // anonymous types would ideally be replaced by a series of custom DTO classes.
    public class UserService : IUserService
    {
        private readonly LmsDbContext db;

        public UserService(LmsDbContext context)
        {
            db = context;
        }

        // Authentication method implementing JWT verification via the SecurityUtil class. Password
        // salts and hashes are currently stored on the same database as the remainder of the data.
        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) 
            {
                return null;
            }

            User user = db.User.SingleOrDefault(u => u.Username == username);
            if (user == null || !SecurityUtil.Verify(password, user.PasswordSalt, user.PasswordHash))
            {
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

            // Pass by reference.
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

        public IEnumerable<object> GetAll()
        {
            return db.User.Select(u => new
            {
                u.UserId,
                u.FirstName,
                u.LastName,
                u.Role
            });
        }

        public object GetUser(long id)
        {
            // TODO: db.User.Find() will be sufficient once additional DTOs are defined and the
            // AutoMapper functionality is implemented.
            return db.User
                .Where(u => u.UserId == id)
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.Role
                });
        }
    }
}
