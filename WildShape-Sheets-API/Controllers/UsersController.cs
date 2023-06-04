using Microsoft.AspNetCore.Mvc;
using WildShape_Sheets_API.Models;
using WildShape_Sheets_API.Services;

namespace WildShape_Sheets_API.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller {

        private readonly UsersService _usersService;

        public UsersController(UsersService clientsService) =>
            _usersService = clientsService;

        [HttpGet]
        public async Task<List<Users>> Get() =>
            await _usersService.GetAsync();

        [HttpPost]
        public async Task<IActionResult> Post(Users newUser) {
            await _usersService.CreateAsync(newUser);

            return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Users updatedUser) {
            var user = await _usersService.GetAsync(id);

            if (user is null) {
                return NotFound();
            }

            updatedUser.Id = user.Id;

            await _usersService.UpdateAsync(id, updatedUser);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id) {
            var user = await _usersService.GetAsync(id);

            if (user is null) {
                return NotFound();
            }

            await _usersService.RemoveAsync(id);

            return NoContent();
        }
    }
}
