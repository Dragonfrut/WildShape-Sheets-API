using MongoDB.Bson.Serialization.Attributes;
using System.Reflection.PortableExecutable;

namespace WildShape_Sheets_API.Models {
    public class User {

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public required string Id { get; set; }

        [BsonElement("username")]
        public string? Username { get; set; }

        [BsonElement("password")]
        public required string Password { get; set; }

        [BsonElement("email")]
        public required string Email { get; set; }

        [BsonElement("playerCharacters")]
        public List<PlayerCharacter> Characters { get; set; } = new List<PlayerCharacter>();

        [BsonElement("salt")]
        public byte[]? Salt { get; set; }

    }
}
