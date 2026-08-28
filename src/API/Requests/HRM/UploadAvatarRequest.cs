namespace API;

public sealed class UploadAvatarRequest
{
    public IFormFile File { get; set; } = default!;
}
