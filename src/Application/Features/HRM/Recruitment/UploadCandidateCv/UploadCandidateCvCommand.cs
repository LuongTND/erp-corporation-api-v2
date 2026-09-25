namespace Application;

public sealed record UploadCandidateCvCommand(Guid CandidateId, Stream FileStream, string FileName) : IRequest<string>;
