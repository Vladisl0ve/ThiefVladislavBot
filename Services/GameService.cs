using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using ThiefVladislavBot.Database;
using ThiefVladislavBot.Models;

namespace ThiefVladislavBot.Services
{
    public class GameService
    {
        private IMongoCollection<GameModel> _games;
        public GameService(IDatabaseSettings settings)
        {
            var databaseSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
            var client = new MongoClient(databaseSettings);
            var database = client.GetDatabase(settings.DatabaseName);

            _games = database.GetCollection<GameModel>(settings.GamesCollectionName);
        }

        public List<GameModel> Get() => _games.Find(u => true).ToList();
        public GameModel Get(long userId, long chatId) => _games.Find(u => u.UserId == userId && u.ChatId == chatId).FirstOrDefault();
        public GameModel Create(GameModel game)
        {
            _games.InsertOne(game);
            return game;
        }
        public GameModel Create(long userId, long chatId)
        {
            return Create(new GameModel()
            {
                ChatId = chatId,
                UserId = userId,
                IsGoldenKey = false,
                IsRustyKey = false,
                IsOver = false,
                IsWCVisited = false,
                IsMadeBed = false,
                IsUnmadeBed = false,
                DidAttackByRustyKey = false,
                DidAttackByGoldenKey = false,
                IsRaidMode = false,
                LastSticker = "",
                LastWords = "",
                LocationRightNow = "Прихожая",
                LocationsToGo = new List<string>() { "Осмотреться", "Зал", "Туалет" }

            });
        }

        public GameModel Update(GameModel oldGame, GameModel newGame)
        {
            _games.ReplaceOne(g => g.UserId == oldGame.UserId && g.ChatId == oldGame.ChatId, newGame);
            return newGame;
        }
        public GameModel Update(long userId, long chatId, GameModel newGame)
        {
            _games.ReplaceOne(g => g.UserId == userId && g.ChatId == chatId, newGame);
            return newGame;
        }

    }
}
