using Microsoft.AspNetCore.Mvc;
using WildShape_Sheets_API.Models;
using WildShape_Sheets_API.Services;
using Microsoft.AspNetCore.Authorization;
using WildShape_Sheets_API.DTO;

namespace WildShape_Sheets_API.Controllers {

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller {


        private readonly IConfiguration _configuration;
        private readonly UserService _userService;
        private readonly EmailService _emailService;
        private readonly HashService _hashService;

        public UserController(UserService userService, EmailService emailService, HashService hashService, IConfiguration configuration)
        {
            _userService = userService;
            _emailService = emailService;
            _hashService = hashService;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(_configuration));

        }

        [HttpGet]
        public ActionResult<List<User>> GetUsers() {
            return _userService.GetUsers();
        }

        [HttpGet("{id:length(24)}")]
        public ActionResult<User> GetUser(string id) {
            var user = _userService.GetUserById(id);
            return Json(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<User> CreateUser(User user) {
            _userService.CreateUser(user);
            return Json(user);
        }

        [HttpDelete("{id:length(24)}")]
        public void DeleteUser(string id) {
            _userService.DeleteUser(id);
        }

        //public class PasswordResetRequest
        //{
        //    public string email { get; set; }
        //    public PasswordResetRequest()
        //    {
        //        email = string.Empty;
        //    }
        //}

        [AllowAnonymous]
        [HttpPost]
        [Route("password/reset")]
        public ActionResult PasswordReset([FromBody] PasswordResetRequestDto passwordReset)
        {
            var user = _userService.GetUserByEmail(passwordReset.email);
            if (user == null)
            {
                return BadRequest(new { message = "Email not found" });
            }

            // Prepare the email subject and body
            string subject = "Password Reset Request";
            string passwordResetToken = _hashService.GetSHA256Hash(user.Password);
            string frontEndUrl = _configuration["URLs:Frontend"];
            string body = String.Format("To reset you password please follow <a href=\"{0}?token={1}\">this link</a>", frontEndUrl, passwordResetToken);

            // Send the email using the email service

            _emailService.SendPasswordResetEmail(passwordReset.email, subject, body);
            string _passwordResetToken = _hashService.GetSHA256Hash(user.Password);

            Console.WriteLine("Send the email");
            Console.WriteLine(passwordResetToken);

            // Create a response object with the hashResult (this is temporary for testing until email is working)
            var responseObj = new
            {
                message = "Pretend email sent successfully.",
                token = passwordResetToken
            };

            return Ok(responseObj);


        }
        //public class PasswordUpdateRequest
        //{
        //    public string email { get; set; }
        //    public string token { get; set; }
        //    public string password { get; set; }
        //}

        [AllowAnonymous]
        [HttpPost]
        [Route("password/update")]
        public ActionResult PasswordUpdate([FromBody] PasswordUpdateRequestDto passwordUpdate)
        {
            Console.WriteLine("process password udpate");
            var user = _userService.GetUserByEmail(passwordUpdate.email);
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

            string hashResult = _hashService.GetSHA256Hash(user.Password);
            if (hashResult != passwordUpdate.token)
            {
                Console.WriteLine("no token matchy");
                return BadRequest(new { message = "Invalid token" });
            }
            Console.WriteLine("We have a good token and pass, update away!");
            user.Password = passwordUpdate.password;
            _userService.SetPassword(user);
            _userService.UpdateUser(user);
            return Ok();
        }
    }
}
