namespace Application;

public sealed class GetMyDepartmentsQueryHandler(IUnitOfWork unitOfWork, IUserContext currentUser)
    : IRequestHandler<GetMyDepartmentsQuery, IEnumerable<DepartmentResponse>>
{
    public async Task<IEnumerable<DepartmentResponse>> Handle(GetMyDepartmentsQuery _, CancellationToken ct)
    {
        // Lấy các phòng ban mà user hiện tại là ManagerId trực tiếp.
        // Không include dept con — FE chỉ cần chọn dept gốc khi tạo phiếu.
        var depts = await unitOfWork.Repository<Department>().Query()
            .Where(d => d.ManagerId == currentUser.UserId && !d.IsDeleted)
            .OrderBy(d => d.DepartmentName)
            .Select(d => new
            {
                d.Id,
                d.DepartmentName,
                d.DepartmentCode,
                d.ParentDepartmentId,
                d.ManagerId,
                d.Description,
                d.IsActive
            })
            .ToListAsync(ct);

        if (depts.Count == 0)
            return [];

        // Batch load tên dept cha (nếu có) — 1 query thay vì N query
        var parentIds = depts.Where(d => d.ParentDepartmentId.HasValue)
            .Select(d => d.ParentDepartmentId!.Value)
            .Distinct()
            .ToList();

        var parentNames = parentIds.Count > 0
            ? await unitOfWork.Repository<Department>().Query()
                .Where(d => parentIds.Contains(d.Id))
                .Select(d => new { d.Id, d.DepartmentName })
                .ToDictionaryAsync(d => d.Id, d => d.DepartmentName, ct)
            : [];

        return depts.Select(d => new DepartmentResponse
        {
            Id = d.Id,
            DepartmentName = d.DepartmentName,
            DepartmentCode = d.DepartmentCode,
            ParentDepartmentId = d.ParentDepartmentId,
            ParentDepartmentName = d.ParentDepartmentId.HasValue
                ? parentNames.GetValueOrDefault(d.ParentDepartmentId.Value)
                : null,
            ManagerId = d.ManagerId,
            ManagerName = null, // currentUser là manager, FE đã biết
            Description = d.Description,
            IsActive = d.IsActive
        });
    }
}
