using ChatSystem.Application.DTOs;

namespace ChatSystem.Application.Services;

public interface IPostService
{
    Task<PostDto?> GetPostByIdAsync(string id, string? currentUserId = null);
    Task<IEnumerable<PostDto>> GetAllPostsAsync(string? currentUserId = null, int skip = 0, int limit = 20);
    Task<IEnumerable<PostDto>> GetUserPostsAsync(string userId, string? currentUserId = null, int skip = 0, int limit = 20);
    Task<PostDto> CreatePostAsync(string userId, CreatePostDto createPostDto);
    Task UpdatePostAsync(string id, UpdatePostDto updatePostDto);
    Task DeletePostAsync(string id);
    Task<bool> ToggleLikeAsync(string postId, string userId);
}
