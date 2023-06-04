using MongoDB.Bson.Serialization.Attributes;

namespace WildShape_Sheets_API.Models {
    public class Users {

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; } = null!;

        [BsonElement("password")]
        public string Password { get; set; } = null!;

        [BsonElement("email")]
        public string Email { get; set; } = null!;

    }
}
