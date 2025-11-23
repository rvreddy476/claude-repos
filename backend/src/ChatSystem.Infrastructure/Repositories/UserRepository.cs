using ChatSystem.Domain.Entities;
using ChatSystem.Domain.Interfaces;
using ChatSystem.Infrastructure.Persistence;
using MongoDB.Driver;

namespace ChatSystem.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MongoDbContext _context;

    public UserRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        try
        {
            return await _context.Users.Find(u => u.Username == username).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {

            throw;
        }
       
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<User>> GetOnlineUsersAsync()
    {
        return await _context.Users.Find(u => u.IsOnline).ToListAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        await _context.Users.InsertOneAsync(user);
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user);
    }

    public async Task DeleteAsync(string id)
    {
        await _context.Users.DeleteOneAsync(u => u.Id == id);
    }

    public async Task UpdateConnectionIdAsync(string userId, string? connectionId)
    {
        var update = Builders<User>.Update
            .Set(u => u.ConnectionId, connectionId)
            .Set(u => u.LastSeenAt, DateTime.UtcNow);

        await _context.Users.UpdateOneAsync(u => u.Id == userId, update);
    }

    public async Task SetUserOnlineStatusAsync(string userId, bool isOnline)
    {
        var update = Builders<User>.Update
            .Set(u => u.IsOnline, isOnline)
            .Set(u => u.LastSeenAt, DateTime.UtcNow);

        await _context.Users.UpdateOneAsync(u => u.Id == userId, update);
    }
}
