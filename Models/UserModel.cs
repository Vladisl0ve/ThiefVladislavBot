using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ThiefVladislavBot.Models
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Id")]
        public long UserId { get; set; }

        [BsonElement("ChatId")]
        public long ChatId { get; set; }

        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("FirstName")]
        public string FirstName { get; set; }

        [BsonElement("LastName")]
        public string LastName { get; set; }
    }
}
