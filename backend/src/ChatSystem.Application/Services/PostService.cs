using ChatSystem.Application.DTOs;
using ChatSystem.Domain.Entities;
using ChatSystem.Domain.Interfaces;

namespace ChatSystem.Application.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILikeRepository _likeRepository;

    public PostService(IPostRepository postRepository, IUserRepository userRepository, ILikeRepository likeRepository)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _likeRepository = likeRepository;
    }

    public async Task<PostDto?> GetPostByIdAsync(string id, string? currentUserId = null)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null)
            return null;

        var user = await _userRepository.GetByIdAsync(post.UserId);
        if (user == null)
            return null;

        bool isLiked = false;
        if (!string.IsNullOrEmpty(currentUserId))
        {
            var like = await _likeRepository.GetByUserAndPostAsync(currentUserId, post.Id);
            isLiked = like != null;
        }

        return MapToDto(post, user, isLiked);
    }

    public async Task<IEnumerable<PostDto>> GetAllPostsAsync(string? currentUserId = null, int skip = 0, int limit = 20)
    {
        var posts = await _postRepository.GetAllAsync(skip, limit);
        return await MapPostsToDtosAsync(posts, currentUserId);
    }

    public async Task<IEnumerable<PostDto>> GetUserPostsAsync(string userId, string? currentUserId = null, int skip = 0, int limit = 20)
    {
        var posts = await _postRepository.GetByUserIdAsync(userId, skip, limit);
        return await MapPostsToDtosAsync(posts, currentUserId);
    }

    public async Task<PostDto> CreatePostAsync(string userId, CreatePostDto createPostDto)
    {
        var post = new Post
        {
            UserId = userId,
            Content = createPostDto.Content,
            ImageUrl = createPostDto.ImageUrl,
            VideoUrl = createPostDto.VideoUrl,
            Type = createPostDto.Type
        };

        var createdPost = await _postRepository.CreateAsync(post);
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            throw new KeyNotFoundException($"User with id {userId} not found");

        return MapToDto(createdPost, user, false);
    }

    public async Task UpdatePostAsync(string id, UpdatePostDto updatePostDto)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null)
            throw new KeyNotFoundException($"Post with id {id} not found");

        post.Content = updatePostDto.Content;
        post.ImageUrl = updatePostDto.ImageUrl;
        post.VideoUrl = updatePostDto.VideoUrl;
        post.UpdatedAt = DateTime.UtcNow;

        await _postRepository.UpdateAsync(post);
    }

    public async Task DeletePostAsync(string id)
    {
        await _postRepository.DeleteAsync(id);
    }

    public async Task<bool> ToggleLikeAsync(string postId, string userId)
    {
        var existingLike = await _likeRepository.GetByUserAndPostAsync(userId, postId);

        if (existingLike != null)
        {
            await _likeRepository.DeleteAsync(existingLike.Id);
            await _postRepository.DecrementLikesCountAsync(postId);
            return false; // unliked
        }
        else
        {
            var like = new Like
            {
                UserId = userId,
                PostId = postId
            };
            await _likeRepository.CreateAsync(like);
            await _postRepository.IncrementLikesCountAsync(postId);
            return true; // liked
        }
    }

    private async Task<IEnumerable<PostDto>> MapPostsToDtosAsync(IEnumerable<Post> posts, string? currentUserId)
    {
        var result = new List<PostDto>();

        foreach (var post in posts)
        {
            var user = await _userRepository.GetByIdAsync(post.UserId);
            if (user == null)
                continue;

            bool isLiked = false;
            if (!string.IsNullOrEmpty(currentUserId))
            {
                var like = await _likeRepository.GetByUserAndPostAsync(currentUserId, post.Id);
                isLiked = like != null;
            }

            result.Add(MapToDto(post, user, isLiked));
        }

        return result;
    }

    private static PostDto MapToDto(Post post, User user, bool isLikedByCurrentUser) => new(
        post.Id,
        post.UserId,
        user.DisplayName,
        user.AvatarUrl,
        post.Content,
        post.ImageUrl,
        post.VideoUrl,
        post.Type,
        post.CreatedAt,
        post.UpdatedAt,
        post.LikesCount,
        post.CommentsCount,
        post.SharesCount,
        isLikedByCurrentUser
    );
}
