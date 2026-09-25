namespace Application;

public sealed class CreateCustomFieldDefinitionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCustomFieldDefinitionCommand, Guid>
{
    public async Task<Guid> Handle(CreateCustomFieldDefinitionCommand cmd, CancellationToken ct)
    {
        var codeExists = await unitOfWork.Repository<CustomFieldDefinition>()
            .AnyAsync(d => d.Code == cmd.Code, ct);
        if (codeExists)
            throw new ConflictException(ExceptionMessages.AlreadyExists("CustomField Code", cmd.Code));

        var definitionId = Guid.NewGuid();
        var definition = new CustomFieldDefinition
        {
            Id = definitionId,
            Code = cmd.Code,
            Name = cmd.Name,
            FieldType = cmd.FieldType,
            Module = cmd.Module,
            IsSystem = false,
            IsRequired = cmd.IsRequired,
            IsActive = true,
            SortOrder = cmd.SortOrder,
            Placeholder = cmd.Placeholder,
            HelpText = cmd.HelpText,
            Group = cmd.Group,
            ValidationJson = cmd.ValidationJson,
        };

        await unitOfWork.Repository<CustomFieldDefinition>().AddAsync(definition);

        if (cmd.Options is not null)
        {
            foreach (var opt in cmd.Options)
            {
                await unitOfWork.Repository<CustomFieldOption>().AddAsync(new CustomFieldOption
                {
                    Id = Guid.NewGuid(),
                    DefinitionId = definitionId,
                    Value = opt.Value,
                    Label = opt.Label,
                    SortOrder = opt.SortOrder,
                    IsActive = true,
                });
            }
        }

        await unitOfWork.EnsureSaveAsync(ct);
        return definitionId;
    }
}
