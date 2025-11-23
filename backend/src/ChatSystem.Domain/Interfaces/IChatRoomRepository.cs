using ChatSystem.Domain.Entities;

namespace ChatSystem.Domain.Interfaces;

public interface IChatRoomRepository
{
    Task<ChatRoom?> GetByIdAsync(string id);
    Task<IEnumerable<ChatRoom>> GetAllAsync();
    Task<IEnumerable<ChatRoom>> GetUserRoomsAsync(string userId);
    Task<ChatRoom> CreateAsync(ChatRoom chatRoom);
    Task UpdateAsync(ChatRoom chatRoom);
    Task DeleteAsync(string id);
    Task AddParticipantAsync(string roomId, string userId);
    Task RemoveParticipantAsync(string roomId, string userId);
}
