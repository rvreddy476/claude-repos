namespace ChatSystem.Application.DTOs;

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
