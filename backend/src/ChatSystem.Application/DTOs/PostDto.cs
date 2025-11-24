using ChatSystem.Domain.Entities;

namespace ChatSystem.Application.DTOs;

public record PostDto(
    string Id,
    string UserId,
    string UserDisplayName,
    string? UserAvatarUrl,
    string? Content,
    string? ImageUrl,
    string? VideoUrl,
    PostType Type,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int LikesCount,
    int CommentsCount,
    int SharesCount,
    bool IsLikedByCurrentUser
);

public record CreatePostDto(
    string? Content,
    string? ImageUrl,
    string? VideoUrl,
    PostType Type
);

public record UpdatePostDto(
    string? Content,
    string? ImageUrl,
    string? VideoUrl
);
