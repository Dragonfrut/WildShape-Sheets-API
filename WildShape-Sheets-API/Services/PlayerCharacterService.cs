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

        public PlayerCharacterService(IConfiguration configuration, DataBaseService dataBaseService)
        {
            _dataBaseService = dataBaseService;

            _configuration = configuration;
        }

        public List<PlayerCharacter> GetPlayerCharacters() => _dataBaseService.playerCharacterCollection.Find(pc => true).ToList();

        public PlayerCharacter GetPlayerCharacter(string id) => _dataBaseService.playerCharacterCollection.Find(pc => pc.Id == id).FirstOrDefault();

        public PlayerCharacter CreatePlayerCharacter(User user, PlayerCharacter pc)
        {
            
            if(user.Characters?.Length > 2)
            {
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
