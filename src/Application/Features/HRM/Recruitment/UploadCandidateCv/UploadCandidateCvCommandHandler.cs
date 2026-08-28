namespace Application;

public sealed class UploadCandidateCvCommandHandler(IUnitOfWork unitOfWork, IBlobStorageService blobStorage)
    : IRequestHandler<UploadCandidateCvCommand, string>
{
    public async Task<string> Handle(UploadCandidateCvCommand cmd, CancellationToken ct)
    {
        var candidate = await unitOfWork.Repository<Candidate>()
            .FindAsync(c => c.Id == cmd.CandidateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Candidate", cmd.CandidateId));

        var blobName = $"{cmd.CandidateId}/{cmd.FileName}";
        await blobStorage.UploadAsync("candidates-cv", blobName, cmd.FileStream, "application/octet-stream", ct: ct);
        var url = blobStorage.GetUrl("candidates-cv", blobName);

        candidate.CvUrl = url;
        await unitOfWork.EnsureSaveAsync(ct);
        return url;
    }
}
