using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WildShape_Sheets_API.Models;

namespace WildShape_Sheets_API.Services
{
    public class PlayerCharacterService
    {
        private readonly IMongoCollection<PlayerCharacter> playerChars;

        public PlayerCharacterService(IOptions<WildshapeSheetsDBSettings> wildshapeSheetsDBSettings, IConfiguration configuration)
        {
            var mongoClient = new MongoClient(
                wildshapeSheetsDBSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                wildshapeSheetsDBSettings.Value.DatabaseName);

            playerChars = mongoDatabase.GetCollection<PlayerCharacter>(
                wildshapeSheetsDBSettings.Value.PlayerCharactersCollectionName);
        }

        public List<PlayerCharacter> GetPlayerCharacters() => playerChars.Find(pc => true).ToList();

        public PlayerCharacter GetPlayerCharacter(string id) => playerChars.Find(pc => pc.Id == id).FirstOrDefault();

        public PlayerCharacter CreatePlayerCharacter(User user, PlayerCharacter pc)
        {
            
            if(user.Characters?.Length > 2)
            {
                playerChars.InsertOne(pc);
                user.Characters.Append(pc).ToArray();
                return pc;
            }
            Console.WriteLine("Characters are full");
            return null;
        }

        public void DeletePlayerCharacter(string id) => playerChars.DeleteOne(pc => pc.Id == id);
    }
}
