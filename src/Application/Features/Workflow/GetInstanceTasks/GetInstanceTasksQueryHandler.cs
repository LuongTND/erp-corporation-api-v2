namespace Application;

public sealed class GetInstanceTasksQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetInstanceTasksQuery, IReadOnlyList<WorkflowTaskResponse>>
{
    public async Task<IReadOnlyList<WorkflowTaskResponse>> Handle(GetInstanceTasksQuery q, CancellationToken ct)
    {
        var instance = await unitOfWork.Repository<WorkflowInstance>()
            .FindAsync(i => i.Id == q.InstanceId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("WorkflowInstance", q.InstanceId));

        return await unitOfWork.Repository<WorkflowTask>()
            .Query()
            .Where(t => t.InstanceId == q.InstanceId)
            .OrderBy(t => t.StepOrder)
            .Select(t => new WorkflowTaskResponse
            {
                Id = t.Id,
                InstanceId = t.InstanceId,
                EntityType = instance.EntityType,
                EntityId = instance.EntityId,
                StepOrder = t.StepOrder,
                StepName = t.StepName,
                Status = t.Status.ToString(),
                Note = t.Note,
                ActedAt = t.ActedAt,
                CreatedAt = t.CreatedAt,
            })
            .ToListAsync(ct);
    }
}
