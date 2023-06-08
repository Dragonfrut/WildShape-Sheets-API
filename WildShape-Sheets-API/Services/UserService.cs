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
        private readonly string key;

        public UserService(IOptions<WildshapeSheetsDBSettings> wildshapeSheetsDBSettings, IConfiguration configuration) {
            var mongoClient = new MongoClient(
                wildshapeSheetsDBSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                wildshapeSheetsDBSettings.Value.DatabaseName);

            users = mongoDatabase.GetCollection<User>(
                wildshapeSheetsDBSettings.Value.UsersCollectionName);

            this.key = configuration.GetSection("JwtKey").ToString();

        }

        public List<User> GetUsers() => users.Find(user => true).ToList();

        public User GetUser(string id) => users.Find(user => user.Id == id).FirstOrDefault();

        public User Create(User user) {
            users.InsertOne(user);
            return user;
        }


        public string Authenticate(string email, string password) {

            var user = users.Find(user => user.Email == email && user.Password == password).FirstOrDefault();

            if (user != null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenKey = Encoding.ASCII.GetBytes(key);

            var tokenDescriptor = new SecurityTokenDescriptor() {
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.Email, email),
                }),

                Expires = DateTime.UtcNow.AddHours(1),

                SigningCredentials = new SigningCredentials (
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                    )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        //public async Task<List<User>> GetAsync() =>
        //    await _userCollection.Find(_ => true).ToListAsync();

        //public async Task<User> GetAsync(string id) =>
        //    await _userCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        //public async Task CreateAsync(User newUser) =>
        //await _userCollection.InsertOneAsync(newUser);

        //public async Task UpdateAsync(string id, User updatedUser) =>
        //    await _userCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

        //public async Task RemoveAsync(string id) =>
        //    await _userCollection.DeleteOneAsync(x => x.Id == id);
    }
}
