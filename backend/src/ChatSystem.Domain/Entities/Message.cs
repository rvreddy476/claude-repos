namespace ChatSystem.Domain.Entities;

public class Message
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ChatRoomId { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public MessageType Type { get; set; } = MessageType.Text;
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
}

public enum MessageType
{
    Text,
    Image,
    File,
    System
}
