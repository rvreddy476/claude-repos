using ChatSystem.Domain.Interfaces;
using ChatSystem.Infrastructure.Configuration;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace ChatSystem.Infrastructure.Services;

public class GoogleCloudStorageService : IMediaStorageService
{
    private readonly GoogleCloudStorageSettings _settings;
    private readonly StorageClient _storageClient;

    public GoogleCloudStorageService(IOptions<GoogleCloudStorageSettings> settings)
    {
        _settings = settings.Value;

        // Initialize Storage Client with credentials
        if (!string.IsNullOrEmpty(_settings.CredentialsPath) && File.Exists(_settings.CredentialsPath))
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", _settings.CredentialsPath);
            _storageClient = StorageClient.Create();
        }
        else
        {
            // Fall back to default credentials (for environments where credentials are already configured)
            _storageClient = StorageClient.Create();
        }
    }

    public async Task<string> UploadFileAsync(string fileName, string contentType, Stream fileStream, string userId)
    {
        try
        {
            // Create a unique file name to avoid collisions
            var fileExtension = Path.GetExtension(fileName);
            var uniqueFileName = $"{userId}/{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid()}{fileExtension}";

            Console.WriteLine($"📤 Uploading file to GCS: {uniqueFileName}");
            Console.WriteLine($"📦 Bucket: {_settings.BucketName}");
            Console.WriteLine($"📄 Content Type: {contentType}");

            // Upload to Google Cloud Storage
            var uploadedObject = await _storageClient.UploadObjectAsync(
                bucket: _settings.BucketName,
                objectName: uniqueFileName,
                contentType: contentType,
                source: fileStream,
                options: new UploadObjectOptions
                {
                    PredefinedAcl = PredefinedObjectAcl.PublicRead // Make file publicly accessible
                }
            );

            Console.WriteLine($"✅ File uploaded successfully: {uploadedObject.Name}");

            // Return CDN URL if configured, otherwise return GCS public URL
            if (!string.IsNullOrEmpty(_settings.MediaCdnUrl))
            {
                var cdnUrl = $"{_settings.MediaCdnUrl.TrimEnd('/')}/{uniqueFileName}";
                Console.WriteLine($"🌐 CDN URL: {cdnUrl}");
                return cdnUrl;
            }
            else
            {
                var publicUrl = $"https://storage.googleapis.com/{_settings.BucketName}/{uniqueFileName}";
                Console.WriteLine($"🌐 Public URL: {publicUrl}");
                return publicUrl;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error uploading file: {ex.Message}");
            throw new Exception($"Failed to upload file to Google Cloud Storage: {ex.Message}", ex);
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

    public async Task<string> GetSignedUrlAsync(string fileName, int expirationMinutes = 60)
    {
        try
        {
          
            var urlSigner = UrlSigner.FromCredentialFile(_settings.CredentialsPath);
            var signedUrl = urlSigner.Sign(
                bucket: _settings.BucketName,
                objectName: fileName,
                TimeSpan.FromMinutes(30),
                 HttpMethod.Put
            );

            return signedUrl;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error generating signed URL: {ex.Message}");
            throw new Exception($"Failed to generate signed URL: {ex.Message}", ex);
        }
    }

    private string? ExtractObjectNameFromUrl(string fileUrl)
    {
        try
        {
            // Handle CDN URLs
            if (fileUrl.StartsWith(_settings.MediaCdnUrl, StringComparison.OrdinalIgnoreCase))
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
