using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WildShape_Sheets_API.Models;

namespace WildShape_Sheets_API.Services
{
    public class AuthService
    {
        private readonly DataBaseService _dataBaseService;
        private readonly UserService _userService;
        private readonly TokenService _tokenService;
        private const int RefreshTokenExpiration = 60 * 12;
        

        public AuthService(UserService userService, DataBaseService dataBaseService, TokenService tokenService)
        {
            _dataBaseService = dataBaseService;
            _userService = userService;
            _tokenService = tokenService;
        }

        public string Authenticate(string email, string password) {
            
            var user = _dataBaseService.userCollection.Find(user => user.Email == email).SingleOrDefault();

            if (user != null && user.Password != null && user.Salt != null) {
                if (_userService.VerifyPassword(password, user.Password, user.Salt)) {
                    Console.WriteLine("Login valid");
                } else {
                    Console.WriteLine("Login invalid");
                    return null;
                }
            }

            JwtSecurityToken jwt = _tokenService.GenerateAccessToken(user);
            string accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
            string refreshToken = _tokenService.GenerateRefreshToken(RefreshTokenExpiration, user);

            //user.RefreshToken = refreshToken;
            //_dataBaseService.userCollection.ReplaceOne(u => u.Id == user.Id, user);

            var authTokens = new AuthTokens {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return JsonSerializer.Serialize(authTokens);
        }
    }
}
