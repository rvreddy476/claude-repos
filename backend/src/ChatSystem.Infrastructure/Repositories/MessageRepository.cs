using ChatSystem.Domain.Entities;
using ChatSystem.Domain.Interfaces;
using ChatSystem.Infrastructure.Persistence;
using MongoDB.Driver;

namespace ChatSystem.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly MongoDbContext _context;

    public MessageRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Message?> GetByIdAsync(string id)
    {
        return await _context.Messages.Find(m => m.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Message>> GetRoomMessagesAsync(string chatRoomId, int limit = 50, int skip = 0)
    {
        return await _context.Messages
            .Find(m => m.ChatRoomId == chatRoomId)
            .SortByDescending(m => m.Timestamp)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<Message> CreateAsync(Message message)
    {
        await _context.Messages.InsertOneAsync(message);
        return message;
    }

    public async Task UpdateAsync(Message message)
    {
        await _context.Messages.ReplaceOneAsync(m => m.Id == message.Id, message);
    }

    public async Task DeleteAsync(string id)
    {
        await _context.Messages.DeleteOneAsync(m => m.Id == id);
    }
}
