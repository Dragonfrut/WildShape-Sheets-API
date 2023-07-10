using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WildShape_Sheets_API.Models;

namespace WildShape_Sheets_API.Services {
    public class UserService {

        private readonly IMongoCollection<User> users;
        //private readonly string key;

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
            users.InsertOne(user);
            return user;
        }

        public void DeleteUser(string id) => users.DeleteOne(user => user.Id == id);

    }
}
