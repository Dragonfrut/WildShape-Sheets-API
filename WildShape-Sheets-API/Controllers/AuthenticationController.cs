﻿using Microsoft.AspNetCore.Authorization;
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
            var token = AuthService.Authenticate(dto.Email, dto.Password);
            if (token == null)
                return Unauthorized();

            return Ok(new { token });
        }

        

    }
}
