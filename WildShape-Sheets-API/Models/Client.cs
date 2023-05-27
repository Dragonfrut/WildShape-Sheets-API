using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WildShape_Sheets_API.Models {
    public class Client {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public DateTime RegistrationTime { get; set; }

        public Client()
        {
            
        }


    }
}
