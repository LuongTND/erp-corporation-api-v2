namespace Application;

public sealed class GetUsersQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetUsersQuery, IEnumerable<UserSummaryResponse>>
{
    public async Task<IEnumerable<UserSummaryResponse>> Handle(GetUsersQuery query, CancellationToken ct)
    {
        var result = await unitOfWork.Repository<User>().GetPagedAsync(
            new QueryInfo { Top = 10000, NeedTotalCount = false },
            filter: u => u.IsActive && (query.Search == null || u.FullName.Contains(query.Search) || u.EmployeeCode.Contains(query.Search)),
            orderBy: q => q.OrderBy(u => u.FullName),
            ct: ct);

        return result.Items.Adapt<IEnumerable<UserSummaryResponse>>();
    }
}
