namespace ChatSystem.Infrastructure.Configuration;

public class GoogleCloudStorageSettings
{
    public string ProjectId { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string CredentialsPath { get; set; } = string.Empty;
    public string MediaCdnUrl { get; set; } = string.Empty;
}
