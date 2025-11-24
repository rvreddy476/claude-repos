using ChatSystem.Domain.Entities;
using ChatSystem.Domain.Interfaces;
using ChatSystem.Infrastructure.Persistence;
using MongoDB.Driver;

namespace ChatSystem.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly IMongoCollection<Comment> _comments;
    private readonly MongoDbContext _context;
    public CommentRepository(MongoDbContext context)
    {
        _context = context;
        _comments = _context.Comments;
    }

    public async Task<Comment?> GetByIdAsync(string id)
    {
        return await _comments.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Comment>> GetByPostIdAsync(string postId, int skip = 0, int limit = 3)
    {
        return await _comments.Find(c => c.PostId == postId)
            .SortByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<int> GetCountByPostIdAsync(string postId)
    {
        return (int)await _comments.CountDocumentsAsync(c => c.PostId == postId);
    }

    public async Task<Comment> CreateAsync(Comment comment)
    {
        await _comments.InsertOneAsync(comment);
        return comment;
    }

    public async Task UpdateAsync(Comment comment)
    {
        await _comments.ReplaceOneAsync(c => c.Id == comment.Id, comment);
    }

    public async Task DeleteAsync(string id)
    {
        await _comments.DeleteOneAsync(c => c.Id == id);
    }

    public async Task IncrementLikesCountAsync(string commentId)
    {
        var update = Builders<Comment>.Update.Inc(c => c.LikesCount, 1);
        await _comments.UpdateOneAsync(c => c.Id == commentId, update);
    }

    public async Task DecrementLikesCountAsync(string commentId)
    {
        var update = Builders<Comment>.Update.Inc(c => c.LikesCount, -1);
        await _comments.UpdateOneAsync(c => c.Id == commentId, update);
    }
}
