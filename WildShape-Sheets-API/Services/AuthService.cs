using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WildShape_Sheets_API.Models;

namespace WildShape_Sheets_API.Services
{
    public class AuthService
    {
        private readonly DataBaseService _dataBaseService;
        private readonly AppSettings _appSettings;
        private readonly UserService _userService;
        

        public AuthService(UserService userService, DataBaseService dataBaseService, AppSettings appSettings)
        {
            

            _dataBaseService = dataBaseService;
            
            _userService = userService;

            _appSettings = appSettings;

            
        }

        public (string?, string?) Authenticate(string email, string password) {
            // Existing authentication code...
            var user = _dataBaseService.userCollection.Find(user => user.Email == email).FirstOrDefault();

            if (user != null && user.Password != null && user.Salt != null) {
                if (_userService.VerifyPassword(password, user.Password, user.Salt)) {
                    // Password is valid, proceed with successful login
                    Console.WriteLine("Login valid");
                } else {
                    Console.WriteLine("Login invalid");
                    return (null, null);
                }
            }

            // Generate refresh token
            var refreshToken = GenerateRefreshToken();

            // Store the refresh token in the user's record in the database
            user.RefreshToken = refreshToken;
            _dataBaseService.userCollection.ReplaceOne(u => u.Id == user.Id, user);

            // Generate and return the access token
            // Existing code to create claims and JWT...
            JwtSecurityToken jwt = GenerateAccessToken(user);

            return (new JwtSecurityTokenHandler().WriteToken(jwt), refreshToken);
        }

        
        private JwtSecurityToken GenerateAccessToken(User user) {
            var secretKey = _appSettings.SecretKey;
            Console.WriteLine($"Secret Key: {secretKey}");
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

            var claims = new[]
            {
                    new Claim(ClaimTypes.Email, user?.Email!)
                };

            var jwt = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            return jwt;
        }

        private string GenerateRefreshToken() {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public string? RefreshToken(string refreshToken) {
            var user = _dataBaseService.userCollection.Find(user => user.RefreshToken == refreshToken).FirstOrDefault();

            if (user == null) {
                // Refresh token not found, handle accordingly (e.g., return null or throw an exception)
                return null;
            }

            // Check if the refresh token has expired, and handle accordingly (e.g., return null or throw an exception)

            // Generate a new access token
            JwtSecurityToken jwt = GenerateAccessToken(user);
            // Existing code to create claims and JWT...

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

    }
}
