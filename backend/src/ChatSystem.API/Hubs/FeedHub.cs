using ChatSystem.Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatSystem.API.Hubs;

public class FeedHub : Hub
{
    private readonly IPostService _postService;
    private readonly ICommentService _commentService;

    public FeedHub(IPostService postService, ICommentService commentService)
    {
        _postService = postService;
        _commentService = commentService;
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"🔌 FeedHub client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"🔌 FeedHub client disconnected: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }

    // Broadcast new post to all connected clients
    public async Task BroadcastNewPost(string postId, string userId)
    {
        Console.WriteLine($"📢 Broadcasting new post: {postId} from user: {userId}");
        var post = await _postService.GetPostByIdAsync(postId, userId);

        if (post != null)
        {
            await Clients.All.SendAsync("NewPostCreated", post);
            Console.WriteLine($"✅ Broadcasted post to all clients");
        }
    }

    // Broadcast new comment to all connected clients
    public async Task BroadcastNewComment(string commentId, string postId, string userId)
    {
        Console.WriteLine($"📢 Broadcasting new comment: {commentId} for post: {postId}");
        var comment = await _commentService.GetCommentByIdAsync(commentId, userId);

        if (comment != null)
        {
            await Clients.All.SendAsync("NewCommentCreated", comment);
            Console.WriteLine($"✅ Broadcasted comment to all clients");
        }
    }

    // Broadcast like update to all connected clients
    public async Task BroadcastLikeUpdate(string postId, int likesCount, bool isLiked, string userId)
    {
        Console.WriteLine($"📢 Broadcasting like update for post: {postId}, likes: {likesCount}");
        await Clients.All.SendAsync("PostLikeUpdated", new
        {
            PostId = postId,
            LikesCount = likesCount,
            IsLiked = isLiked,
            UserId = userId
        });
    }

    // Broadcast comment like update
    public async Task BroadcastCommentLikeUpdate(string commentId, int likesCount, bool isLiked, string userId)
    {
        Console.WriteLine($"📢 Broadcasting comment like update: {commentId}, likes: {likesCount}");
        await Clients.All.SendAsync("CommentLikeUpdated", new
        {
            CommentId = commentId,
            LikesCount = likesCount,
            IsLiked = isLiked,
            UserId = userId
        });
    }
}
