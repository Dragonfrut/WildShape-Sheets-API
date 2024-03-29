﻿using MongoDB.Bson.Serialization.Attributes;
using System.Reflection.PortableExecutable;

namespace WildShape_Sheets_API.Models {
    public class User {

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("username")]
        public string? Username { get; set; }

        [BsonElement("password")]
        public string? Password { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("playerCharacters")]
        public List<PlayerCharacter> Characters { get; set; } = new List<PlayerCharacter>();

        [BsonElement("salt")]
        public byte[]? Salt { get; set; }

        [BsonElement("refreshToken")]
        public string? RefreshToken { get; set; }

        [BsonElement("refreshTokenExpiration")]
        public DateTime RefreshTokenExpiration { get; internal set; }
    }
}
