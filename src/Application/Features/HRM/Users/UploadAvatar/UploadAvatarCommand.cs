namespace Application;

public sealed record UploadAvatarCommand(Guid UserId, Stream FileStream, string ContentType, string FileName)
    : IRequest<string>;
