using Microsoft.AspNetCore.Mvc;
using WildShape_Sheets_API.Models;
using WildShape_Sheets_API.Services;
using Microsoft.AspNetCore.Authorization;

namespace WildShape_Sheets_API.Controllers {

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller {

        private readonly UserService _userService;

        public UserController(UserService userService) =>
            _userService = userService;

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

        }
    }
