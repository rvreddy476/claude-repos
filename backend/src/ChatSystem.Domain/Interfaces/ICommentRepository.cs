using ChatSystem.Domain.Entities;

namespace ChatSystem.Domain.Interfaces;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(string id);
    Task<IEnumerable<Comment>> GetByPostIdAsync(string postId, int skip = 0, int limit = 3);
    Task<int> GetCountByPostIdAsync(string postId);
    Task<Comment> CreateAsync(Comment comment);
    Task UpdateAsync(Comment comment);
    Task DeleteAsync(string id);
    Task IncrementLikesCountAsync(string commentId);
    Task DecrementLikesCountAsync(string commentId);
}
