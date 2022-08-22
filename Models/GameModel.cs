using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ThiefVladislavBot.Models
{
    public class GameModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserId")]
        public long UserId { get; set; }

        [BsonElement("ChatId")]
        public long ChatId { get; set; }

        [BsonElement("IsGoldenKey")]
        public bool IsGoldenKey { get; set; }

        [BsonElement("DidAttackByGoldenKey")]
        public bool DidAttackByGoldenKey { get; set; }

        [BsonElement("IsRustyKey")]
        public bool IsRustyKey { get; set; }

        [BsonElement("DidAttackByRustyKey")]
        public bool DidAttackByRustyKey { get; set; }

        [BsonElement("IsOver")]
        public bool IsOver { get; set; }

        [BsonElement("IsWCVisited")]
        public bool IsWCVisited { get; set; }

        [BsonElement("IsMadeBed")]
        public bool IsMadeBed { get; set; }

        [BsonElement("IsUnmadeBed")]
        public bool IsUnmadeBed { get; set; }

        [BsonElement("IsRaidMode")]
        public bool IsRaidMode { get; set; }

        [BsonElement("LocationsToGo")]
        public List<string> LocationsToGo { get; set; }

        [BsonElement("LocationRightNow")]
        public string LocationRightNow { get; set; }

        [BsonElement("LastWords")]
        public string LastWords { get; set; }

        [BsonElement("LastSticker")]
        public string LastSticker { get; set; }
    }
}
