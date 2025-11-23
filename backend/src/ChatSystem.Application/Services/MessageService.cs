using ChatSystem.Application.DTOs;
using ChatSystem.Domain.Entities;
using ChatSystem.Domain.Interfaces;

namespace ChatSystem.Application.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly ICacheService _cacheService;

    public MessageService(IMessageRepository messageRepository, ICacheService cacheService)
    {
        _messageRepository = messageRepository;
        _cacheService = cacheService;
    }

    public async Task<MessageDto?> GetMessageByIdAsync(string id)
    {
        var message = await _messageRepository.GetByIdAsync(id);
        return message == null ? null : MapToDto(message);
    }

    public async Task<IEnumerable<MessageDto>> GetRoomMessagesAsync(string chatRoomId, int limit = 50, int skip = 0)
    {
        var cacheKey = $"room:{chatRoomId}:messages:{skip}:{limit}";
        var cachedMessages = await _cacheService.GetAsync<IEnumerable<MessageDto>>(cacheKey);

        if (cachedMessages != null)
            return cachedMessages;

        var messages = await _messageRepository.GetRoomMessagesAsync(chatRoomId, limit, skip);
        var messageDtos = messages.Select(MapToDto).ToList();

        await _cacheService.SetAsync(cacheKey, messageDtos, TimeSpan.FromMinutes(2));

        return messageDtos;
    }

    public async Task<MessageDto> SendMessageAsync(SendMessageDto sendMessageDto, string senderId, string senderName)
    {
        var message = new Message
        {
            ChatRoomId = sendMessageDto.ChatRoomId,
            SenderId = senderId,
            SenderName = senderName,
            Content = sendMessageDto.Content,
            Type = sendMessageDto.Type
        };

        var createdMessage = await _messageRepository.CreateAsync(message);

        await InvalidateRoomCache(sendMessageDto.ChatRoomId);

        return MapToDto(createdMessage);
    }

    public async Task<MessageDto> UpdateMessageAsync(string messageId, string content)
    {
        var message = await _messageRepository.GetByIdAsync(messageId);
        if (message == null)
            throw new KeyNotFoundException($"Message with id {messageId} not found");

        message.Content = content;
        message.IsEdited = true;
        message.EditedAt = DateTime.UtcNow;

        await _messageRepository.UpdateAsync(message);
        await InvalidateRoomCache(message.ChatRoomId);

        return MapToDto(message);
    }

    public async Task DeleteMessageAsync(string id)
    {
        var message = await _messageRepository.GetByIdAsync(id);
        if (message != null)
        {
            await _messageRepository.DeleteAsync(id);
            await InvalidateRoomCache(message.ChatRoomId);
        }
    }

    private async Task InvalidateRoomCache(string chatRoomId)
    {
        for (int skip = 0; skip < 200; skip += 50)
        {
            await _cacheService.RemoveAsync($"room:{chatRoomId}:messages:{skip}:50");
        }
    }

    private static MessageDto MapToDto(Message message) => new(
        message.Id,
        message.ChatRoomId,
        message.SenderId,
        message.SenderName,
        message.Content,
        message.Timestamp,
        message.Type,
        message.IsEdited,
        message.EditedAt
    );
}
