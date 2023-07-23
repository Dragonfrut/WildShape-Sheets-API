using Microsoft.AspNetCore.Mvc;
using WildShape_Sheets_API.Models;
using WildShape_Sheets_API.Services;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Cryptography;
using System.Text;
using AspNetCore.Identity.MongoDbCore.Models;

namespace WildShape_Sheets_API.Controllers {

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller {

        private readonly UserService userService;
        private readonly EmailService emailService;

        public UserController(UserService _userService, EmailService _emailService)
        {
            userService = _userService;
            emailService = _emailService;
        }

        [HttpGet]
        public ActionResult<List<User>> GetUsers() {
            return userService.GetUsers();
        }

        [HttpGet("{id:length(24)}")]
        public ActionResult<User> GetUser(string id) {
            var user = userService.GetUserById(id);
            return Json(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<User> CreateUser(User user) {
            userService.CreateUser(user);
            return Json(user);
        }

        [HttpDelete("{id:length(24)}")]
        public void DeleteUser(string id) {
            userService.DeleteUser(id);
        }

        public class SHA256Example
        {
            public static string ComputeSHA256Hash(string input)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    // Convert the input string to a byte array and compute the hash.
                    byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                    byte[] hashBytes = sha256.ComputeHash(inputBytes);

                    // Convert the byte array to a hexadecimal string.
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("x2"));
                    }

                    return sb.ToString();
                }
            }
        }

        public class PasswordResetRequest
        {
            public string email { get; set; }
            public PasswordResetRequest()
            {
                email = string.Empty;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("password/reset")]
        public ActionResult PasswordReset([FromBody] PasswordResetRequest passwordReset)
        {
            var user = userService.GetUserByEmail(passwordReset.email);
            if (user == null)
            {
                return BadRequest(new { message = "Email not found" });
            }

            // Prepare the email subject and body
            string subject = "Password Reset Request";
            string body = "Hello password reset. This is the email body.";

            // Send the email using the email service
            // emailService.SendPasswordResetEmail(passwordReset.email, subject, body);
            string hashResult = SHA256Example.ComputeSHA256Hash(user.Password);

            Console.WriteLine("Send the email");
            Console.WriteLine(hashResult);

            return Ok();

        }
        public class PasswordUpdateRequest
        {
            public string email { get; set; }
            public string token { get; set; }
            public string password { get; set; }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("password/update")]
        public ActionResult PasswordUpdate([FromBody] PasswordUpdateRequest passwordUpdate)
        {
            Console.WriteLine("process password udpate");
            var user = userService.GetUserByEmail(passwordUpdate.email);
            if (user == null)
            {
                return BadRequest(new { message = "Email not found" });
            }
            
            if (user.Password == null)
            {
                // Handle the situation where the user's password is null
                // Return an error response or take appropriate action
                return BadRequest(new { message = "User password is null" });
            }

            string hashResult = SHA256Example.ComputeSHA256Hash(user.Password);
            if (hashResult != passwordUpdate.token)
            {
                Console.WriteLine("no token matchy");
                return BadRequest(new { message = "Invalid token" });
            }
            Console.WriteLine("We have a good token and pass, update away!");
            return Ok();
        }
    }
}
