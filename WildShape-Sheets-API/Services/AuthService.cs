using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WildShape_Sheets_API.Models;

namespace WildShape_Sheets_API.Services
{
    public class AuthService
    {
        private readonly IMongoCollection<User> users;

        public AuthService(IOptions<WildshapeSheetsDBSettings> wildshapeSheetsDBSettings, IConfiguration configuration)
        {
            var mongoClient = new MongoClient(
                wildshapeSheetsDBSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                wildshapeSheetsDBSettings.Value.DatabaseName);

            users = mongoDatabase.GetCollection<User>(
                wildshapeSheetsDBSettings.Value.UsersCollectionName);
        }

        public  string? Authenticate(string email, string password)
        {

            var user = users.Find(user => user.Email == email && user.Password == password).FirstOrDefault();

            if (user == null)
            {
                return null;
            }

            const string secretKey = "Shared secret key that no one ever knew";
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var claims = new[]
            {
                new Claim(ClaimTypes.Email,user.Email)
            };

            var jwt = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(4),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);

        }
    }
}
