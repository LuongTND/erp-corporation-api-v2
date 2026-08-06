namespace Application;

public sealed class UpdateCustomFieldDefinitionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCustomFieldDefinitionCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCustomFieldDefinitionCommand cmd, CancellationToken ct)
    {
        var definition = await unitOfWork.Repository<CustomFieldDefinition>()
            .FindTrackedAsync(d => d.Id == cmd.DefinitionId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("CustomFieldDefinition", cmd.DefinitionId));

        if (definition.IsSystem)
            throw new BadRequestException("Không thể chỉnh sửa trường hệ thống.");

        definition.Name = cmd.Name;
        definition.IsRequired = cmd.IsRequired;
        definition.IsActive = cmd.IsActive;
        definition.SortOrder = cmd.SortOrder;
        definition.Placeholder = cmd.Placeholder;
        definition.HelpText = cmd.HelpText;
        definition.Group = cmd.Group;
        definition.ValidationJson = cmd.ValidationJson;

        if (cmd.Options is not null &&
            definition.FieldType is CustomFieldType.Select or CustomFieldType.MultiSelect)
        {
            await SyncOptionsAsync(definition.Id, cmd.Options, ct);
        }

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }

    private async Task SyncOptionsAsync(Guid definitionId, IEnumerable<UpsertCustomFieldOptionDto> incoming, CancellationToken ct)
    {
        var existing = await unitOfWork.Repository<CustomFieldOption>()
            .GetAllTrackedAsync(o => o.DefinitionId == definitionId, ct);

        var existingMap = existing.ToDictionary(o => o.Id);
        var incomingList = incoming.ToList();
        var incomingIds = incomingList.Where(o => o.Id.HasValue).Select(o => o.Id!.Value).ToHashSet();

        // Remove options not in incoming (skip IsSystem options — CustomFieldOption has no IsSystem, remove all not in list)
        foreach (var old in existing.Where(o => !incomingIds.Contains(o.Id)))
            await unitOfWork.Repository<CustomFieldOption>().RemoveAsync(old);

        foreach (var dto in incomingList)
        {
            if (dto.Id.HasValue && existingMap.TryGetValue(dto.Id.Value, out var opt))
            {
                opt.Value = dto.Value;
                opt.Label = dto.Label;
                opt.SortOrder = dto.SortOrder;
                opt.IsActive = dto.IsActive;
            }
            else
            {
                await unitOfWork.Repository<CustomFieldOption>().AddAsync(new CustomFieldOption
                {
                    Id = Guid.NewGuid(),
                    DefinitionId = definitionId,
                    Value = dto.Value,
                    Label = dto.Label,
                    SortOrder = dto.SortOrder,
                    IsActive = dto.IsActive,
                });
            }
        }
    }
}
