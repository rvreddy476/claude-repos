using ChatSystem.Domain.Entities;
using ChatSystem.Domain.Interfaces;
using ChatSystem.Infrastructure.Persistence;
using MongoDB.Driver;

namespace ChatSystem.Infrastructure.Repositories;

public class ChatRoomRepository : IChatRoomRepository
{
    private readonly MongoDbContext _context;

    public ChatRoomRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<ChatRoom?> GetByIdAsync(string id)
    {
        return await _context.ChatRooms.Find(r => r.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ChatRoom>> GetAllAsync()
    {
        return await _context.ChatRooms.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<ChatRoom>> GetUserRoomsAsync(string userId)
    {
        return await _context.ChatRooms
            .Find(r => r.ParticipantIds.Contains(userId))
            .ToListAsync();
    }

    public async Task<ChatRoom> CreateAsync(ChatRoom chatRoom)
    {
        await _context.ChatRooms.InsertOneAsync(chatRoom);
        return chatRoom;
    }

    public async Task UpdateAsync(ChatRoom chatRoom)
    {
        await _context.ChatRooms.ReplaceOneAsync(r => r.Id == chatRoom.Id, chatRoom);
    }

    public async Task DeleteAsync(string id)
    {
        await _context.ChatRooms.DeleteOneAsync(r => r.Id == id);
    }

    public async Task AddParticipantAsync(string roomId, string userId)
    {
        var update = Builders<ChatRoom>.Update.AddToSet(r => r.ParticipantIds, userId);
        await _context.ChatRooms.UpdateOneAsync(r => r.Id == roomId, update);
    }

    public async Task RemoveParticipantAsync(string roomId, string userId)
    {
        var update = Builders<ChatRoom>.Update.Pull(r => r.ParticipantIds, userId);
        await _context.ChatRooms.UpdateOneAsync(r => r.Id == roomId, update);
    }
}
