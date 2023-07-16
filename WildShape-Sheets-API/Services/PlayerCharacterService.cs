using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WildShape_Sheets_API.Models;

namespace WildShape_Sheets_API.Services
{
    public class PlayerCharacterService
    {
        private readonly DataBaseService _dataBaseService;
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;

        public PlayerCharacterService(IConfiguration configuration, DataBaseService dataBaseService, UserService userService)
        {
            _dataBaseService = dataBaseService;

            _configuration = configuration;

            _userService = userService;
        }

        public List<PlayerCharacter> GetPlayerCharacters() => _dataBaseService.playerCharacterCollection.Find(pc => true).ToList();

        public PlayerCharacter GetPlayerCharacter(string id) => _dataBaseService.playerCharacterCollection.Find(pc => pc.Id == id).FirstOrDefault();

        public PlayerCharacter CreatePlayerCharacter(string userEmail, PlayerCharacter pc)
        {
            
            var user = _userService.GetUserByEmail(userEmail);

            if(user.Characters?.Length > 2)
            {
                pc.User = user.Id;
                _dataBaseService.playerCharacterCollection.InsertOne(pc);
                user.Characters.Append(pc).ToArray();
                return pc;
            }
            Console.WriteLine("Characters are full");
            return null;
        }

        public void DeletePlayerCharacter(string id) => _dataBaseService.playerCharacterCollection.DeleteOne(pc => pc.Id == id);

        
    }
}
