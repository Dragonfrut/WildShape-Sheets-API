using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WildShape_Sheets_API.Models;

namespace WildShape_Sheets_API.Services {
    public class UserService {

        private readonly IMongoCollection<User> _users;
        private readonly IConfiguration _configuration;
        private readonly int _keySize;
        private readonly int _iterations;
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        public UserService(IOptions<WildshapeSheetsDBSettings> wildshapeSheetsDBSettings, IConfiguration configuration) {
            var mongoClient = new MongoClient(
                wildshapeSheetsDBSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                wildshapeSheetsDBSettings.Value.DatabaseName);

            _users = mongoDatabase.GetCollection<User>(
                wildshapeSheetsDBSettings.Value.UsersCollectionName);

            _configuration = configuration;

            _keySize = int.Parse(_configuration.GetSection("Hashbrowns:KeySize").Value!);
            _iterations = int.Parse(_configuration.GetSection("Hashbrowns:Iterations").Value!);

        }

        public List<User> GetUsers() => _users.Find(user => true).ToList();

        public User GetUser(string id) => _users.Find(user => user.Id == id).FirstOrDefault();

        public User CreateUser(User user) {
            user.Password = HashPassword(user.Password!, out var salt);
            user.Salt = salt;
            _users.InsertOne(user);
            return user;
        }

        public void DeleteUser(string id) => _users.DeleteOne(user => user.Id == id);

        string HashPassword(string password, out byte[] salt) {
            salt = RandomNumberGenerator.GetBytes(_keySize);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                _iterations,
                hashAlgorithm,
                _keySize);

            return Convert.ToHexString(hash);
        }

        public bool VerifyPassword(string password, string hash, byte[] salt) {

            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, _iterations, hashAlgorithm, _keySize);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }
    }
}
