using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ThiefVladislavBot.Models
{
    public class MessageModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserId")]
        public long UserId { get; set; }

        [BsonElement("ChatId")]
        public long ChatId { get; set; }

        [BsonElement("Message")]
        public string MessageText { get; set; }
    }
}
