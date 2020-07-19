using System;
using MongoDB.Driver;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using BiteByByteAPI.Entities;
using BiteByByteAPI.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BiteByByteAPI.Helpers;
using WebApi.Helpers;

namespace BiteByByteAPI.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(string id);
        User Create(User user, string password);
        void Update(User user, string password = null);
        void Delete(string id);
    }

    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IBiteByByteDatabaseSettings settings)
        {
            // connect to local mongodb session and database as listed in appsettings.json
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            // get mongodb collection name from appsettings.json
            _users = database.GetCollection<User>(settings.UsersCollectionName);
        }
        
        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _users.Find(x => x.Username == username).FirstOrDefault();

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public IEnumerable<User> GetAll() =>
            _users.Find(user => true).ToList();

        public User GetById(string id) =>
            _users.Find<User>(user => user.Id == id).FirstOrDefault();

        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            var duplicate = _users.Find(x => x.Username == user.Username).FirstOrDefault();
            if (duplicate != null)
            {
                throw new AppException("Username \"" + user.Username + "\" is already taken" + duplicate);
            }
            
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            
            _users.InsertOne(user);
            return user;
        }

        public void Update(User userIn, string password = null)
        {
            var user = _users.Find<User>(x => x.Id == userIn.Id).FirstOrDefault();
            if (user == null)
                throw new AppException("User not found");
            
            // update username if it has changed
            if (!string.IsNullOrWhiteSpace(userIn.Username) && userIn.Username != user.Username)
            {
                // throw error if the new username is already taken
                var duplicate = _users.Find(x => x.Username == user.Username).FirstOrDefault();
                if (duplicate == null)
                {
                    throw new AppException("Username \"" + user.Username + "\" is already taken");
                }

                user.Username = userIn.Username;
            }

            // update user properties if provided
            if (!string.IsNullOrWhiteSpace(userIn.FirstName))
                user.FirstName = userIn.FirstName;

            if (!string.IsNullOrWhiteSpace(userIn.LastName))
                user.LastName = userIn.LastName;

            // update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }
            
            _users.ReplaceOne(user => user.Id == userIn.Id, user);
        }

        public void Delete(User userIn) =>
            _users.DeleteOne(user => user.Id == userIn.Id);

        public void Delete(string id) => 
            _users.DeleteOne(user => user.Id == id);
        
        
        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}