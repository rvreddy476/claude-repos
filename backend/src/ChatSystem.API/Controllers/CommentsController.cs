using ChatSystem.Application.DTOs;
using ChatSystem.Application.Services;
using ChatSystem.API.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly IHubContext<FeedHub> _feedHubContext;

    public CommentsController(ICommentService commentService, IHubContext<FeedHub> feedHubContext)
    {
        _commentService = commentService;
        _feedHubContext = feedHubContext;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommentDto>> GetComment(string id, [FromQuery] string? userId)
    {
        var comment = await _commentService.GetCommentByIdAsync(id, userId);
        if (comment == null)
            return NotFound();

        return Ok(comment);
    }

    [HttpGet("post/{postId}")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetPostComments(
        string postId,
        [FromQuery] string? userId,
        [FromQuery] int skip = 0,
        [FromQuery] int limit = 3)
    {
        var comments = await _commentService.GetPostCommentsAsync(postId, userId, skip, limit);
        return Ok(comments);
    }

    [HttpGet("post/{postId}/count")]
    public async Task<ActionResult<int>> GetPostCommentsCount(string postId)
    {
        var count = await _commentService.GetPostCommentsCountAsync(postId);
        return Ok(count);
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> CreateComment([FromQuery] string userId, [FromBody] CreateCommentDto createCommentDto)
    {
        var comment = await _commentService.CreateCommentAsync(userId, createCommentDto);

        // Broadcast the new comment to all connected clients
        await _feedHubContext.Clients.All.SendAsync("NewCommentCreated", comment);

        return CreatedAtAction(nameof(GetComment), new { id = comment.Id, userId }, comment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(string id, [FromBody] UpdateCommentDto updateCommentDto)
    {
        try
        {
            await _commentService.UpdateCommentAsync(id, updateCommentDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(string id)
    {
        try
        {
            await _commentService.DeleteCommentAsync(id);

            // Broadcast comment deletion
            await _feedHubContext.Clients.All.SendAsync("CommentDeleted", id);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/like")]
    public async Task<ActionResult<object>> ToggleLike(string id, [FromQuery] string userId)
    {
        var isLiked = await _commentService.ToggleLikeAsync(id, userId);
        var comment = await _commentService.GetCommentByIdAsync(id, userId);

        if (comment == null)
            return NotFound();

        // Broadcast like update to all connected clients
        await _feedHubContext.Clients.All.SendAsync("CommentLikeUpdated", new
        {
            CommentId = id,
            LikesCount = comment.LikesCount,
            IsLiked = isLiked,
            UserId = userId
        });

        return Ok(new { isLiked, likesCount = comment.LikesCount });
    }
}
