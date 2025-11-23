namespace ChatSystem.Infrastructure.Configuration;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "ChatSystemDb";
    public string UsersCollectionName { get; set; } = "Users";
    public string ChatRoomsCollectionName { get; set; } = "ChatRooms";
    public string MessagesCollectionName { get; set; } = "Messages";
}
