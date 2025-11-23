namespace ChatSystem.Infrastructure.Configuration;

public class RedisSettings
{
    public string ConnectionString { get; set; } = "localhost:6379";
    public int DefaultExpirationMinutes { get; set; } = 30;
}
