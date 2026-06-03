using API.Base;
using API.Filters;
using Application.DTOs.Departments;
using Application.Interfaces.Services.Departments;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Departments;

public class DepartmentsController : BaseApiController
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    // API lấy tất cả phòng ban, sau khi lấy về thì dùng filter để lọc các phòng ban cần hiển thị
    [HttpGet]
    [AuthorizePermission("hrm.department.read")]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetAll(CancellationToken ct)
    {
        var depts = await _departmentService.GetAllAsync(ct);
        return Ok(depts);
    }

    // API lấy thông tin phòng ban theo id, dùng để hiển thị thông tin phòng ban
    [HttpGet("{id:guid}")]
    [AuthorizePermission("hrm.department.read")]
    public async Task<ActionResult<DepartmentDto>> GetById(Guid id, CancellationToken ct)
    {
        var dept = await _departmentService.GetByIdAsync(id, ct);
        return Ok(dept);
    }

    // API tạo phòng ban mới, sau khi tạo thành công sẽ trả về thông tin phòng ban vừa tạo
    [HttpPost]
    [AuthorizePermission("hrm.department.create")]
    public async Task<ActionResult<DepartmentDto>> Create([FromBody] CreateDepartmentRequest request, CancellationToken ct)
    {
        var dept = await _departmentService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = dept.Id }, dept);
    }

    // API cập nhật thông tin phòng ban, sau khi cập nhật thành công sẽ trả về thông tin phòng ban vừa cập nhật
    [HttpPut("{id:guid}")]
    [AuthorizePermission("hrm.department.update")]
    public async Task<ActionResult<DepartmentDto>> Update(Guid id, [FromBody] UpdateDepartmentRequest request, CancellationToken ct)
    {
        var dept = await _departmentService.UpdateAsync(id, request, ct);
        return Ok(dept);
    }
}
