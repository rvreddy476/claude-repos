namespace ChatSystem.Application.DTOs;

public record PresignedUploadUrlDto(
    string PresignedUrl,
    string ObjectName,
    string FinalUrl,
    int ExpiresInMinutes
);

public record MediaUploadResponseDto(
    string Url,
    string FileName,
    string ContentType,
    long Size
);

public record MediaUploadErrorDto(
    string Error,
    string Message
);
