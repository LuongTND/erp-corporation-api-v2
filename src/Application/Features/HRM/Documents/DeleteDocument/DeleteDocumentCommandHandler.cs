namespace Application;

public sealed class DeleteDocumentCommandHandler(IUnitOfWork unitOfWork, IBlobStorageService blobStorage, IUserContext currentUser)
    : IRequestHandler<DeleteDocumentCommand, Unit>
{
    private const string Container = "employee-documents";

    public async Task<Unit> Handle(DeleteDocumentCommand cmd, CancellationToken ct)
    {
        var doc = await unitOfWork.Repository<EmployeeDocument>()
            .FindTrackedAsync(d => d.Id == cmd.DocumentId && d.UserId == cmd.UserId, ct)
            ?? throw new NotFoundException($"Document {cmd.DocumentId} not found");

        // Employee chỉ xóa doc do mình upload; HR (cmd.IsHrDelete) xóa được tất cả
        if (!cmd.IsHrDelete && doc.CreatedBy != currentUser.UserId)
            throw new ForbiddenException("Không thể xóa tài liệu do người khác upload");

        await blobStorage.DeleteAsync(Container, doc.BlobName, ct);
        await unitOfWork.Repository<EmployeeDocument>().RemoveAsync(doc);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
