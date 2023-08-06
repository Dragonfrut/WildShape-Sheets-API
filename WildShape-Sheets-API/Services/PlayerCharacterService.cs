using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Reflection.Metadata;
using WildShape_Sheets_API.Models;

namespace WildShape_Sheets_API.Services
{
    public class PlayerCharacterService
    {
        private readonly DataBaseService _dataBaseService;
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;
        private const int MaxNumOfPlayerCharacters = 4;

        public PlayerCharacterService(IConfiguration configuration, DataBaseService dataBaseService, UserService userService)
        {
            _dataBaseService = dataBaseService;

            _configuration = configuration;

            _userService = userService;
        }

        public List<PlayerCharacter> GetPlayerCharacters() => _dataBaseService.playerCharacterCollection.Find(pc => true).ToList();

        public List<PlayerCharacter> GetUsersPlayerCharacters(string email) {
            var user = _userService.GetUserByEmail(email);
            return user.Characters;
        }

        public PlayerCharacter GetPlayerCharacterById(string id) => _dataBaseService.playerCharacterCollection.Find(pc => pc.Id == id).FirstOrDefault();

        public PlayerCharacter? CreatePlayerCharacter(string userEmail, PlayerCharacter pc)
        {
            var user = _userService.GetUserByEmail(userEmail);
            if (user.Characters?.Count <= MaxNumOfPlayerCharacters) {
                pc.User = user.Id;
                _dataBaseService.playerCharacterCollection.InsertOne(pc);
                user.Characters.Add(pc);
                
                _userService.UpdateUser(user);
                return pc;
            }
            Console.WriteLine("Characters are full");
            return null;
        }

        public void DeletePlayerCharacter(string id) {
            var pc = GetPlayerCharacterById(id);

            var filter = Builders<User>.Filter.ElemMatch(user => user.Characters, nestedPc => nestedPc.Id == id);
            var update = Builders<User>.Update.PullFilter(user => user.Characters, pc => pc.Id == id);
            _dataBaseService.userCollection.UpdateOne(filter, update);
            _dataBaseService.playerCharacterCollection.DeleteOne(pc => pc.Id == id);

        }

        
    }
}
