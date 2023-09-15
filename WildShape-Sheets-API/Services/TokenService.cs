using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text;
using WildShape_Sheets_API.Models;
using MongoDB.Driver;
using System.Collections.Generic;

namespace WildShape_Sheets_API.Services {
    public class TokenService {
        private readonly DataBaseService _dataBaseService;
        private readonly AppSettings _appSettings;
        private const int AccessTokenExpiration = 15;

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
                expires: DateTime.UtcNow.AddMinutes(AccessTokenExpiration),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            return jwt;
        }

        internal Dictionary<string, string> DecodeToken(AuthTokens tokens) {
            var accessToken = tokens.AccessToken;
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(accessToken);

            Dictionary<string, string> claims = new Dictionary<string, string>();

            foreach (var claim in jwtToken.Claims) {
                claims.Add(claim.Type, claim.Value);
            }

            return claims;
        }
    


internal string GenerateRefreshToken(int expirationMinutes, User user) {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(randomNumber);
                string refreshToken = Convert.ToBase64String(randomNumber);
           
                var expirationTime = DateTime.UtcNow.AddMinutes(expirationMinutes);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiration = expirationTime;

                _dataBaseService.userCollection.ReplaceOne(u => u.Id == user.Id, user);

                return refreshToken;
            }
        }

        internal string? RefreshToken(string refreshToken) {
            var user = _dataBaseService.userCollection.Find(user => user.RefreshToken == refreshToken).FirstOrDefault();

            if (user == null) {
                return null;
            }

            if (user.RefreshTokenExpiration <= DateTime.UtcNow) {
                return null; 
            }

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
