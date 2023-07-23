using Microsoft.AspNetCore.Mvc;
using WildShape_Sheets_API.Models;
using WildShape_Sheets_API.Services;
using Microsoft.AspNetCore.Authorization;

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

        public class PasswordResetRequest
        {
            public string email { get; set; }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("password/reset")]
        public ActionResult PasswordReset([FromBody] PasswordResetRequest passwordReset)
        {

            // Validate the emailForm, e.g., using ModelState.IsValid if needed
            /*
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Email is missing or empty." });
            }
            */
            var user = userService.GetUserByEmail(passwordReset.email);
            if (user == null)
            {
                return BadRequest(new { message = "Email not found" });
            }
            else
            {
                // Prepare the email subject and body
                string subject = "Password Reset Request";
                string body = "Hello password reset. This is the email body.";

                // Send the email using the email service
                emailService.SendPasswordResetEmail(passwordReset.email, subject, body);

                return Ok();
            }
            return Ok();
        }
    }
}
