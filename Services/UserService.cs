using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using ThiefVladislavBot.Database;
using ThiefVladislavBot.Models;

namespace ThiefVladislavBot.Services
{
    public class UserService
    {
        private IMongoCollection<UserModel> _users;
        public UserService(IDatabaseSettings settings)
        {
            var databaseSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
            var client = new MongoClient(databaseSettings);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<UserModel>(settings.UsersCollectionName);
        }
        public List<UserModel> Get() => _users.Find(u => true).ToList();
        public UserModel Get(long userId, long chatId) => _users.Find(u => u.UserId == userId && u.ChatId == chatId).FirstOrDefault();
        public UserModel Create(UserModel user)
        {
            _users.InsertOne(user);
            return user;
        }
        public UserModel Create(Telegram.Bot.Types.User user, long chatId)
        {
            return Create(new UserModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserId = user.Id,
                Username = user.Username,
                ChatId = chatId
            });
        }
        public UserModel Update(long userId, UserModel userIn)
        {
            _users.ReplaceOne(u => u.UserId == userId, userIn);
            return userIn;
        }
        public UserModel Update(long userId, Telegram.Bot.Types.User userIn)
        {
            var oldUser = _users.Find(u => u.UserId == userId).First();
            UserModel newUser = new UserModel()
            {
                Id = oldUser.Id,
                FirstName = userIn.FirstName,
                LastName = userIn.LastName,
                UserId = userIn.Id,
                Username = userIn.Username,
                ChatId = oldUser.ChatId
            };
            _users.ReplaceOne(u => u.UserId == userId, newUser);
            return newUser;
        }

        public void Remove(long userId) => _users.DeleteOne(u => u.UserId == userId);

    }
}
