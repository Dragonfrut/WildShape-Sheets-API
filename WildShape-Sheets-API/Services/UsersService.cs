using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WildShape_Sheets_API.Models;

namespace WildShape_Sheets_API.Services {
    public class UsersService {

        private readonly IMongoCollection<Users> _usersCollection;

        public UsersService(IOptions<WildshapeSheetsDBSettings> wildshapeSheetsDBSettings) {
            var mongoClient = new MongoClient(
                wildshapeSheetsDBSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                wildshapeSheetsDBSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<Users>(
                wildshapeSheetsDBSettings.Value.UsersCollectionName);
        }

        public async Task<List<Users>> GetAsync() =>
            await _usersCollection.Find(_ => true).ToListAsync();

        public async Task<Users> GetAsync(string id) =>
            await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Users newClient) =>
        await _usersCollection.InsertOneAsync(newClient);

        public async Task UpdateAsync(string id, Users updatedClient) =>
            await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedClient);

        public async Task RemoveAsync(string id) =>
            await _usersCollection.DeleteOneAsync(x => x.Id == id);
    }
}
