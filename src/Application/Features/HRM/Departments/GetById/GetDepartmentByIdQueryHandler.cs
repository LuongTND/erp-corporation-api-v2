namespace Application;

public sealed class GetDepartmentByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetDepartmentByIdQuery, DepartmentResponse>
{
    public async Task<DepartmentResponse> Handle(GetDepartmentByIdQuery query, CancellationToken ct)
    {
        var dept = await unitOfWork.Repository<Department>()
            .FindAsync(d => d.Id == query.DepartmentId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Department", query.DepartmentId));

        string? parentName = null;
        if (dept.ParentDepartmentId.HasValue)
        {
            var parent = await unitOfWork.Repository<Department>()
                .FindAsync(d => d.Id == dept.ParentDepartmentId.Value, ct);
            parentName = parent?.DepartmentName;
        }

        string? managerName = null;
        if (dept.ManagerId.HasValue)
        {
            var manager = await unitOfWork.Repository<User>()
                .FindAsync(u => u.Id == dept.ManagerId.Value, ct);
            managerName = manager?.FullName;
        }

        return new DepartmentResponse
        {
            Id = dept.Id,
            DepartmentName = dept.DepartmentName,
            DepartmentCode = dept.DepartmentCode,
            ParentDepartmentId = dept.ParentDepartmentId,
            ParentDepartmentName = parentName,
            ManagerId = dept.ManagerId,
            ManagerName = managerName,
            Description = dept.Description,
            IsActive = dept.IsActive
        };
    }
}
