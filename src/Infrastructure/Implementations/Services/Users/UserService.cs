using Application.Common.Exceptions;
using Application.DTOs.Users;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.Users;
using Application.Interfaces.Repositories.Departments;
using Application.Interfaces.Repositories.Roles;
using Application.Interfaces.Services.Users;
using Application.Interfaces.Services.Auth;
using Infrastructure.Implementations.Services.Auth;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations.Services.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IGenericRepository<JobLevel> _jobLevelRepository;
    private readonly IGenericRepository<UserDepartment> _userDeptRepository;
    private readonly IGenericRepository<UserRole> _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDataScopeService _dataScopeService;
    private readonly ICurrentUserService _currentUserService;

    public UserService(
        IUserRepository userRepository,
        IUserAccountRepository userAccountRepository,
        IDepartmentRepository departmentRepository,
        IRoleRepository roleRepository,
        IGenericRepository<JobLevel> jobLevelRepository,
        IGenericRepository<UserDepartment> userDeptRepository,
        IGenericRepository<UserRole> userRoleRepository,
        IUnitOfWork unitOfWork,
        IDataScopeService dataScopeService,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _userAccountRepository = userAccountRepository;
        _departmentRepository = departmentRepository;
        _roleRepository = roleRepository;
        _jobLevelRepository = jobLevelRepository;
        _userDeptRepository = userDeptRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
        _dataScopeService = dataScopeService;
        _currentUserService = currentUserService;
    }

    public async Task<UserDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        var scope = await _dataScopeService.GetEffectiveScopeAsync(currentUserId, ct);
        var deptIds = await _dataScopeService.GetAccessibleDepartmentIdsAsync(currentUserId, ct);

        var user = await _userRepository.GetByIdWithDetailsScopedAsync(id, currentUserId, scope, deptIds, ct);

        if (user == null)
            throw new NotFoundException("Không tìm thấy nhân sự hoặc bạn không có quyền xem thông tin nhân sự này.");

        return MapToDto(user);
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        var scope = await _dataScopeService.GetEffectiveScopeAsync(currentUserId, ct);
        var deptIds = await _dataScopeService.GetAccessibleDepartmentIdsAsync(currentUserId, ct);

        var users = await _userRepository.GetWithDetailsScopedAsync(currentUserId, scope, deptIds, ct);

        return users.Select(MapToDto).ToList();
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        // 1. Kiểm tra mã nhân viên & Email đã tồn tại chưa
        var employeeCodeUpper = request.EmployeeCode.ToUpperInvariant();
        var emailLower = request.Email.ToLowerInvariant();

        if (await _userRepository.ExistsByEmployeeCodeAsync(employeeCodeUpper, ct))
            throw new ConflictException($"Mã nhân viên '{request.EmployeeCode}' đã tồn tại trong hệ thống.");

        if (await _userRepository.ExistsByEmailAsync(emailLower, ct))
            throw new ConflictException($"Email '{request.Email}' đã được sử dụng bởi nhân sự khác.");

        // Kiểm tra Department & JobLevel hợp lệ
        var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, ct);
        if (department == null || !department.IsActive)
            throw new NotFoundException("Phòng ban không tồn tại hoặc đã bị vô hiệu hóa.");

        var jobLevel = await _jobLevelRepository.GetByIdAsync(request.JobLevelId, ct);
        if (jobLevel == null || !jobLevel.IsActive)
            throw new NotFoundException("Cấp bậc chức danh không tồn tại hoặc đã bị vô hiệu hóa.");

        if (request.ManagerId.HasValue)
        {
            var manager = await _userRepository.GetByIdAsync(request.ManagerId.Value, ct);
            if (manager == null)
                throw new NotFoundException("Quản lý trực tiếp không tồn tại.");
        }

        // 2. Tạo nhân sự mới
        var user = User.Create(
            employeeCodeUpper,
            request.FullName,
            emailLower,
            request.DepartmentId,
            request.JobLevelId,
            request.DateOfJoin,
            request.Status,
            request.ManagerId,
            request.AvatarUrl
        );

        await _userRepository.AddAsync(user, ct);

        // 3. Tạo tài khoản đăng nhập (UserAccount)
        var password = request.Password ?? "123456aA@"; // Mật khẩu mặc định nếu không truyền
        var passwordHash = PasswordHasher.Hash(password);
        var account = UserAccount.Create(user.Id, emailLower, passwordHash);
        await _userAccountRepository.AddAsync(account, ct);

        // 4. Tạo bản ghi phòng ban chính (UserDepartment)
        var userDept = UserDepartment.Create(user.Id, request.DepartmentId, isPrimary: true, request.DateOfJoin);
        await _userDeptRepository.AddAsync(userDept, ct);

        // 5. Gán vai trò mặc định (ví dụ EMPLOYEE nếu có)
        var defaultRole = await _roleRepository.GetQueryable()
            .FirstOrDefaultAsync(r => r.RoleName == "ROLE_EMPLOYEE", ct);
        if (defaultRole != null)
        {
            var userRole = UserRole.Create(user.Id, defaultRole.Id);
            await _userRoleRepository.AddAsync(userRole, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        // Tải lại user để có đủ thông tin liên kết phòng ban/cấp bậc cho DTO
        return await GetByIdAsync(user.Id, ct);
    }

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var user = await _userRepository.GetQueryable()
            .Include(u => u.UserAccount)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        if (user == null)
            throw new NotFoundException("Không tìm thấy nhân sự cần cập nhật.");

        var emailLower = request.Email.ToLowerInvariant();
        if (user.Email != emailLower && await _userRepository.ExistsByEmailExcludeIdAsync(emailLower, id, ct))
            throw new ConflictException($"Email '{request.Email}' đã được sử dụng bởi nhân sự khác.");

        var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, ct);
        if (department == null || !department.IsActive)
            throw new NotFoundException("Phòng ban không tồn tại hoặc đã bị vô hiệu hóa.");

        var jobLevel = await _jobLevelRepository.GetByIdAsync(request.JobLevelId, ct);
        if (jobLevel == null || !jobLevel.IsActive)
            throw new NotFoundException("Cấp bậc chức danh không tồn tại hoặc đã bị vô hiệu hóa.");

        if (request.ManagerId.HasValue)
        {
            var manager = await _userRepository.GetByIdAsync(request.ManagerId.Value, ct);
            if (manager == null)
                throw new NotFoundException("Quản lý trực tiếp không tồn tại.");
        }

        // Cập nhật thông tin chính
        user.UpdateProfile(
            request.FullName,
            emailLower,
            request.DepartmentId,
            request.JobLevelId,
            request.DateOfJoin,
            request.Status,
            request.ManagerId,
            request.AvatarUrl
        );

        // Đồng bộ LoginEmail của tài khoản đăng nhập
        if (user.UserAccount != null && user.UserAccount.LoginEmail != emailLower)
        {
            user.UserAccount.UpdateEmail(emailLower);
        }

        // Cập nhật UserDepartment chính nếu thay đổi phòng ban
        var primaryDept = await _userDeptRepository.GetQueryable()
            .FirstOrDefaultAsync(ud => ud.UserId == id && ud.IsPrimary && ud.IsActive, ct);

        if (primaryDept != null && primaryDept.DepartmentId != request.DepartmentId)
        {
            primaryDept.Terminate(request.DateOfJoin); // Đóng phòng cũ
            var newPrimaryDept = UserDepartment.Create(id, request.DepartmentId, isPrimary: true, request.DateOfJoin);
            await _userDeptRepository.AddAsync(newPrimaryDept, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);
        return await GetByIdAsync(id, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _userRepository.GetQueryable()
            .Include(u => u.UserAccount)
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        if (user == null)
            throw new NotFoundException("Không tìm thấy nhân sự.");

        // Vô hiệu hóa nhân sự
        user.SetStatus(UserStatus.Resigned);

        // Khóa tài khoản đăng nhập
        user.UserAccount?.Lock();

        // Thu hồi toàn bộ vai trò đang có
        foreach (var userRole in user.UserRoles.Where(ur => ur.IsActive))
        {
            userRole.Revoke();
        }

        // Vô hiệu hóa các phòng ban kiêm nhiệm
        var userDepts = await _userDeptRepository.GetQueryable()
            .Where(ud => ud.UserId == id && ud.IsActive)
            .ToListAsync(ct);

        foreach (var ud in userDepts)
        {
            ud.Terminate(DateOnly.FromDateTime(DateTime.UtcNow));
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task AssignRolesAsync(Guid id, List<Guid> roleIds, CancellationToken ct = default)
    {
        var user = await _userRepository.GetQueryable()
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        if (user == null)
            throw new NotFoundException("Không tìm thấy nhân sự.");

        // Kiểm tra tất cả RoleIds hợp lệ và đang active
        var validRoles = await _roleRepository.GetQueryable()
            .Where(r => roleIds.Contains(r.Id) && r.IsActive)
            .ToListAsync(ct);

        if (validRoles.Count != roleIds.Count)
            throw new NotFoundException("Một hoặc nhiều vai trò không tồn tại hoặc đã bị vô hiệu hóa.");

        // Thu hồi các vai trò cũ không nằm trong danh sách mới
        foreach (var ur in user.UserRoles.Where(ur => ur.IsActive))
        {
            if (!roleIds.Contains(ur.RoleId))
            {
                ur.Revoke();
            }
        }

        // Gán các vai trò mới chưa có
        var activeRoleIds = user.UserRoles.Where(ur => ur.IsActive).Select(ur => ur.RoleId).ToList();
        foreach (var roleId in roleIds)
        {
            if (!activeRoleIds.Contains(roleId))
            {
                var newUr = UserRole.Create(id, roleId);
                await _userRoleRepository.AddAsync(newUr, ct);
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ResetPasswordAsync(Guid id, string newPassword, CancellationToken ct = default)
    {
        var account = await _userAccountRepository.GetByUserIdAsync(id, ct);

        if (account == null)
            throw new NotFoundException("Tài khoản đăng nhập của nhân sự này không tồn tại.");

        var passwordHash = PasswordHasher.Hash(newPassword);
        account.UpdatePassword(passwordHash);
        account.Unlock(); // Mở khóa nếu bị khóa do nhập sai nhiều lần

        await _unitOfWork.SaveChangesAsync(ct);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            EmployeeCode = user.EmployeeCode,
            FullName = user.FullName,
            Email = user.Email,
            AvatarUrl = user.AvatarUrl,
            DepartmentId = user.DepartmentId,
            DepartmentName = user.Department?.DepartmentName ?? string.Empty,
            JobLevelId = user.JobLevelId,
            JobLevelName = user.JobLevel?.LevelName ?? string.Empty,
            ManagerId = user.ManagerId,
            ManagerName = user.Manager?.FullName,
            DateOfJoin = user.DateOfJoin,
            Status = user.Status,
            IsActive = user.IsActive,
            Roles = user.UserRoles
                .Where(ur => ur.IsActive && ur.RevokedAt == null && ur.Role.IsActive)
                .Select(ur => ur.Role.RoleName)
                .ToList()
        };
    }
}
