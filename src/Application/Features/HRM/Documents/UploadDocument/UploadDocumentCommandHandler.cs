namespace Application;

public sealed class UploadDocumentCommandHandler(IUnitOfWork unitOfWork, IBlobStorageService blobStorage, IUserContext currentUser)
    : IRequestHandler<UploadDocumentCommand, EmployeeDocumentResponse>
{
    private const string Container = "employee-documents";

    public async Task<EmployeeDocumentResponse> Handle(UploadDocumentCommand cmd, CancellationToken ct)
    {
        var userExists = await unitOfWork.Repository<User>().FindAsync(u => u.Id == cmd.UserId, ct)
            ?? throw new NotFoundException($"User {cmd.UserId} not found");

        var ext = Path.GetExtension(cmd.OriginalFileName);
        var blobName = $"{cmd.UserId}/{Guid.NewGuid()}{ext}";

        await blobStorage.UploadAsync(Container, blobName, cmd.FileStream, cmd.ContentType, ct: ct);

        var doc = new EmployeeDocument
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            Category = cmd.Category,
            CustomName = cmd.Category == DocumentCategory.Other ? cmd.CustomName : null,
            BlobName = blobName,
            OriginalFileName = cmd.OriginalFileName,
            ContentType = cmd.ContentType,
            FileSizeBytes = cmd.FileSizeBytes,
            IssuedDate = cmd.IssuedDate,
            ExpiryDate = cmd.ExpiryDate,
            Notes = cmd.Notes,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = currentUser.UserId,
        };

        await unitOfWork.Repository<EmployeeDocument>().AddAsync(doc);
        await unitOfWork.SaveChangesAsync(ct);

        var now = DateTimeOffset.UtcNow;
        var warnWindow = TimeSpan.FromDays(30);
        return new EmployeeDocumentResponse
        {
            Id = doc.Id,
            Category = doc.Category.ToString(),
            CustomName = doc.CustomName,
            DisplayName = doc.Category == DocumentCategory.Other ? (doc.CustomName ?? "Khác") : cmd.OriginalFileName,
            OriginalFileName = doc.OriginalFileName,
            ContentType = doc.ContentType,
            FileSizeBytes = doc.FileSizeBytes,
            FileUrl = blobStorage.GetUrl(Container, blobName),
            IssuedDate = doc.IssuedDate,
            ExpiryDate = doc.ExpiryDate,
            Notes = doc.Notes,
            CreatedAt = doc.CreatedAt,
            IsExpired = doc.ExpiryDate.HasValue && doc.ExpiryDate.Value < now,
            IsExpiringSoon = doc.ExpiryDate.HasValue && doc.ExpiryDate.Value >= now && doc.ExpiryDate.Value < now.Add(warnWindow),
        };
    }
}
