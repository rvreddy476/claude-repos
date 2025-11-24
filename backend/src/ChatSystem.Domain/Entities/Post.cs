namespace ChatSystem.Domain.Entities;

public class Post
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public PostType Type { get; set; } = PostType.Text;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public int SharesCount { get; set; }
}

public enum PostType
{
    Text,
    Image,
    Video,
    Mixed
}
