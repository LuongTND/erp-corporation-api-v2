namespace Application;

public sealed class GetBonusPoliciesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetBonusPoliciesQuery, QueryResult<BonusPolicyResponse>>
{
    public async Task<QueryResult<BonusPolicyResponse>> Handle(GetBonusPoliciesQuery query, CancellationToken ct)
    {
        var search = query.QueryInfo.SearchText?.Trim().ToLower();

        var result = await unitOfWork.Repository<Domain.BonusPolicy>().GetPagedAsync(
            query.QueryInfo,
            filter: b => search == null || b.Name.ToLower().Contains(search),
            orderBy: q => q.OrderBy(b => b.Name),
            ct: ct);

        return new QueryResult<BonusPolicyResponse>
        {
            Items = result.Items.Select(b => new BonusPolicyResponse
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                IsActive = b.IsActive
            }),
            TotalCount = result.TotalCount
        };
    }
}
