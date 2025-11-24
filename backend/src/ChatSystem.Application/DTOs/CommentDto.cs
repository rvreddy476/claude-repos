namespace ChatSystem.Application.DTOs;

public record CommentDto(
    string Id,
    string PostId,
    string UserId,
    string UserDisplayName,
    string? UserAvatarUrl,
    string Content,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int LikesCount,
    bool IsLikedByCurrentUser
);

public record CreateCommentDto(
    string PostId,
    string Content
);

public record UpdateCommentDto(
    string Content
);
