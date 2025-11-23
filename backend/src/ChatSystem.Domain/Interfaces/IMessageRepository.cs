using ChatSystem.Domain.Entities;

namespace ChatSystem.Domain.Interfaces;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(string id);
    Task<IEnumerable<Message>> GetRoomMessagesAsync(string chatRoomId, int limit = 50, int skip = 0);
    Task<Message> CreateAsync(Message message);
    Task UpdateAsync(Message message);
    Task DeleteAsync(string id);
}
