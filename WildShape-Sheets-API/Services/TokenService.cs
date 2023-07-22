using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text;
using WildShape_Sheets_API.Models;
using MongoDB.Driver;

namespace WildShape_Sheets_API.Services {
    public class TokenService {
        private readonly DataBaseService _dataBaseService;
        private readonly AppSettings _appSettings;

        public TokenService(DataBaseService dataBaseService, AppSettings appSettings) {
            _dataBaseService = dataBaseService;
            _appSettings = appSettings;
        }
        
        internal JwtSecurityToken GenerateAccessToken(User user) {
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

        internal string GenerateRefreshToken() {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        internal string? RefreshToken(string refreshToken) {
            var user = _dataBaseService.userCollection.Find(user => user.RefreshToken == refreshToken).FirstOrDefault();

            if (user == null) {
                return null;
            }

            //TODO Check if the refresh token has expired, and handle accordingly (e.g., return null or throw an exception)

            JwtSecurityToken jwt = GenerateAccessToken(user);
            string accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            var authTokens = new {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return JsonSerializer.Serialize(authTokens);
        }
    }
}
