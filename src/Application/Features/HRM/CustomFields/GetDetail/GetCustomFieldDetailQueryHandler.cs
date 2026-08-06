namespace Application;

public sealed class GetCustomFieldDetailQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetCustomFieldDetailQuery, CustomFieldDefinitionResponse>
{
    public async Task<CustomFieldDefinitionResponse> Handle(GetCustomFieldDetailQuery query, CancellationToken ct)
    {
        var definition = await unitOfWork.Repository<CustomFieldDefinition>()
            .FindAsync(d => d.Id == query.Id, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("CustomFieldDefinition", query.Id));

        var options = await unitOfWork.Repository<CustomFieldOption>()
            .GetAllAsync(o => o.DefinitionId == query.Id, ct);

        return new CustomFieldDefinitionResponse
        {
            Id = definition.Id,
            Code = definition.Code,
            Name = definition.Name,
            FieldType = definition.FieldType.ToString(),
            Module = definition.Module,
            IsSystem = definition.IsSystem,
            IsRequired = definition.IsRequired,
            IsActive = definition.IsActive,
            SortOrder = definition.SortOrder,
            Placeholder = definition.Placeholder,
            HelpText = definition.HelpText,
            Group = definition.Group,
            ValidationJson = definition.ValidationJson,
            Options = options
                .OrderBy(o => o.SortOrder)
                .Select(o => new CustomFieldOptionResponse
                {
                    Id = o.Id,
                    Value = o.Value,
                    Label = o.Label,
                    SortOrder = o.SortOrder,
                    IsActive = o.IsActive,
                }),
        };
    }
}
