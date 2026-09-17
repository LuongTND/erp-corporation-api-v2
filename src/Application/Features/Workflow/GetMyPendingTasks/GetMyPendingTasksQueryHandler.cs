namespace Application;

public sealed class GetMyPendingTasksQueryHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<GetMyPendingTasksQuery, IReadOnlyList<WorkflowTaskResponse>>
{
    public async Task<IReadOnlyList<WorkflowTaskResponse>> Handle(GetMyPendingTasksQuery q, CancellationToken ct)
    {
        var userId = userContext.UserId;

        // roleIds hiện tại của user — dùng để lấy role tasks
        var myRoleIds = await unitOfWork.Repository<UserRole>()
            .Query()
            .Where(ur => ur.UserId == userId && ur.IsActive && ur.RevokedAt == null
                         && (ur.ExpiresAt == null || ur.ExpiresAt > DateTimeOffset.UtcNow))
            .Select(ur => ur.RoleId)
            .ToListAsync(ct);

        var query = from t in unitOfWork.Repository<WorkflowTask>().Query()
                    join i in unitOfWork.Repository<WorkflowInstance>().Query() on t.InstanceId equals i.Id
                    join u in unitOfWork.Repository<User>().Query() on t.AssignedTo equals u.Id into uj
                    from u in uj.DefaultIfEmpty()
                    where t.Status == WorkflowTaskStatus.Pending
                          && (t.AssignedTo == userId || (t.AssignedToRoleId != null && myRoleIds.Contains(t.AssignedToRoleId.Value)))
                    select new WorkflowTaskResponse
                    {
                        Id = t.Id,
                        InstanceId = t.InstanceId,
                        EntityType = i.EntityType,
                        EntityId = i.EntityId,
                        StepOrder = t.StepOrder,
                        StepName = t.StepName,
                        AssignedTo = t.AssignedTo,
                        AssignedToRoleId = t.AssignedToRoleId,
                        AssignedToName = u != null ? u.FullName : string.Empty,
                        Status = t.Status.ToString(),
                        Note = t.Note,
                        ActedAt = t.ActedAt,
                        CreatedAt = t.CreatedAt,
                    };

        if (q.EntityType is not null)
            query = query.Where(t => t.EntityType == q.EntityType);

        return await query.OrderBy(t => t.CreatedAt).ToListAsync(ct);
    }
}
