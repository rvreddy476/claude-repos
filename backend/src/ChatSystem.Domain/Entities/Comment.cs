namespace ChatSystem.Domain.Entities;

public class Comment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PostId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int LikesCount { get; set; }
}
