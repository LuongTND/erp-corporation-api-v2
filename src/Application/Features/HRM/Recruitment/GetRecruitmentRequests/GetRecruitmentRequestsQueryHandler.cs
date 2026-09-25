namespace Application;

public sealed class GetRecruitmentRequestsQueryHandler(
    IUnitOfWork unitOfWork,
    IUserContext currentUser,
    IDataScopeService dataScope)
    : IRequestHandler<GetRecruitmentRequestsQuery, QueryResult<RecruitmentRequestResponse>>
{
    public async Task<QueryResult<RecruitmentRequestResponse>> Handle(
        GetRecruitmentRequestsQuery q, CancellationToken ct)
    {
        // Lấy scope dữ liệu của user hiện tại từ các role đang active.
        // Scope được lấy theo mức cao nhất (max) trong tất cả role user đang giữ.
        // Thứ tự ưu tiên: Own < Team < Department < Store < Region < All
        // Nếu user có ScopeOverride thì dùng giá trị đó trực tiếp (bỏ qua role).
        var scope = await dataScope.GetUserScopeAsync(currentUser.UserId, ct);

        // Áp dụng filter từ FE (status, context, dept, store, người tạo) trước.
        // Scope filter sẽ được chồng thêm phía sau.
        var baseQuery = unitOfWork.Repository<RecruitmentRequest>().Query()
            .Where(r =>
                (!q.Status.HasValue || r.Status == q.Status.Value) &&
                (!q.RequestContext.HasValue || r.RequestContext == q.RequestContext.Value) &&
                (!q.DepartmentId.HasValue || r.DepartmentId == q.DepartmentId.Value) &&
                (!q.StoreId.HasValue || r.StoreId == q.StoreId.Value) &&
                (!q.RequestedByUserId.HasValue || r.RequestedByUserId == q.RequestedByUserId.Value));

        // Pre-load dept subtree async trước khi vào ApplyScopeFilter (sync).
        // BFS cần gọi DB nên không thể để bên trong switch sync.
        HashSet<Guid>? managedDeptIds = scope == ScopeType.Department
            ? await GetManagedDeptSubtreeAsync(ct)
            : null;

        // Giới hạn dữ liệu user được xem dựa trên scope của họ.
        // ApplyScopeFilter trả về IQueryable — chưa thực thi query xuống DB.
        baseQuery = ApplyScopeFilter(baseQuery, scope, managedDeptIds);

        var total = q.QueryInfo.NeedTotalCount ? await baseQuery.CountAsync(ct) : 0;

        var items = await baseQuery
            .OrderBy(r => r.Id)
            .Skip(q.QueryInfo.Skip)
            .Take(Math.Min(q.QueryInfo.Top, AppConstants.MaxPageSize))
            .Select(r => new RecruitmentRequestResponse
            {
                Id = r.Id,
                RequestCode = r.RequestCode,
                RequestContext = r.RequestContext.ToString(),
                DepartmentId = r.DepartmentId,
                DepartmentName = r.Department != null ? r.Department.DepartmentName : null,
                StoreId = r.StoreId,
                StoreName = r.Store != null ? r.Store.Name : null,
                PositionTitle = r.PositionTitle,
                RequestedByUserId = r.RequestedByUserId,
                RequestedByName = r.RequestedBy != null ? r.RequestedBy.FullName : string.Empty,
                Headcount = r.Headcount,
                Reason = r.Reason,
                JobDescription = r.JobDescription,
                RequiredByDate = r.RequiredByDate,
                Status = r.Status.ToString(),
                CreatedAt = r.CreatedAt
            })
            .ToListAsync(ct);

        // Đánh dấu IsAssignedApprover cho các phiếu đang PendingApproval.
        // Dùng IQueryable subquery — EF Core tự JOIN, không load userRoleIds về RAM.
        var pendingRequestIds = items
            .Where(r => r.Status == nameof(RecruitmentRequestStatus.PendingApproval))
            .Select(r => r.Id)
            .ToHashSet();

        if (pendingRequestIds.Count > 0)
        {
            // IQueryable<Guid> — chưa thực thi, sẽ được EF dịch thành subquery IN (SELECT ...)
            var activeUserRoleIds = unitOfWork.Repository<UserRole>().Query()
                .Where(ur => ur.UserId == currentUser.UserId
                          && ur.IsActive
                          && ur.RevokedAt == null
                          && (ur.ExpiresAt == null || ur.ExpiresAt > DateTimeOffset.UtcNow))
                .Select(ur => ur.RoleId);

            var assignedRequestIds = await (
                from instance in unitOfWork.Repository<WorkflowInstance>().Query()
                join task in unitOfWork.Repository<WorkflowTask>().Query()
                    on instance.Id equals task.InstanceId
                where instance.EntityType == "RecruitmentRequest"
                   && pendingRequestIds.Contains(instance.EntityId)
                   && task.Status == WorkflowTaskStatus.Pending
                   && (task.AssignedTo == currentUser.UserId
                       || (task.AssignedToRoleId != null && activeUserRoleIds.Contains(task.AssignedToRoleId.Value)))
                select instance.EntityId
            ).ToHashSetAsync(ct);

            foreach (var item in items)
                item.IsAssignedApprover = assignedRequestIds.Contains(item.Id);
        }

        return new QueryResult<RecruitmentRequestResponse> { TotalCount = total, Items = items };
    }

    /// <summary>
    /// Chồng thêm filter scope lên query gốc. Trả về IQueryable — chưa thực thi.
    /// EF Core sẽ dịch các subquery thành IN (SELECT ...) và gom lại thành 1 DB round-trip.
    ///
    /// Quy tắc visibility theo scope:
    ///   All        → thấy toàn bộ phiếu, không filter thêm
    ///   Region     → thấy phiếu của các store thuộc region mình là ManagerId
    ///   Department → thấy phiếu của dept mình manage và toàn bộ dept con (BFS trên RAM)
    ///   Store      → thấy phiếu của store mình là Store.ManagerId
    ///   Own/Team   → chỉ thấy phiếu mình tạo
    ///
    /// Mọi scope (trừ All) đều bao gồm phiếu do chính user tạo (RequestedByUserId).
    /// </summary>
    private IQueryable<RecruitmentRequest> ApplyScopeFilter(
        IQueryable<RecruitmentRequest> query, ScopeType scope, HashSet<Guid>? managedDeptIds = null)
    {
        switch (scope)
        {
            case ScopeType.All:
                return query;

            case ScopeType.Region:
                // IQueryable subquery: EF dịch thành WHERE StoreId IN (SELECT s.Id FROM Stores s WHERE s.RegionId IN (SELECT r.Id FROM Regions r WHERE r.ManagerId = @userId))
                var storeIdsInRegion = unitOfWork.Repository<Store>().Query()
                    .Where(s => s.RegionId.HasValue &&
                        unitOfWork.Repository<Region>().Query()
                            .Where(r => r.ManagerId == currentUser.UserId && !r.IsDeleted)
                            .Select(r => r.Id)
                            .Contains(s.RegionId!.Value))
                    .Select(s => s.Id);
                return query.Where(r =>
                    r.RequestedByUserId == currentUser.UserId ||
                    (r.StoreId != null && storeIdsInRegion.Contains(r.StoreId.Value)));

            case ScopeType.Department:
                // managedDeptIds được load async trước ở Handle() để tránh blocking call trong switch sync.
                // BFS cần DB nên không thể inline IQueryable — phải load trước rồi truyền vào.
                var deptIds = managedDeptIds ?? [];
                return query.Where(r =>
                    r.RequestedByUserId == currentUser.UserId ||
                    (r.DepartmentId != null && deptIds.Contains(r.DepartmentId.Value)));

            case ScopeType.Store:
                // IQueryable subquery: EF dịch thành WHERE StoreId IN (SELECT s.Id FROM Stores s WHERE s.ManagerId = @userId)
                var managedStoreIds = unitOfWork.Repository<Store>().Query()
                    .Where(s => s.ManagerId == currentUser.UserId)
                    .Select(s => s.Id);
                return query.Where(r =>
                    r.RequestedByUserId == currentUser.UserId ||
                    (r.StoreId != null && managedStoreIds.Contains(r.StoreId.Value)));

            default: // Own, Team — chỉ xem phiếu của mình
                return query.Where(r => r.RequestedByUserId == currentUser.UserId);
        }
    }

    /// <summary>
    /// Load toàn bộ dept async (1 query), rồi BFS trên RAM để lấy subtree
    /// của tất cả dept mà user đang là ManagerId.
    /// Dept count nhỏ (thường &lt; 50) nên load all vào RAM là an toàn.
    /// </summary>
    private async Task<HashSet<Guid>> GetManagedDeptSubtreeAsync(CancellationToken ct)
    {
        var allDepts = await unitOfWork.Repository<Department>().Query()
            .Where(d => !d.IsDeleted)
            .Select(d => new { d.Id, d.ParentDepartmentId, d.ManagerId })
            .ToListAsync(ct);

        // Root = các dept mà user đang là manager trực tiếp.
        var roots = allDepts.Where(d => d.ManagerId == currentUser.UserId).Select(d => d.Id).ToHashSet();
        var result = new HashSet<Guid>(roots);

        // childLookup: parentId → [childId, ...]
        var childLookup = allDepts
            .Where(d => d.ParentDepartmentId.HasValue)
            .ToLookup(d => d.ParentDepartmentId!.Value, d => d.Id);

        // BFS: duyệt xuống các dept con, cháu...
        var queue = new Queue<Guid>(roots);
        while (queue.Count > 0)
        {
            var parentId = queue.Dequeue();
            foreach (var childId in childLookup[parentId])
                if (result.Add(childId))
                    queue.Enqueue(childId);
        }

        return result;
    }
}
