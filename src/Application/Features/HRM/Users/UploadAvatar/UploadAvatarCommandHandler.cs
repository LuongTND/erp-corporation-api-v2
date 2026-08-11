namespace Application;

public sealed class UploadAvatarCommandHandler(IUnitOfWork unitOfWork, IBlobStorageService blobStorage)
    : IRequestHandler<UploadAvatarCommand, string>
{
    private const string Container = "avatars";

    private static string GetExtension(string contentType) => contentType.ToLowerInvariant() switch
    {
        "image/png"  => ".png",
        "image/webp" => ".webp",
        "image/gif"  => ".gif",
        _            => ".jpg",
    };

    public async Task<string> Handle(UploadAvatarCommand cmd, CancellationToken ct)
    {
        var user = await unitOfWork.Repository<User>().FindTrackedAsync(u => u.Id == cmd.UserId, ct)
            ?? throw new NotFoundException($"User {cmd.UserId} not found");

        var blobName = $"{cmd.UserId}{GetExtension(cmd.ContentType)}";
        await blobStorage.UploadAsync(Container, blobName, cmd.FileStream, cmd.ContentType, ct: ct);

        user.AvatarUrl = blobName;
        await unitOfWork.SaveChangesAsync(ct);

        return blobStorage.GetUrl(Container, blobName);
    }
}
