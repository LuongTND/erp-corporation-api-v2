using API.Base;
using API.Filters;
using Application.DTOs.Users;
using Application.Interfaces.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Users;

public class UsersController : BaseApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // API lấy danh sách tất cả người dùng, sau khi lấy về thì dùng filter để lọc các người dùng cần hiển thị
    [HttpGet]
    [AuthorizePermission("hrm.employee.read")]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken ct)
    {
        var users = await _userService.GetAllAsync(ct);
        return Ok(users);
    }

    // API lấy thông tin người dùng theo id, dùng để hiển thị thông tin người dùng
    [HttpGet("{id:guid}")]
    [AuthorizePermission("hrm.employee.read")]
    public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken ct)
    {
        var user = await _userService.GetByIdAsync(id, ct);
        return Ok(user);
    }

    // API tạo người dùng mới, sau khi tạo thành công sẽ trả về thông tin người dùng vừa tạo
    [HttpPost]
    [AuthorizePermission("hrm.employee.create")]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var user = await _userService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    // API cập nhật thông tin người dùng, sau khi cập nhật thành công sẽ trả về thông tin người dùng vừa cập nhật
    [HttpPut("{id:guid}")]
    [AuthorizePermission("hrm.employee.update")]
    public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        var user = await _userService.UpdateAsync(id, request, ct);
        return Ok(user);
    }

    // API xóa thông tin người dùng, sau khi xóa thành công sẽ trả về thông báo
    [HttpDelete("{id:guid}")]
    [AuthorizePermission("hrm.employee.delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _userService.DeleteAsync(id, ct);
        return NoContent();
    }

    // API gán vai trò cho người dùng, sau khi gán thành công sẽ trả về thông báo
    [HttpPost("{id:guid}/roles")]
    [AuthorizePermission("system.role.assign")]
    public async Task<IActionResult> AssignRoles(Guid id, [FromBody] List<Guid> roleIds, CancellationToken ct)
    {
        await _userService.AssignRolesAsync(id, roleIds, ct);
        return Ok(new { Message = "Gán vai trò thành công." });
    }

    // API đặt lại mật khẩu, sau khi đặt lại mật khẩu thành công sẽ trả về thông báo
    [HttpPost("{id:guid}/reset-password")]
    [AuthorizePermission("system.user.resetpassword")]
    public async Task<IActionResult> ResetPassword(Guid id, [FromBody] string newPassword, CancellationToken ct)
    {
        await _userService.ResetPasswordAsync(id, newPassword, ct);
        return Ok(new { Message = "Đặt lại mật khẩu thành công." });
    }

    // API lấy danh sách phòng ban kiêm nhiệm của người dùng
    [HttpGet("{id:guid}/departments")]
    [AuthorizePermission("hrm.employee.read")]
    public async Task<ActionResult<IReadOnlyList<UserDepartmentDto>>> GetSecondaryDepartments(Guid id, CancellationToken ct)
    {
        var list = await _userService.GetSecondaryDepartmentsAsync(id, ct);
        return Ok(list);
    }

    // API gán phòng ban kiêm nhiệm cho người dùng
    [HttpPost("{id:guid}/departments")]
    [AuthorizePermission("hrm.employee.update")]
    public async Task<ActionResult<UserDepartmentDto>> AddSecondaryDepartment(Guid id, [FromBody] AddUserDepartmentRequest request, CancellationToken ct)
    {
        var userDept = await _userService.AddSecondaryDepartmentAsync(id, request, ct);
        return Ok(userDept);
    }

    // API gỡ bỏ phòng ban kiêm nhiệm của người dùng
    [HttpDelete("{id:guid}/departments/{departmentId:guid}")]
    [AuthorizePermission("hrm.employee.update")]
    public async Task<IActionResult> RemoveSecondaryDepartment(Guid id, Guid departmentId, CancellationToken ct)
    {
        await _userService.RemoveSecondaryDepartmentAsync(id, departmentId, ct);
        return NoContent();
    }
}
