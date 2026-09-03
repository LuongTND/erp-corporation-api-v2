namespace Application;

public sealed class GetMyPendingTasksQueryHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<GetMyPendingTasksQuery, IReadOnlyList<WorkflowTaskResponse>>
{
    public async Task<IReadOnlyList<WorkflowTaskResponse>> Handle(GetMyPendingTasksQuery q, CancellationToken ct)
    {
        var query = from t in unitOfWork.Repository<WorkflowTask>().Query()
                    join i in unitOfWork.Repository<WorkflowInstance>().Query() on t.InstanceId equals i.Id
                    where t.AssignedTo == userContext.UserId && t.Status == WorkflowTaskStatus.Pending
                    select new WorkflowTaskResponse
                    {
                        Id = t.Id,
                        InstanceId = t.InstanceId,
                        EntityType = i.EntityType,
                        EntityId = i.EntityId,
                        StepOrder = t.StepOrder,
                        StepName = t.StepName,
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
