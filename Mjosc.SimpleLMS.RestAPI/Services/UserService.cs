using System;
using System.Collections.Generic;
using System.Linq;
using Mjosc.SimpleLMS.Entities.Models;

namespace Mjosc.SimpleLMS.RestAPI.Services
{
    // -------------------------------------------------------------------
    // An interface and service class describing a few simple methods for 
    // the manipulation of user objects. This class is a dependency of
    // UsersController.cs and handles all of its underlying database
    // access.
    //
    // Currently, password hashes and their corresponding salts are stored
    // on the same database as all other data.
    //
    // Note the use of anonymous types. This could be replaced with DTOs
    // but is not currently implemented.
    //
    // See StudentController, TeacherController, etc. for more complex
    // database queries.
    // --------------------------------------------------------------------

    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<object> GetAll();
        object GetUser(long id);
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
                return null;
            }

            return user;
        }

        // -------------------------------------------------------------------
        // TODO: How are transactions handled within LINQ (more specifically,
        // using the method syntax)?
        //
        // The Create method should theoretically contain a transaction given
        // that after the database is queried for the existence of a particular
        // username, another entity might create that username before this one.
        // --------------------------------------------------------------------

        public User Create(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ApplicationException("Invalid password.");
            }

            if (db.User.Any(u => u.Username == user.Username))
            {
                throw new ApplicationException($"Username: {user.Username} is already taken");
            }

            // Pass by reference.
            byte[] salt, hash;
            SecurityUtil.Hash(password, out salt, out hash);

            user.PasswordSalt = salt;
            user.PasswordHash = hash;

            db.User.Add(user);
            db.SaveChanges();

            return user;
        }

        public IEnumerable<object> GetAll()
        {
            // -----------------------------------------------------------
            // LINQ method syntax equivalent the following SQl query:
            // 
            // Select User.Id, User.FirstName, User.LastName, User.Role
            // from User;
            //
            // db.User.ToList() would work here with the appropriate
            // mapper and DTO in order to avoid responding with the
            // complete User object (with fields such as passwordSalt and
            // passwordHash).
            // -----------------------------------------------------------

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
            // -----------------------------------------------------------
            // LINQ method syntax equivalent the following SQl query:
            // 
            // Select User.FirstName, User.LastName, User.Role
            //      where User.UserId = id;
            //
            // db.User.Find() would work here with the appropriate
            // mapper and DTO in order to avoid responding with the
            // complete User object (with fields such as passwordSalt and
            // passwordHash).
            // -----------------------------------------------------------

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
