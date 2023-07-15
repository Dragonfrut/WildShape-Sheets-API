using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WildShape_Sheets_API.Models;

namespace WildShape_Sheets_API.Services {
    public class DataBaseService {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<PlayerCharacter> _playerCharacters;

        public DataBaseService(IOptions<WildshapeSheetsDBSettings> wildshapeSheetsDBSettings) {
            _client = new MongoClient(wildshapeSheetsDBSettings.Value.ConnectionString);
            _database = _client.GetDatabase(wildshapeSheetsDBSettings.Value.DatabaseName);
            _users = _database.GetCollection<User>(wildshapeSheetsDBSettings.Value.UsersCollectionName);
            //_playerCharacters = _database.GetCollection<PlayerCharacter>(wildshapeSheetsDBSettings.Value.PlayerCharactersCollectionName);
        }

        public IMongoCollection<User> userCollection => _users;
        public IMongoCollection<PlayerCharacter> playerCharacterCollection => _playerCharacters;
    }
}
