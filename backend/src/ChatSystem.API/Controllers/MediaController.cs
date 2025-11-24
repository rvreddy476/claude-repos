using ChatSystem.Application.DTOs;
using ChatSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediaStorageService _mediaStorageService;
    private const long MaxFileSize = 100 * 1024 * 1024; // 100 MB
    private static readonly string[] AllowedImageTypes = { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
    private static readonly string[] AllowedVideoTypes = { "video/mp4", "video/webm", "video/ogg", "video/quicktime" };
    private static readonly string[] AllowedAudioTypes = { "audio/mpeg", "audio/mp3", "audio/wav", "audio/ogg", "audio/webm" };

    public MediaController(IMediaStorageService mediaStorageService)
    {
        _mediaStorageService = mediaStorageService;
    }

    [HttpPost("presigned-url")]
    public async Task<ActionResult<PresignedUploadUrlDto>> GetPresignedUploadUrl(
        [FromQuery] string userId,
        [FromQuery] string fileName,
        [FromQuery] string contentType,
        [FromQuery] long fileSize)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new MediaUploadErrorDto("INVALID_USER", "User ID is required"));
            }

            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest(new MediaUploadErrorDto("INVALID_FILE_NAME", "File name is required"));
            }

            if (string.IsNullOrEmpty(contentType))
            {
                return BadRequest(new MediaUploadErrorDto("INVALID_CONTENT_TYPE", "Content type is required"));
            }

            // Validate file size
            if (fileSize <= 0 || fileSize > MaxFileSize)
            {
                return BadRequest(new MediaUploadErrorDto("FILE_TOO_LARGE", $"File size must be between 1 byte and {MaxFileSize / 1024 / 1024} MB"));
            }

            // Validate content type
            if (!IsAllowedContentType(contentType))
            {
                return BadRequest(new MediaUploadErrorDto("INVALID_FILE_TYPE", $"File type '{contentType}' is not allowed"));
            }

            Console.WriteLine($"🔑 Generating presigned URL for user: {userId}");
            Console.WriteLine($"   File Name: {fileName}");
            Console.WriteLine($"   Content Type: {contentType}");
            Console.WriteLine($"   Size: {fileSize / 1024.0:F2} KB");

            // Generate presigned URL
            var (presignedUploadUrl, objectName, finalCdnUrl) = await _mediaStorageService.GeneratePresignedUploadUrlAsync(
                fileName: fileName,
                contentType: contentType,
                userId: userId,
                expirationMinutes: 15
            );

            var response = new PresignedUploadUrlDto(
                PresignedUrl: presignedUploadUrl,
                ObjectName: objectName,
                FinalUrl: finalCdnUrl,
                ExpiresInMinutes: 15
            );

            Console.WriteLine($"✅ Presigned URL generated successfully");

            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error generating presigned URL: {ex.Message}");
            return StatusCode(500, new MediaUploadErrorDto("PRESIGNED_URL_GENERATION_FAILED", ex.Message));
        }
    }

    [HttpPost("presigned-urls")]
    public async Task<ActionResult<List<PresignedUploadUrlDto>>> GetMultiplePresignedUploadUrls(
        [FromQuery] string userId,
        [FromBody] List<FileUploadRequest> files)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new MediaUploadErrorDto("INVALID_USER", "User ID is required"));
            }

            if (files == null || files.Count == 0)
            {
                return BadRequest(new MediaUploadErrorDto("INVALID_FILES", "At least one file is required"));
            }

            if (files.Count > 10)
            {
                return BadRequest(new MediaUploadErrorDto("TOO_MANY_FILES", "Maximum 10 files allowed per request"));
            }

            var responses = new List<PresignedUploadUrlDto>();

            foreach (var file in files)
            {
                // Validate each file
                if (file.FileSize <= 0 || file.FileSize > MaxFileSize)
                {
                    return BadRequest(new MediaUploadErrorDto("FILE_TOO_LARGE", $"File '{file.FileName}' exceeds maximum size"));
                }

                if (!IsAllowedContentType(file.ContentType))
                {
                    return BadRequest(new MediaUploadErrorDto("INVALID_FILE_TYPE", $"File type '{file.ContentType}' is not allowed for '{file.FileName}'"));
                }

                var (presignedUploadUrl, objectName, finalCdnUrl) = await _mediaStorageService.GeneratePresignedUploadUrlAsync(
                    fileName: file.FileName,
                    contentType: file.ContentType,
                    userId: userId,
                    expirationMinutes: 15
                );

                responses.Add(new PresignedUploadUrlDto(
                    PresignedUrl: presignedUploadUrl,
                    ObjectName: objectName,
                    FinalUrl: finalCdnUrl,
                    ExpiresInMinutes: 15
                ));
            }

            Console.WriteLine($"✅ Generated {responses.Count} presigned URLs successfully");

            return Ok(responses);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error generating presigned URLs: {ex.Message}");
            return StatusCode(500, new MediaUploadErrorDto("PRESIGNED_URL_GENERATION_FAILED", ex.Message));
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteMedia([FromQuery] string fileUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                return BadRequest(new MediaUploadErrorDto("INVALID_URL", "File URL is required"));
            }

            await _mediaStorageService.DeleteFileAsync(fileUrl);

            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error deleting media: {ex.Message}");
            return StatusCode(500, new MediaUploadErrorDto("DELETE_FAILED", ex.Message));
        }
    }

    private bool IsAllowedContentType(string contentType)
    {
        return AllowedImageTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase) ||
               AllowedVideoTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase) ||
               AllowedAudioTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase);
    }
}

public record FileUploadRequest(string FileName, string ContentType, long FileSize);
