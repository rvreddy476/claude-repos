using ChatSystem.Domain.Entities;

namespace ChatSystem.Domain.Interfaces;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(string id);
    Task<IEnumerable<Post>> GetAllAsync(int skip = 0, int limit = 20);
    Task<IEnumerable<Post>> GetByUserIdAsync(string userId, int skip = 0, int limit = 20);
    Task<Post> CreateAsync(Post post);
    Task UpdateAsync(Post post);
    Task DeleteAsync(string id);
    Task IncrementLikesCountAsync(string postId);
    Task DecrementLikesCountAsync(string postId);
    Task IncrementCommentsCountAsync(string postId);
    Task DecrementCommentsCountAsync(string postId);
    Task IncrementSharesCountAsync(string postId);
}
