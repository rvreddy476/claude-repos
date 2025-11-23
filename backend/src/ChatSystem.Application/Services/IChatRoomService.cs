using ChatSystem.Application.DTOs;

namespace ChatSystem.Application.Services;

public interface IChatRoomService
{
    Task<ChatRoomDto?> GetChatRoomByIdAsync(string id);
    Task<IEnumerable<ChatRoomDto>> GetAllChatRoomsAsync();
    Task<IEnumerable<ChatRoomDto>> GetUserChatRoomsAsync(string userId);
    Task<ChatRoomDto> CreateChatRoomAsync(CreateChatRoomDto createChatRoomDto, string createdBy);
    Task AddParticipantAsync(string roomId, string userId);
    Task RemoveParticipantAsync(string roomId, string userId);
}
