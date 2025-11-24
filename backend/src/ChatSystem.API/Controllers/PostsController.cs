using ChatSystem.Application.DTOs;
using ChatSystem.Application.Services;
using ChatSystem.API.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IHubContext<FeedHub> _feedHubContext;

    public PostsController(IPostService postService, IHubContext<FeedHub> feedHubContext)
    {
        _postService = postService;
        _feedHubContext = feedHubContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetAllPosts([FromQuery] string? userId, [FromQuery] int skip = 0, [FromQuery] int limit = 20)
    {
        var posts = await _postService.GetAllPostsAsync(userId, skip, limit);
        return Ok(posts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PostDto>> GetPost(string id, [FromQuery] string? userId)
    {
        var post = await _postService.GetPostByIdAsync(id, userId);
        if (post == null)
            return NotFound();

        return Ok(post);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetUserPosts(string userId, [FromQuery] string? currentUserId, [FromQuery] int skip = 0, [FromQuery] int limit = 20)
    {
        var posts = await _postService.GetUserPostsAsync(userId, currentUserId, skip, limit);
        return Ok(posts);
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost([FromQuery] string userId, [FromBody] CreatePostDto createPostDto)
    {
        var post = await _postService.CreatePostAsync(userId, createPostDto);

        // Broadcast the new post to all connected clients
        await _feedHubContext.Clients.All.SendAsync("NewPostCreated", post);

        return CreatedAtAction(nameof(GetPost), new { id = post.Id, userId }, post);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(string id, [FromBody] UpdatePostDto updatePostDto)
    {
        try
        {
            await _postService.UpdatePostAsync(id, updatePostDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(string id)
    {
        await _postService.DeletePostAsync(id);

        // Broadcast post deletion
        await _feedHubContext.Clients.All.SendAsync("PostDeleted", id);

        return NoContent();
    }

    [HttpPost("{id}/like")]
    public async Task<ActionResult<object>> ToggleLike(string id, [FromQuery] string userId)
    {
        var isLiked = await _postService.ToggleLikeAsync(id, userId);
        var post = await _postService.GetPostByIdAsync(id, userId);

        if (post == null)
            return NotFound();

        // Broadcast like update to all connected clients
        await _feedHubContext.Clients.All.SendAsync("PostLikeUpdated", new
        {
            PostId = id,
            LikesCount = post.LikesCount,
            IsLiked = isLiked,
            UserId = userId
        });

        return Ok(new { isLiked, likesCount = post.LikesCount });
    }
}
