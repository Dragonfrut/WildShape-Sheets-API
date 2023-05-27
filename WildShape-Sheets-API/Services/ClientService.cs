using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WildShape_Sheets_API.Models;

namespace WildShape_Sheets_API.Services {
    public class ClientService {
        private readonly IMongoCollection<Client> _clientCollection;

        public ClientService(IOptions<ClientDatabaseSetings> clientDatabaseSettings) {
            var mongoClient = new MongoClient(
                clientDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
            clientDatabaseSettings.Value.DatabaseName);

            _clientCollection = mongoDatabase.GetCollection<Client>(
                clientDatabaseSettings.Value.BooksCollectionName);
        }

        public async Task<List<Client>> GetAsync() =>
            await _clientCollection.Find(_ => true).ToListAsync();


        
    }
}
