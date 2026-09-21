namespace Application;

public sealed class GetRoundTypesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRoundTypesQuery, IEnumerable<RoundTypeResponse>>
{
    public async Task<IEnumerable<RoundTypeResponse>> Handle(GetRoundTypesQuery _, CancellationToken ct)
    {
        var list = await unitOfWork.Repository<Domain.RoundType>()
            .GetAllAsync(r => true, ct);
        return list.OrderBy(r => r.DisplayOrder).ThenBy(r => r.Name).Adapt<IEnumerable<RoundTypeResponse>>();
    }
}
