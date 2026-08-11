namespace Application;

public sealed class GetUserStatusHistoryQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetUserStatusHistoryQuery, IEnumerable<UserStatusHistoryResponse>>
{
    public async Task<IEnumerable<UserStatusHistoryResponse>> Handle(
        GetUserStatusHistoryQuery query, CancellationToken ct)
        => await unitOfWork.Repository<UserStatusHistory>()
            .Query()
            .Where(h => h.UserId == query.UserId)
            .OrderByDescending(h => h.ChangedAt)
            .ProjectToType<UserStatusHistoryResponse>()
            .ToListAsync(ct);
}
