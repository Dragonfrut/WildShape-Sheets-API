using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WildShape_Sheets_API.DTO;
using WildShape_Sheets_API.Services;

namespace WildShape_Sheets_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly AuthService AuthService;

        public AuthenticationController(AuthService _authService) =>
            AuthService = _authService;

        
        [HttpPost]
        public ActionResult Login(LoginDto dto)
        {
            Console.WriteLine($"email: {dto.Email} and password: {dto.Password}");
            var tokens = AuthService.Authenticate(dto.Email, dto.Password);
            if (tokens == null)
                return Unauthorized();

            Console.WriteLine(tokens.ToString());
            return Ok(new { tokens });
        }

        

    }
}
