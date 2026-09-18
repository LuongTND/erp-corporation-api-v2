namespace Application;

public sealed class CreateWorkflowTemplateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateWorkflowTemplateCommand, Guid>
{
    public async Task<Guid> Handle(CreateWorkflowTemplateCommand cmd, CancellationToken ct)
    {
        var exists = await unitOfWork.Repository<WorkflowTemplate>()
            .AnyAsync(t => t.EntityType == cmd.EntityType && t.ScopeType == cmd.ScopeType && t.ScopeEntityId == cmd.ScopeEntityId, ct);

        if (exists)
            throw new ConflictException($"Đã tồn tại workflow template cho '{cmd.EntityType}' với scope này.");

        var template = new WorkflowTemplate
        {
            Id = Guid.NewGuid(),
            Name = cmd.Name,
            EntityType = cmd.EntityType,
            ScopeType = cmd.ScopeType,
            ScopeEntityId = cmd.ScopeEntityId,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        await unitOfWork.Repository<WorkflowTemplate>().AddAsync(template);
        await unitOfWork.EnsureSaveAsync(ct);
        return template.Id;
    }
}
