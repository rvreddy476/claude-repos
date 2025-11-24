using ChatSystem.Domain.Entities;
using ChatSystem.Domain.Interfaces;
using ChatSystem.Infrastructure.Persistence;
using MongoDB.Driver;

namespace ChatSystem.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly MongoDbContext _context;
    private readonly IMongoCollection<Post> _posts;

    public PostRepository(MongoDbContext context)
    {
        _context = context;
        _posts = _context.Posts;
    }

    public async Task<Post?> GetByIdAsync(string id)
    {
        return await _posts.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Post>> GetAllAsync(int skip = 0, int limit = 20)
    {
        return await _posts.Find(_ => true)
            .SortByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetByUserIdAsync(string userId, int skip = 0, int limit = 20)
    {
        return await _posts.Find(p => p.UserId == userId)
            .SortByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<Post> CreateAsync(Post post)
    {
        await _posts.InsertOneAsync(post);
        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        await _posts.ReplaceOneAsync(p => p.Id == post.Id, post);
    }

    public async Task DeleteAsync(string id)
    {
        await _posts.DeleteOneAsync(p => p.Id == id);
    }

    public async Task IncrementLikesCountAsync(string postId)
    {
        var update = Builders<Post>.Update.Inc(p => p.LikesCount, 1);
        await _posts.UpdateOneAsync(p => p.Id == postId, update);
    }

    public async Task DecrementLikesCountAsync(string postId)
    {
        var update = Builders<Post>.Update.Inc(p => p.LikesCount, -1);
        await _posts.UpdateOneAsync(p => p.Id == postId, update);
    }

    public async Task IncrementCommentsCountAsync(string postId)
    {
        var update = Builders<Post>.Update.Inc(p => p.CommentsCount, 1);
        await _posts.UpdateOneAsync(p => p.Id == postId, update);
    }

    public async Task DecrementCommentsCountAsync(string postId)
    {
        var update = Builders<Post>.Update.Inc(p => p.CommentsCount, -1);
        await _posts.UpdateOneAsync(p => p.Id == postId, update);
    }

    public async Task IncrementSharesCountAsync(string postId)
    {
        var update = Builders<Post>.Update.Inc(p => p.SharesCount, 1);
        await _posts.UpdateOneAsync(p => p.Id == postId, update);
    }
}
