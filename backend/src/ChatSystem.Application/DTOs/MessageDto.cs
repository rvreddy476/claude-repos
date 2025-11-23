using ChatSystem.Domain.Entities;

namespace ChatSystem.Application.DTOs;

public record MessageDto(
    string Id,
    string ChatRoomId,
    string SenderId,
    string SenderName,
    string Content,
    DateTime Timestamp,
    MessageType Type,
    bool IsEdited,
    DateTime? EditedAt
);

public record SendMessageDto(
    string ChatRoomId,
    string Content,
    MessageType Type = MessageType.Text
);
