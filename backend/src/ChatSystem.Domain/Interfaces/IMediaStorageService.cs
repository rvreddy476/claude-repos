namespace ChatSystem.Domain.Interfaces;

public interface IMediaStorageService
{
    /// <summary>
    /// Generates a presigned URL for direct upload from client to Google Cloud Storage
    /// </summary>
    /// <param name="fileName">Original file name</param>
    /// <param name="contentType">MIME type of the file</param>
    /// <param name="userId">User ID for organizing files</param>
    /// <param name="expirationMinutes">Expiration time in minutes</param>
    /// <returns>Tuple containing (presignedUploadUrl, objectName, finalCdnUrl)</returns>
    Task<(string presignedUploadUrl, string objectName, string finalCdnUrl)> GeneratePresignedUploadUrlAsync(
        string fileName,
        string contentType,
        string userId,
        int expirationMinutes = 15);

    /// <summary>
    /// Deletes a file from Google Cloud Storage
    /// </summary>
    /// <param name="fileUrl">CDN URL or file path to delete</param>
    Task DeleteFileAsync(string fileUrl);

    /// <summary>
    /// Gets a signed URL for temporary access to a private file (download)
    /// </summary>
    /// <param name="fileName">Name of the file</param>
    /// <param name="expirationMinutes">Expiration time in minutes</param>
    /// <returns>Signed URL</returns>
    Task<string> GetSignedDownloadUrlAsync(string fileName, int expirationMinutes = 60);
}
