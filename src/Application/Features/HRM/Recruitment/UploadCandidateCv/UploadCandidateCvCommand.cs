namespace Application;

public sealed record UploadCandidateCvCommand(Guid ApplicantId, Stream FileStream, string FileName) : IRequest<string>;
