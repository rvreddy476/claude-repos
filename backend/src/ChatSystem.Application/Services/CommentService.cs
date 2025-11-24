using ChatSystem.Application.DTOs;
using ChatSystem.Domain.Entities;
using ChatSystem.Domain.Interfaces;

namespace ChatSystem.Application.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILikeRepository _likeRepository;

    public CommentService(
        ICommentRepository commentRepository,
        IPostRepository postRepository,
        IUserRepository userRepository,
        ILikeRepository likeRepository)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _userRepository = userRepository;
        _likeRepository = likeRepository;
    }

    public async Task<CommentDto?> GetCommentByIdAsync(string id, string? currentUserId = null)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment == null)
            return null;

        var user = await _userRepository.GetByIdAsync(comment.UserId);
        if (user == null)
            return null;

        bool isLiked = false;
        if (!string.IsNullOrEmpty(currentUserId))
        {
            var like = await _likeRepository.GetByUserAndCommentAsync(currentUserId, comment.Id);
            isLiked = like != null;
        }

        return MapToDto(comment, user, isLiked);
    }

    public async Task<IEnumerable<CommentDto>> GetPostCommentsAsync(string postId, string? currentUserId = null, int skip = 0, int limit = 3)
    {
        var comments = await _commentRepository.GetByPostIdAsync(postId, skip, limit);
        return await MapCommentsToDtosAsync(comments, currentUserId);
    }

    public async Task<int> GetPostCommentsCountAsync(string postId)
    {
        return await _commentRepository.GetCountByPostIdAsync(postId);
    }

    public async Task<CommentDto> CreateCommentAsync(string userId, CreateCommentDto createCommentDto)
    {
        var comment = new Comment
        {
            PostId = createCommentDto.PostId,
            UserId = userId,
            Content = createCommentDto.Content
        };

        var createdComment = await _commentRepository.CreateAsync(comment);
        await _postRepository.IncrementCommentsCountAsync(createCommentDto.PostId);

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with id {userId} not found");

        return MapToDto(createdComment, user, false);
    }

    public async Task UpdateCommentAsync(string id, UpdateCommentDto updateCommentDto)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment == null)
            throw new KeyNotFoundException($"Comment with id {id} not found");

        comment.Content = updateCommentDto.Content;
        comment.UpdatedAt = DateTime.UtcNow;

        await _commentRepository.UpdateAsync(comment);
    }

    public async Task DeleteCommentAsync(string id)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment == null)
            throw new KeyNotFoundException($"Comment with id {id} not found");

        await _commentRepository.DeleteAsync(id);
        await _postRepository.DecrementCommentsCountAsync(comment.PostId);
    }

    public async Task<bool> ToggleLikeAsync(string commentId, string userId)
    {
        var existingLike = await _likeRepository.GetByUserAndCommentAsync(userId, commentId);

        if (existingLike != null)
        {
            await _likeRepository.DeleteAsync(existingLike.Id);
            await _commentRepository.DecrementLikesCountAsync(commentId);
            return false; // unliked
        }
        else
        {
            var like = new Like
            {
                UserId = userId,
                CommentId = commentId
            };
            await _likeRepository.CreateAsync(like);
            await _commentRepository.IncrementLikesCountAsync(commentId);
            return true; // liked
        }
    }

    private async Task<IEnumerable<CommentDto>> MapCommentsToDtosAsync(IEnumerable<Comment> comments, string? currentUserId)
    {
        var result = new List<CommentDto>();

        foreach (var comment in comments)
        {
            var user = await _userRepository.GetByIdAsync(comment.UserId);
            if (user == null)
                continue;

            bool isLiked = false;
            if (!string.IsNullOrEmpty(currentUserId))
            {
                var like = await _likeRepository.GetByUserAndCommentAsync(currentUserId, comment.Id);
                isLiked = like != null;
            }

            result.Add(MapToDto(comment, user, isLiked));
        }

        return result;
    }

    private static CommentDto MapToDto(Comment comment, User user, bool isLikedByCurrentUser) => new(
        comment.Id,
        comment.PostId,
        comment.UserId,
        user.DisplayName,
        user.AvatarUrl,
        comment.Content,
        comment.CreatedAt,
        comment.UpdatedAt,
        comment.LikesCount,
        isLikedByCurrentUser
    );
}
