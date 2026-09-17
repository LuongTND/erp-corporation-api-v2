namespace Application;

public sealed class GetInstanceTasksQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetInstanceTasksQuery, IReadOnlyList<WorkflowTaskResponse>>
{
    public async Task<IReadOnlyList<WorkflowTaskResponse>> Handle(GetInstanceTasksQuery q, CancellationToken ct)
    {
        // Join instance inline — 1 query thay vì FindAsync riêng + query tasks
        return await (from i in unitOfWork.Repository<WorkflowInstance>().Query()
                      where i.Id == q.InstanceId
                      join t in unitOfWork.Repository<WorkflowTask>().Query() on i.Id equals t.InstanceId
                      join u in unitOfWork.Repository<User>().Query() on t.AssignedTo equals u.Id into uj
                      from u in uj.DefaultIfEmpty()
                      join rol in unitOfWork.Repository<Role>().Query() on t.AssignedToRoleId equals rol.Id into rj
                      from rol in rj.DefaultIfEmpty()
                      join actor in unitOfWork.Repository<User>().Query() on t.ActedByUserId equals actor.Id into aj
                      from actor in aj.DefaultIfEmpty()
                      orderby t.StepOrder
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
                          AssignedToName = u != null ? u.FullName : (rol != null ? rol.RoleName : string.Empty),
                          ActedByUserId = t.ActedByUserId,
                          ActedByName = actor != null ? actor.FullName : string.Empty,
                          Status = t.Status.ToString(),
                          Note = t.Note,
                          ActedAt = t.ActedAt,
                          CreatedAt = t.CreatedAt,
                      }).ToListAsync(ct);
    }
}
