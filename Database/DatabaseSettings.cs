namespace ThiefVladislavBot.Database
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string MessagesCollectionName { get; set; }
        public string UsersCollectionName { get; set; }
        public string GamesCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

    }


    public interface IDatabaseSettings
    {
        string MessagesCollectionName { get; set; }
        string UsersCollectionName { get; set; }
        string GamesCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
