namespace ChatSystem.Application.DTOs;

public record UserDto(
    string Id,
    string Username,
    string DisplayName,
    string? AvatarUrl,
    bool IsOnline,
    DateTime LastSeenAt,
    string? ConnectionId
);

public record CreateUserDto(
    string Username,
    string DisplayName,
    string? AvatarUrl = null
);

public record UpdateUserDto(
    string DisplayName,
    string? AvatarUrl
);
