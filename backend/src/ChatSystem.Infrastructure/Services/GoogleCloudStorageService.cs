using ChatSystem.Domain.Interfaces;
using ChatSystem.Infrastructure.Configuration;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace ChatSystem.Infrastructure.Services;

public class GoogleCloudStorageService : IMediaStorageService
{
    private readonly GoogleCloudStorageSettings _settings;
    private readonly StorageClient _storageClient;
    private readonly UrlSigner _urlSigner;

    public GoogleCloudStorageService(IOptions<GoogleCloudStorageSettings> settings)
    {
        _settings = settings.Value;

        // Initialize Storage Client with credentials
        if (!string.IsNullOrEmpty(_settings.CredentialsPath) && File.Exists(_settings.CredentialsPath))
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", _settings.CredentialsPath);
            _storageClient = StorageClient.Create();
            _urlSigner = UrlSigner.FromServiceAccountPath(_settings.CredentialsPath);
        }
        else
        {
            // Fall back to default credentials (for environments where credentials are already configured)
            _storageClient = StorageClient.Create();
            _urlSigner = UrlSigner.FromServiceAccountCredential(Google.Apis.Auth.OAuth2.GoogleCredential.GetApplicationDefault().UnderlyingCredential as Google.Apis.Auth.OAuth2.ServiceAccountCredential);
        }
    }

    public async Task<(string presignedUploadUrl, string objectName, string finalCdnUrl)> GeneratePresignedUploadUrlAsync(
        string fileName,
        string contentType,
        string userId,
        int expirationMinutes = 15)
    {
        try
        {
            // Create a unique file name to avoid collisions
            var fileExtension = Path.GetExtension(fileName);
            var objectName = $"{userId}/{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid()}{fileExtension}";

            Console.WriteLine($"🔑 Generating presigned upload URL for: {objectName}");
            Console.WriteLine($"📦 Bucket: {_settings.BucketName}");
            Console.WriteLine($"📄 Content Type: {contentType}");

            // Generate presigned URL for PUT request
            var presignedUrl = await _urlSigner.SignAsync(
                bucket: _settings.BucketName,
                objectName: objectName,
                duration: TimeSpan.FromMinutes(expirationMinutes),
                httpMethod: HttpMethod.Put,
                signingVersion: SigningVersion.V4,
                contentHeaders: new Dictionary<string, IEnumerable<string>>
                {
                    { "Content-Type", new[] { contentType } }
                });

            // Generate the final CDN URL
            string finalCdnUrl;
            if (!string.IsNullOrEmpty(_settings.MediaCdnUrl))
            {
                finalCdnUrl = $"{_settings.MediaCdnUrl.TrimEnd('/')}/{objectName}";
                Console.WriteLine($"🌐 Final CDN URL: {finalCdnUrl}");
            }
            else
            {
                finalCdnUrl = $"https://storage.googleapis.com/{_settings.BucketName}/{objectName}";
                Console.WriteLine($"🌐 Final Public URL: {finalCdnUrl}");
            }

            Console.WriteLine($"✅ Presigned upload URL generated successfully");

            return (presignedUrl, objectName, finalCdnUrl);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error generating presigned upload URL: {ex.Message}");
            throw new Exception($"Failed to generate presigned upload URL: {ex.Message}", ex);
        }
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        try
        {
            // Extract object name from URL
            var objectName = ExtractObjectNameFromUrl(fileUrl);

            if (string.IsNullOrEmpty(objectName))
            {
                Console.WriteLine($"⚠️ Could not extract object name from URL: {fileUrl}");
                return;
            }

            Console.WriteLine($"🗑️ Deleting file from GCS: {objectName}");

            await _storageClient.DeleteObjectAsync(_settings.BucketName, objectName);

            Console.WriteLine($"✅ File deleted successfully: {objectName}");
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.WriteLine($"⚠️ File not found: {fileUrl}");
            // File doesn't exist, no need to throw an error
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error deleting file: {ex.Message}");
            throw new Exception($"Failed to delete file from Google Cloud Storage: {ex.Message}", ex);
        }
    }

    public async Task<string> GetSignedDownloadUrlAsync(string fileName, int expirationMinutes = 60)
    {
        try
        {
            var signedUrl = await _urlSigner.SignAsync(
                bucket: _settings.BucketName,
                objectName: fileName,
                duration: TimeSpan.FromMinutes(expirationMinutes),
                httpMethod: HttpMethod.Get,
                signingVersion: SigningVersion.V4
            );

            return signedUrl;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error generating signed download URL: {ex.Message}");
            throw new Exception($"Failed to generate signed download URL: {ex.Message}", ex);
        }
    }

    private string? ExtractObjectNameFromUrl(string fileUrl)
    {
        try
        {
            // Handle CDN URLs
            if (!string.IsNullOrEmpty(_settings.MediaCdnUrl) && fileUrl.StartsWith(_settings.MediaCdnUrl, StringComparison.OrdinalIgnoreCase))
            {
                return fileUrl.Substring(_settings.MediaCdnUrl.Length).TrimStart('/');
            }

            // Handle direct GCS URLs
            var gcsPrefix = $"https://storage.googleapis.com/{_settings.BucketName}/";
            if (fileUrl.StartsWith(gcsPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return fileUrl.Substring(gcsPrefix.Length);
            }

            // If URL doesn't match known patterns, return null
            return null;
        }
        catch
        {
            return null;
        }
    }
}
