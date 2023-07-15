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
        private readonly DataBaseService _dataBaseService;
        private readonly AppSettings _appSettings;
        private readonly UserService _userService;
        

        public AuthService(UserService userService, DataBaseService dataBaseService, AppSettings appSettings)
        {
            

            _dataBaseService = dataBaseService;
            
            _userService = userService;

            _appSettings = appSettings;

            
        }

        public  string? Authenticate(string email, string password)
        {

            
            var user = _dataBaseService.userCollection.Find(user => user.Email == email).FirstOrDefault();

            if (user != null && user.Password != null && user.Salt != null) {
                if (_userService.VerifyPassword(password, user.Password, user.Salt)) {
                    // Password is valid, proceed with successful login
                    Console.WriteLine("Login valid");
                } else {
                    Console.WriteLine("Login invalid");
                    return null;
                }
            }
            var secretKey = _appSettings.SecretKey;
            Console.WriteLine($"Secret Key: {secretKey}");
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user?.Email!)
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
