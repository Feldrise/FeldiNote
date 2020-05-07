using FeldiNote.Api.Models;
using FeldiNote.Api.Settings;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeldiNote.Api.Services
{
    public class AuthenticationService
    {
        private readonly IMongoCollection<User> _users;

        public AuthenticationService(IMongoSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>("users");
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public User Register(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("The password is required");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new Exception("You must provide an email");
            }

            if (string.IsNullOrWhiteSpace(user.Username))
            {
                throw new Exception("You must provide a username");
            }

            if (UserExist(user))
            {
                throw new Exception("The email or the username are already in use");
            }

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _users.InsertOne(user);
            
            return user;
        }

        public bool CheckCredential(string id, string password)
        {
            User databaseUser = _users.Find(searchedUser => searchedUser.Id == id).First();

            if (databaseUser == null)
                return false;

            return VerifyPasswordHash(password, databaseUser.PasswordHash, databaseUser.PasswordSalt);
        }

        private bool UserExist(User checkedUser)
        {
            var exist = _users.AsQueryable<User>().Any(user => user.Email == checkedUser.Email || user.Username == checkedUser.Username);

            return exist;
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
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
