using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Diagnostics.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WildShape_Sheets_API.Models;

namespace WildShape_Sheets_API.Services {
    public class UserService {

        private readonly DataBaseService _dataBaseService;
        private readonly IConfiguration _configuration;
        private readonly HashService _hashService;
        //private readonly int _keySize;
        //private readonly int _iterations;
        //HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        public UserService(IConfiguration configuration, DataBaseService dataBaseService, HashService hashService) {

            _dataBaseService = dataBaseService;
            _configuration = configuration;
            _hashService = hashService;

            //_keySize = int.Parse(_configuration.GetSection("Hashbrowns:KeySize").Value!);
            //_iterations = int.Parse(_configuration.GetSection("Hashbrowns:Iterations").Value!);

        }

        

        public List<User> GetUsers() => _dataBaseService.userCollection.Find(user => true).ToList();

        public User GetUserById(string id) => _dataBaseService.userCollection.Find(user => user.Id == id).FirstOrDefault();

        public User GetUserByEmail(string email) => _dataBaseService.userCollection.Find(user => user.Email == email).FirstOrDefault();

        public User? CreateUser(User user) {

            var emailExist = VerifyEmailExists(user.Email);
            if (emailExist) {
                Console.WriteLine("User exists with used email");
                return null;
            }
            SetPassword(user);
            _dataBaseService.userCollection.InsertOne(user);
            return user;
        }

        public ReplaceOneResult UpdateUser(string id, User updatedUser) {

            var filter = Builders<User>.Filter.Eq(user => user.Id, id);
            
            return _dataBaseService.userCollection.ReplaceOne(filter, updatedUser);
        }

        public void DeleteUser(string id) => _dataBaseService.userCollection.DeleteOne(user => user.Id == id);

        public void SetPassword(User user) {
            user.Password = _hashService.HashPassword(user.Password, out var salt);
            user.Salt = salt;
        }

        

        public bool VerifyEmailExists(string email) {
            return GetUserByEmail(email) != null;
        }
    }
}
