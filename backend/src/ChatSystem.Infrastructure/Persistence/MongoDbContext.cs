using ChatSystem.Domain.Entities;
using ChatSystem.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Xml.Linq;

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
    public IMongoCollection<Comment> Comments =>
        _database.GetCollection<Comment>("Comments");
    public IMongoCollection<Post> Posts =>
       _database.GetCollection<Post>("Posts");

    public IMongoCollection<Like> Likes => 
        _database.GetCollection<Like>("Likes");

}
