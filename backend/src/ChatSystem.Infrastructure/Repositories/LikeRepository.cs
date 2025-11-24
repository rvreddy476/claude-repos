using ChatSystem.Domain.Entities;
using ChatSystem.Domain.Interfaces;
using ChatSystem.Infrastructure.Persistence;
using MongoDB.Driver;

namespace ChatSystem.Infrastructure.Repositories;

public class LikeRepository : ILikeRepository
{
    private readonly IMongoCollection<Like> _likes;
    private readonly MongoDbContext _context;

    public LikeRepository(MongoDbContext context)
    {
        _context= context;
        _likes = _context.Likes;
    }

    public async Task<Like?> GetByUserAndPostAsync(string userId, string postId)
    {
        return await _likes.Find(l => l.UserId == userId && l.PostId == postId).FirstOrDefaultAsync();
    }

    public async Task<Like?> GetByUserAndCommentAsync(string userId, string commentId)
    {
        return await _likes.Find(l => l.UserId == userId && l.CommentId == commentId).FirstOrDefaultAsync();
    }

    public async Task<Like> CreateAsync(Like like)
    {
        await _likes.InsertOneAsync(like);
        return like;
    }

    public async Task DeleteAsync(string id)
    {
        await _likes.DeleteOneAsync(l => l.Id == id);
    }
}
