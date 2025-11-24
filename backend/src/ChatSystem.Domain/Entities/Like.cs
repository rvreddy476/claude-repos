namespace ChatSystem.Domain.Entities;

public class Like
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string? PostId { get; set; }
    public string? CommentId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
