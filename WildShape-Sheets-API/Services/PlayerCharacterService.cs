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
            Console.WriteLine($"user email: {userEmail}");
            var user = _userService.GetUserByEmail(userEmail);
            Console.WriteLine($"array length: {user.Characters?.Count}");
            if (user.Characters?.Count <= 2) {
                pc.User = user.Id;
                _dataBaseService.playerCharacterCollection.InsertOne(pc);
                user.Characters.Add(pc);
                Console.WriteLine($"characters: {user.Characters[0]}");
                Console.WriteLine($"array length: {user.Characters?.Count}");
                _userService.UpdateUser(user.Id, user);
                return pc;
            }
            Console.WriteLine("Characters are full");
            return null;
        }

        public void DeletePlayerCharacter(string id) => _dataBaseService.playerCharacterCollection.DeleteOne(pc => pc.Id == id);

        
    }
}
