namespace ChatSystem.Domain.Interfaces;

public interface IMediaStorageService
{
    /// <summary>
    /// Uploads a file to Google Cloud Storage and returns the CDN URL
    /// </summary>
    /// <param name="fileName">Name of the file</param>
    /// <param name="contentType">MIME type of the file</param>
    /// <param name="fileStream">File content stream</param>
    /// <param name="userId">User ID for organizing files</param>
    /// <returns>CDN URL of the uploaded file</returns>
    Task<string> UploadFileAsync(string fileName, string contentType, Stream fileStream, string userId);

    /// <summary>
    /// Deletes a file from Google Cloud Storage
    /// </summary>
    /// <param name="fileUrl">CDN URL or file path to delete</param>
    Task DeleteFileAsync(string fileUrl);

    /// <summary>
    /// Gets a signed URL for temporary access to a private file
    /// </summary>
    /// <param name="fileName">Name of the file</param>
    /// <param name="expirationMinutes">Expiration time in minutes</param>
    /// <returns>Signed URL</returns>
    Task<string> GetSignedUrlAsync(string fileName, int expirationMinutes = 60);
}
