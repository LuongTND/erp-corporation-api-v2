namespace Application;

public sealed class UploadCandidateCvCommandHandler(IUnitOfWork unitOfWork, IBlobStorageService blobStorage)
    : IRequestHandler<UploadCandidateCvCommand, string>
{
    public async Task<string> Handle(UploadCandidateCvCommand cmd, CancellationToken ct)
    {
        var applicant = await unitOfWork.Repository<Applicant>()
            .FindAsync(a => a.Id == cmd.ApplicantId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Applicant", cmd.ApplicantId));

        var blobName = $"{applicant.Id}/{cmd.FileName}";
        await blobStorage.UploadAsync("applicants-cv", blobName, cmd.FileStream, "application/octet-stream", ct: ct);
        var url = blobStorage.GetUrl("applicants-cv", blobName);

        var doc = new ApplicantDocument
        {
            Id = Guid.NewGuid(),
            ApplicantId = applicant.Id,
            DocumentType = ApplicantDocumentType.Cv,
            FileName = cmd.FileName,
            FileUrl = url
        };
        await unitOfWork.Repository<ApplicantDocument>().AddAsync(doc);
        await unitOfWork.EnsureSaveAsync(ct);
        return url;
    }
}
