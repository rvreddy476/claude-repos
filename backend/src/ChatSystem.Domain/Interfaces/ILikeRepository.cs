using ChatSystem.Domain.Entities;

namespace ChatSystem.Domain.Interfaces;

public interface ILikeRepository
{
    Task<Like?> GetByUserAndPostAsync(string userId, string postId);
    Task<Like?> GetByUserAndCommentAsync(string userId, string commentId);
    Task<Like> CreateAsync(Like like);
    Task DeleteAsync(string id);
}
