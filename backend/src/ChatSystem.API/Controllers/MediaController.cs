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

    [HttpPost("upload")]
    [RequestSizeLimit(100 * 1024 * 1024)] // 100 MB
    public async Task<ActionResult<MediaUploadResponseDto>> UploadMedia([FromForm] IFormFile file, [FromQuery] string userId)
    {
        try
        {
            // Validate file
            if (file == null || file.Length == 0)
            {
                return BadRequest(new MediaUploadErrorDto("INVALID_FILE", "No file provided"));
            }

            if (file.Length > MaxFileSize)
            {
                return BadRequest(new MediaUploadErrorDto("FILE_TOO_LARGE", $"File size exceeds maximum allowed size of {MaxFileSize / 1024 / 1024} MB"));
            }

            // Validate content type
            if (!IsAllowedContentType(file.ContentType))
            {
                return BadRequest(new MediaUploadErrorDto("INVALID_FILE_TYPE", $"File type '{file.ContentType}' is not allowed"));
            }

            // Validate user ID
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new MediaUploadErrorDto("INVALID_USER", "User ID is required"));
            }

            Console.WriteLine($"📤 Received file upload request:");
            Console.WriteLine($"   File Name: {file.FileName}");
            Console.WriteLine($"   Content Type: {file.ContentType}");
            Console.WriteLine($"   Size: {file.Length / 1024.0:F2} KB");
            Console.WriteLine($"   User ID: {userId}");

            // Upload to Google Cloud Storage
            using (var stream = file.OpenReadStream())
            {
                var url = await _mediaStorageService.UploadFileAsync(
                    fileName: file.FileName,
                    contentType: file.ContentType,
                    fileStream: stream,
                    userId: userId
                );

                var response = new MediaUploadResponseDto(
                    Url: url,
                    FileName: file.FileName,
                    ContentType: file.ContentType,
                    Size: file.Length
                );

                Console.WriteLine($"✅ File uploaded successfully: {url}");

                return Ok(response);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error uploading media: {ex.Message}");
            return StatusCode(500, new MediaUploadErrorDto("UPLOAD_FAILED", ex.Message));
        }
    }

    [HttpPost("upload-multiple")]
    [RequestSizeLimit(500 * 1024 * 1024)] // 500 MB for multiple files
    public async Task<ActionResult<List<MediaUploadResponseDto>>> UploadMultipleMedia([FromForm] List<IFormFile> files, [FromQuery] string userId)
    {
        try
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest(new MediaUploadErrorDto("INVALID_FILES", "No files provided"));
            }

            if (files.Count > 10)
            {
                return BadRequest(new MediaUploadErrorDto("TOO_MANY_FILES", "Maximum 10 files allowed per upload"));
            }

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new MediaUploadErrorDto("INVALID_USER", "User ID is required"));
            }

            var responses = new List<MediaUploadResponseDto>();

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                if (file.Length > MaxFileSize)
                {
                    return BadRequest(new MediaUploadErrorDto("FILE_TOO_LARGE", $"File '{file.FileName}' exceeds maximum size"));
                }

                if (!IsAllowedContentType(file.ContentType))
                {
                    return BadRequest(new MediaUploadErrorDto("INVALID_FILE_TYPE", $"File type '{file.ContentType}' is not allowed for '{file.FileName}'"));
                }

                using (var stream = file.OpenReadStream())
                {
                    var url = await _mediaStorageService.UploadFileAsync(
                        fileName: file.FileName,
                        contentType: file.ContentType,
                        fileStream: stream,
                        userId: userId
                    );

                    responses.Add(new MediaUploadResponseDto(
                        Url: url,
                        FileName: file.FileName,
                        ContentType: file.ContentType,
                        Size: file.Length
                    ));
                }
            }

            Console.WriteLine($"✅ {responses.Count} files uploaded successfully");

            return Ok(responses);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error uploading multiple media: {ex.Message}");
            return StatusCode(500, new MediaUploadErrorDto("UPLOAD_FAILED", ex.Message));
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
