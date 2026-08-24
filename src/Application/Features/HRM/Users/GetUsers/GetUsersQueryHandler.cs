namespace Application;

public sealed class GetUsersQueryHandler(IUnitOfWork unitOfWork, IBlobStorageService blobStorage, IDataScopeService dataScope)
    : IRequestHandler<GetUsersQuery, IEnumerable<UserSummaryResponse>>
{
    private const string Container = "avatars";

    public async Task<IEnumerable<UserSummaryResponse>> Handle(GetUsersQuery query, CancellationToken ct)
    {
        var q = await dataScope.ApplyScopeAsync(unitOfWork.Repository<User>().Query(), query.CallerId, ct);

        List<Guid>? deptUserIds = null;
        if (query.DepartmentId.HasValue)
        {
            var members = await unitOfWork.Repository<UserDepartment>()
                .GetAllAsync(ud => ud.DepartmentId == query.DepartmentId.Value && ud.IsActive, ct);
            deptUserIds = members.Select(ud => ud.UserId).ToList();
            if (deptUserIds.Count == 0) return [];
        }

        List<Guid>? labelUserIds = null;
        if (query.LabelId.HasValue)
        {
            var ul = await unitOfWork.Repository<UserLabel>()
                .GetAllAsync(ul => ul.LabelId == query.LabelId.Value, ct);
            labelUserIds = ul.Select(ul => ul.UserId).ToList();
            if (labelUserIds.Count == 0) return [];
        }

        List<Guid>? storeUserIds = null;
        if (query.StoreId.HasValue)
        {
            var us = await unitOfWork.Repository<UserStore>()
                .GetAllAsync(us => us.StoreId == query.StoreId.Value && us.IsActive, ct);
            storeUserIds = us.Select(us => us.UserId).ToList();
            if (storeUserIds.Count == 0) return [];
        }
        else if (query.RegionId.HasValue)
        {
            var storeIds = (await unitOfWork.Repository<Store>()
                .GetAllAsync(s => s.RegionId == query.RegionId.Value, ct))
                .Select(s => s.Id).ToList();
            if (storeIds.Count == 0) return [];
            var us = await unitOfWork.Repository<UserStore>()
                .GetAllAsync(us => storeIds.Contains(us.StoreId) && us.IsActive, ct);
            storeUserIds = us.Select(us => us.UserId).Distinct().ToList();
            if (storeUserIds.Count == 0) return [];
        }

        var users = await q
            .Where(u => (query.Status == null ? u.IsActive : u.Status == query.Status.Value)
                && (query.Search == null || u.FullName.Contains(query.Search) || u.EmployeeCode.Contains(query.Search))
                && (query.JobLevelId == null || u.JobLevelId == query.JobLevelId)
                && (deptUserIds == null || deptUserIds.Contains(u.Id))
                && (labelUserIds == null || labelUserIds.Contains(u.Id))
                && (storeUserIds == null || storeUserIds.Contains(u.Id)))
            .OrderBy(u => u.FullName)
            .ToListAsync(ct);

        var userIds = users.Select(u => u.Id).ToList();
        var userLabels = await unitOfWork.Repository<UserLabel>()
            .GetAllAsync(ul => userIds.Contains(ul.UserId), ct);
        var labelIds = userLabels.Select(ul => ul.LabelId).Distinct().ToList();
        var labels = (await unitOfWork.Repository<Label>()
            .GetAllAsync(l => labelIds.Contains(l.Id), ct))
            .ToDictionary(l => l.Id);
        var labelsByUser = userLabels
            .GroupBy(ul => ul.UserId)
            .ToDictionary(g => g.Key, g => g
                .Where(ul => labels.ContainsKey(ul.LabelId))
                .Select(ul => new LabelResponse { Id = labels[ul.LabelId].Id, Name = labels[ul.LabelId].Name, Color = labels[ul.LabelId].Color, IsActive = labels[ul.LabelId].IsActive })
                .ToList());

        return users.Select(u => new UserSummaryResponse
        {
            Id = u.Id,
            FullName = u.FullName,
            EmployeeCode = u.EmployeeCode,
            Email = u.Email,
            AvatarUrl = u.AvatarUrl is null ? null : blobStorage.GetUrl(Container, u.AvatarUrl),
            Status = u.Status.ToString(),
            JoinDate = u.CreatedAt,
            Labels = labelsByUser.TryGetValue(u.Id, out var lbls) ? lbls : [],
        });
    }
}
