namespace Application;

public sealed class ToggleDocumentVisibilityCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ToggleDocumentVisibilityCommand, Unit>
{
    public async Task<Unit> Handle(ToggleDocumentVisibilityCommand cmd, CancellationToken ct)
    {
        var doc = await unitOfWork.Repository<EmployeeDocument>()
            .FindTrackedAsync(d => d.Id == cmd.DocumentId && d.UserId == cmd.UserId, ct)
            ?? throw new NotFoundException($"Document {cmd.DocumentId} not found");

        doc.IsVisibleToEmployee = cmd.IsVisibleToEmployee;
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
