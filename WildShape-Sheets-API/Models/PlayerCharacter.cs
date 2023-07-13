using MongoDB.Bson.Serialization.Attributes;

namespace WildShape_Sheets_API.Models
{
    public class PlayerCharacter
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("level")]
        public int Level { get; set; }

        [BsonElement("user")]
        public User? User { get; set; }
    }
}
