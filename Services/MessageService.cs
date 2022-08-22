using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using ThiefVladislavBot.Database;
using ThiefVladislavBot.Models;

namespace ThiefVladislavBot.Services
{
    public class MessageService
    {
        private IMongoCollection<MessageModel> _messages;
        public MessageService(IDatabaseSettings settings)
        {
            var databaseSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
            var client = new MongoClient(databaseSettings);
            var database = client.GetDatabase(settings.DatabaseName);

            _messages = database.GetCollection<MessageModel>(settings.MessagesCollectionName);
        }

        public List<MessageModel> Get() => _messages.Find(m => true).ToList();
        public List<MessageModel> GetAllMessagesFromUser(long userId) => _messages.Find(p => p.UserId == userId).ToList();
        public MessageModel Create(MessageModel message)
        {
            _messages.InsertOne(message);
            return message;
        }
        public MessageModel Create(long userId, long chatId, string text)
        {
            return Create(new MessageModel()
            {
                ChatId = chatId,
                UserId = userId,
                MessageText = text
            });
        }

    }
}
