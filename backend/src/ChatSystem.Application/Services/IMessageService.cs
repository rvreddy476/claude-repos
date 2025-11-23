using ChatSystem.Application.DTOs;

namespace ChatSystem.Application.Services;

public interface IMessageService
{
    Task<MessageDto?> GetMessageByIdAsync(string id);
    Task<IEnumerable<MessageDto>> GetRoomMessagesAsync(string chatRoomId, int limit = 50, int skip = 0);
    Task<MessageDto> SendMessageAsync(SendMessageDto sendMessageDto, string senderId, string senderName);
    Task<MessageDto> UpdateMessageAsync(string messageId, string content);
    Task DeleteMessageAsync(string id);
}
