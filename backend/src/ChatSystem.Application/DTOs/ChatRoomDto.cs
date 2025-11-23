namespace ChatSystem.Application.DTOs;

public record ChatRoomDto(
    string Id,
    string Name,
    string? Description,
    List<string> ParticipantIds,
    DateTime CreatedAt,
    string CreatedBy,
    bool IsPrivate
);

public record CreateChatRoomDto(
    string Name,
    string? Description,
    bool IsPrivate,
    List<string>? ParticipantIds = null
);
