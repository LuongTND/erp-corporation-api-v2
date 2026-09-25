namespace Application;

public sealed class UpsertUserCustomFieldValuesCommandHandler(
    IUnitOfWork unitOfWork,
    ICustomFieldValidationService validationService)
    : IRequestHandler<UpsertUserCustomFieldValuesCommand, Unit>
{
    public async Task<Unit> Handle(UpsertUserCustomFieldValuesCommand cmd, CancellationToken ct)
    {
        var userExists = await unitOfWork.Repository<User>()
            .AnyAsync(u => u.Id == cmd.UserId, ct);
        if (!userExists)
            throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.UserId));

        var inputList = cmd.Values.ToList();
        var defIds = inputList.Select(v => v.DefinitionId).ToHashSet();

        var definitions = await unitOfWork.Repository<CustomFieldDefinition>()
            .GetAllAsync(d => defIds.Contains(d.Id) && d.IsActive, ct);
        var defMap = definitions.ToDictionary(d => d.Id);

        // Validate values against definition rules
        var failures = new List<FluentValidation.Results.ValidationFailure>();
        foreach (var input in inputList)
        {
            if (!defMap.TryGetValue(input.DefinitionId, out var def))
            {
                failures.Add(new(input.DefinitionId.ToString(), $"CustomFieldDefinition không tồn tại hoặc đã bị vô hiệu hóa."));
                continue;
            }

            var error = validationService.Validate(def, input.Value);
            if (error is not null)
                failures.Add(new(def.Code, error));
        }

        // Check required active fields
        var allRequired = await unitOfWork.Repository<CustomFieldDefinition>()
            .GetAllAsync(d => d.IsRequired && d.IsActive, ct);

        foreach (var required in allRequired)
        {
            var provided = inputList.FirstOrDefault(v => v.DefinitionId == required.Id);
            if (provided is null || string.IsNullOrWhiteSpace(provided.Value))
                failures.Add(new(required.Code, $"{required.Name} là bắt buộc."));
        }

        if (failures.Count > 0)
            throw new FluentValidation.ValidationException(failures);

        // Upsert — load existing tracked
        var existing = await unitOfWork.Repository<UserCustomFieldValue>()
            .GetAllTrackedAsync(v => v.UserId == cmd.UserId && defIds.Contains(v.DefinitionId), ct);
        var existingMap = existing.ToDictionary(v => v.DefinitionId);

        foreach (var input in inputList)
        {
            if (!defMap.ContainsKey(input.DefinitionId)) continue;

            if (existingMap.TryGetValue(input.DefinitionId, out var existing_value))
            {
                existing_value.Value = input.Value;
            }
            else
            {
                await unitOfWork.Repository<UserCustomFieldValue>().AddAsync(new UserCustomFieldValue
                {
                    Id = Guid.NewGuid(),
                    UserId = cmd.UserId,
                    DefinitionId = input.DefinitionId,
                    Value = input.Value,
                });
            }
        }

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
