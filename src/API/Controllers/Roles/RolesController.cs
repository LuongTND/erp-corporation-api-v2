using API.Base;
using API.Filters;
using Application.DTOs.Roles;
using Application.Interfaces.Services.Roles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Roles;

public class RolesController : BaseApiController
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    // API lấy tất cả các vai trò, dùng để hiển thị danh sách vai trò
    [HttpGet]
    [AuthorizePermission("system.role.read")]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetAll(CancellationToken ct)
    {
        var roles = await _roleService.GetAllAsync(ct);
        return Ok(roles);
    }

    // API lấy thông tin vai trò theo id, dùng để hiển thị thông tin vai trò
    [HttpGet("{id:guid}")]
    [AuthorizePermission("system.role.read")]
    public async Task<ActionResult<RoleDto>> GetById(Guid id, CancellationToken ct)
    {
        var role = await _roleService.GetByIdAsync(id, ct);
        return Ok(role);
    }

    // API lấy tất cả các quyền, dùng để hiển thị danh sách quyền để gán cho vai trò
    [HttpGet("permissions")]
    [AuthorizePermission("system.role.read")]
    public async Task<ActionResult<IReadOnlyList<PermissionDto>>> GetAllPermissions(CancellationToken ct)
    {
        var permissions = await _roleService.GetAllPermissionsAsync(ct);
        return Ok(permissions);
    }   

    // API tạo vai trò mới, sau khi tạo thành công sẽ trả về thông tin vai trò vừa tạo
    [HttpPost]
    [AuthorizePermission("system.role.create")]
    public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleRequest request, CancellationToken ct)
    {
        var role = await _roleService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
    }

    // API cập nhật danh sách quyền cho vai trò, sau khi cập nhật thành công sẽ trả về thông báo
    [HttpPut("{id:guid}/permissions")]
    [AuthorizePermission("system.role.update")]
    public async Task<IActionResult> UpdatePermissions(Guid id, [FromBody] UpdateRolePermissionsRequest request, CancellationToken ct)
    {
        await _roleService.UpdatePermissionsAsync(id, request, ct);
        return Ok(new { Message = "Cập nhật danh sách quyền cho vai trò thành công." });
    }

    // API cập nhật thông tin vai trò
    [HttpPut("{id:guid}")]
    [AuthorizePermission("system.role.update")]
    public async Task<ActionResult<RoleDto>> Update(Guid id, [FromBody] UpdateRoleRequest request, CancellationToken ct)
    {
        var role = await _roleService.UpdateAsync(id, request, ct);
        return Ok(role);
    }

    // API xóa/vô hiệu hóa vai trò
    [HttpDelete("{id:guid}")]
    [AuthorizePermission("system.role.update")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _roleService.DeleteAsync(id, ct);
        return NoContent();
    }
}
