namespace Application;

public sealed class GetLabelsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetLabelsQuery, IEnumerable<LabelResponse>>
{
    public async Task<IEnumerable<LabelResponse>> Handle(GetLabelsQuery query, CancellationToken ct)
    {
        var labels = await unitOfWork.Repository<Label>().GetAllAsync(
            l => (query.IsActive == null || l.IsActive == query.IsActive.Value)
              && (query.Search == null || l.Name.Contains(query.Search)),
            ct);

        return labels
            .OrderBy(l => l.Name)
            .Select(l => new LabelResponse { Id = l.Id, Name = l.Name, Color = l.Color, IsActive = l.IsActive });
    }
}
