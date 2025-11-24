using ChatSystem.Application.DTOs;

namespace ChatSystem.Application.Services;

public interface ICommentService
{
    Task<CommentDto?> GetCommentByIdAsync(string id, string? currentUserId = null);
    Task<IEnumerable<CommentDto>> GetPostCommentsAsync(string postId, string? currentUserId = null, int skip = 0, int limit = 3);
    Task<int> GetPostCommentsCountAsync(string postId);
    Task<CommentDto> CreateCommentAsync(string userId, CreateCommentDto createCommentDto);
    Task UpdateCommentAsync(string id, UpdateCommentDto updateCommentDto);
    Task DeleteCommentAsync(string id);
    Task<bool> ToggleLikeAsync(string commentId, string userId);
}
