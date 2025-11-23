using ChatSystem.Domain.Entities;
using ChatSystem.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ChatSystem.Infrastructure.Persistence;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users =>
        _database.GetCollection<User>("Users");

    public IMongoCollection<ChatRoom> ChatRooms =>
        _database.GetCollection<ChatRoom>("ChatRooms");

    public IMongoCollection<Message> Messages =>
        _database.GetCollection<Message>("Messages");
}
