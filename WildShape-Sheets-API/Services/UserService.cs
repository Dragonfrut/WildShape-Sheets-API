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

        private readonly IMongoCollection<User> users;
        const int keySize = 64;
        const int iterations = 350000;
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        public UserService(IOptions<WildshapeSheetsDBSettings> wildshapeSheetsDBSettings, IConfiguration configuration) {
            var mongoClient = new MongoClient(
                wildshapeSheetsDBSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                wildshapeSheetsDBSettings.Value.DatabaseName);

            users = mongoDatabase.GetCollection<User>(
                wildshapeSheetsDBSettings.Value.UsersCollectionName);
        }

        public List<User> GetUsers() => users.Find(user => true).ToList();

        public User GetUser(string id) => users.Find(user => user.Id == id).FirstOrDefault();

        public User CreateUser(User user) {
            user.Password = HashPassword(user.Password!, out var salt);
            user.Salt = salt;
            users.InsertOne(user);
            return user;
        }

        public void DeleteUser(string id) => users.DeleteOne(user => user.Id == id);

        string HashPassword(string password, out byte[] salt) {
            salt = RandomNumberGenerator.GetBytes(keySize);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);

            return Convert.ToHexString(hash);
        }

        public bool VerifyPassword(string password, string hash, byte[] salt) {

            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }
    }
}
