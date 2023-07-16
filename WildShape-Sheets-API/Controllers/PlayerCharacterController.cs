using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WildShape_Sheets_API.Models;
using WildShape_Sheets_API.Services;

namespace WildShape_Sheets_API.Controllers {

    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerCharacterController : Controller {
        
        private readonly PlayerCharacterService _playerCharacterService;

        public PlayerCharacterController(PlayerCharacterService playerCharacterService) {
            _playerCharacterService = playerCharacterService;
        }

        [HttpGet]
        public ActionResult<List<PlayerCharacter>> GetPlayerCharacters() { 
            return _playerCharacterService.GetPlayerCharacters();
        }

        [HttpPost]
        public ActionResult<PlayerCharacter> CreatePlayerCharacter(string userEmail, PlayerCharacter playerCharacter) {
            _playerCharacterService.CreatePlayerCharacter(userEmail, playerCharacter);
            return Json(playerCharacter);
        }


    }
}
