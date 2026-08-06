namespace Application;

public sealed class GetCustomFieldDefinitionsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetCustomFieldDefinitionsQuery, IEnumerable<CustomFieldDefinitionResponse>>
{
    public async Task<IEnumerable<CustomFieldDefinitionResponse>> Handle(GetCustomFieldDefinitionsQuery query, CancellationToken ct)
    {
        var all = await unitOfWork.Repository<CustomFieldDefinition>()
            .GetAllAsync(d => query.Module == null || d.Module == query.Module, ct);

        if (all.Count == 0) return [];

        var defIds = all.Select(d => d.Id).ToHashSet();
        var allOptions = await unitOfWork.Repository<CustomFieldOption>()
            .GetAllAsync(o => defIds.Contains(o.DefinitionId), ct);

        var optsByDef = allOptions
            .GroupBy(o => o.DefinitionId)
            .ToDictionary(g => g.Key, g => g.OrderBy(o => o.SortOrder).ToList());

        return all
            .OrderBy(d => d.SortOrder)
            .Select(d => MapToResponse(d, optsByDef.GetValueOrDefault(d.Id) ?? []));
    }

    private static CustomFieldDefinitionResponse MapToResponse(CustomFieldDefinition d, List<CustomFieldOption> options)
        => new()
        {
            Id = d.Id,
            Code = d.Code,
            Name = d.Name,
            FieldType = d.FieldType.ToString(),
            Module = d.Module,
            IsSystem = d.IsSystem,
            IsRequired = d.IsRequired,
            IsActive = d.IsActive,
            SortOrder = d.SortOrder,
            Placeholder = d.Placeholder,
            HelpText = d.HelpText,
            Group = d.Group,
            ValidationJson = d.ValidationJson,
            Options = options.Select(o => new CustomFieldOptionResponse
            {
                Id = o.Id,
                Value = o.Value,
                Label = o.Label,
                SortOrder = o.SortOrder,
                IsActive = o.IsActive,
            }),
        };
}
