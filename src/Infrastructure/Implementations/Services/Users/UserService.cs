using FluentValidation;

namespace Infrastructure;

[RegisterService(typeof(IUserService))]
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IJobLevelRepository _jobLevelRepository;
    private readonly IUserDepartmentRepository _userDeptRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDataScopeService _dataScopeService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<AddUserDepartmentRequest> _addDeptValidator;
    private readonly INotificationPublisher _notificationPublisher;
    private readonly INotificationActorResolver _notificationActorResolver;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        IUserAccountRepository userAccountRepository,
        IDepartmentRepository departmentRepository,
        IRoleRepository roleRepository,
        IJobLevelRepository jobLevelRepository,
        IUserDepartmentRepository userDeptRepository,
        IUnitOfWork unitOfWork,
        IDataScopeService dataScopeService,
        ICurrentUserService currentUserService,
        IValidator<AddUserDepartmentRequest> addDeptValidator,
        INotificationPublisher notificationPublisher,
        INotificationActorResolver notificationActorResolver,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _userAccountRepository = userAccountRepository;
        _departmentRepository = departmentRepository;
        _roleRepository = roleRepository;
        _jobLevelRepository = jobLevelRepository;
        _userDeptRepository = userDeptRepository;
        _unitOfWork = unitOfWork;
        _dataScopeService = dataScopeService;
        _currentUserService = currentUserService;
        _addDeptValidator = addDeptValidator;
        _notificationPublisher = notificationPublisher;
        _notificationActorResolver = notificationActorResolver;
        _mapper = mapper;
    }

    public async Task<UserDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        var scopeContext = await _dataScopeService.GetUserScopeContextAsync(currentUserId, ct);

        var user = await _userRepository.GetByIdWithDetailsScopedAsync(
            id, currentUserId, scopeContext.Scope, scopeContext.AccessibleDepartmentIds, ct);

        if (user == null)
            throw new NotFoundException("Không tìm thấy nhân sự hoặc bạn không có quyền xem thông tin nhân sự này.");

        return _mapper.Map<UserDto>(user);
    }

    public async Task<PaginatedResult<UserDto>> GetPagedAsync(PaginationQuery query, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        var scopeContext = await _dataScopeService.GetUserScopeContextAsync(currentUserId, ct);

        var result = await _userRepository.GetPagedWithDetailsScopedAsync(
            currentUserId, scopeContext.Scope, scopeContext.AccessibleDepartmentIds, query, ct);

        return PaginationMapper.Map<User, UserDto>(result, _mapper);
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        // 1. Kiểm tra mã nhân viên & Email đã tồn tại chưa
        var employeeCode = request.EmployeeCode.Trim();
        var emailLower = request.Email.ToLowerInvariant();

        if (await _userRepository.ExistsByEmployeeCodeAsync(employeeCode, ct))
            throw new ConflictException($"Mã nhân viên '{employeeCode}' đã tồn tại trong hệ thống.");

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
            employeeCode,
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
        var defaultRole = await _roleRepository.GetByNameAsync("ROLE_EMPLOYEE", ct);
        if (defaultRole != null)
        {
            var userRole = UserRole.Create(user.Id, defaultRole.Id);
            await _userRepository.AddUserRoleAsync(userRole, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        var actorName = await _notificationActorResolver.GetActorDisplayNameAsync(ct);

        await _notificationPublisher.PublishAsync(
            NotificationTriggers.UserCreate,
            _notificationActorResolver.BuildContext(user.Id),
            new
            {
                fullName = user.FullName,
                employeeCode = user.EmployeeCode,
                actorName,
                userId = user.Id
            },
            cancellationToken: ct);

        // Tải lại user để có đủ thông tin liên kết phòng ban/cấp bậc cho DTO
        return await GetByIdAsync(user.Id, ct);
    }

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdWithAccountAsync(id, ct);

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
        var primaryDept = await _userDeptRepository.GetActivePrimaryDepartmentByUserIdAsync(id, ct);

        if (primaryDept != null && primaryDept.DepartmentId != request.DepartmentId)
        {
            primaryDept.Terminate(request.DateOfJoin); // Đóng phòng cũ
            var newPrimaryDept = UserDepartment.Create(id, request.DepartmentId, isPrimary: true, request.DateOfJoin);
            await _userDeptRepository.AddAsync(newPrimaryDept, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        var actorName = await _notificationActorResolver.GetActorDisplayNameAsync(ct);

        await _notificationPublisher.PublishAsync(
            NotificationTriggers.UserUpdate,
            _notificationActorResolver.BuildContext(user.Id),
            new
            {
                fullName = user.FullName,
                employeeCode = user.EmployeeCode,
                actorName,
                userId = user.Id
            },
            cancellationToken: ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdWithAccountAndRolesAsync(id, ct);

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
        var userDepts = await _userDeptRepository.GetActiveDepartmentsByUserIdAsync(id, ct);

        foreach (var ud in userDepts)
        {
            ud.Terminate(DateOnly.FromDateTime(DateTime.UtcNow));
        }

        await _unitOfWork.SaveChangesAsync(ct);

        var actorName = await _notificationActorResolver.GetActorDisplayNameAsync(ct);
        await _notificationPublisher.PublishAsync(
            NotificationTriggers.UserDelete,
            _notificationActorResolver.BuildContext(user.Id),
            new
            {
                fullName = user.FullName,
                employeeCode = user.EmployeeCode,
                actorName,
                userId = user.Id
            },
            cancellationToken: ct);
    }

    public async Task AssignRolesAsync(Guid id, List<Guid> roleIds, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdWithRolesAsync(id, ct);

        if (user == null)
            throw new NotFoundException("Không tìm thấy nhân sự.");

        // Kiểm tra tất cả RoleIds hợp lệ và đang active
        var validRoles = await _roleRepository.GetActiveRolesByIdsAsync(roleIds, ct);

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
                await _userRepository.AddUserRoleAsync(newUr, ct);
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);

        var actorName = await _notificationActorResolver.GetActorDisplayNameAsync(ct);
        await _notificationPublisher.PublishAsync(
            NotificationTriggers.UserAssignRoles,
            _notificationActorResolver.BuildContext(user.Id),
            new
            {
                fullName = user.FullName,
                employeeCode = user.EmployeeCode,
                actorName,
                userId = user.Id
            },
            cancellationToken: ct);
    }

    public async Task ResetPasswordAsync(Guid id, string newPassword, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            throw new ConflictException("Mật khẩu mới phải từ 6 ký tự trở lên.");

        var account = await _userAccountRepository.GetByUserIdAsync(id, ct);

        if (account == null)
            throw new NotFoundException("Tài khoản đăng nhập của nhân sự này không tồn tại.");

        var passwordHash = PasswordHasher.Hash(newPassword);
        account.UpdatePassword(passwordHash);
        account.Unlock();

        await _unitOfWork.SaveChangesAsync(ct);

        var user = await _userRepository.GetByIdAsync(id, ct);
        if (user != null)
        {
            var actorName = await _notificationActorResolver.GetActorDisplayNameAsync(ct);
            await _notificationPublisher.PublishAsync(
                NotificationTriggers.UserResetPassword,
                _notificationActorResolver.BuildContext(id),
                new
                {
                    fullName = user.FullName,
                    employeeCode = user.EmployeeCode,
                    actorName,
                    userId = id
                },
                cancellationToken: ct);
        }
    }

    public async Task<IReadOnlyList<UserDepartmentDto>> GetSecondaryDepartmentsAsync(Guid userId,
        CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user == null)
            throw new NotFoundException("Không tìm thấy nhân sự.");

        var secondaryDepts = await _userDeptRepository.GetSecondaryDepartmentsByUserIdAsync(userId, ct);

        return _mapper.Map<List<UserDepartmentDto>>(secondaryDepts);
    }

    public async Task<UserDepartmentDto> AddSecondaryDepartmentAsync(Guid userId, AddUserDepartmentRequest request,
        CancellationToken ct = default)
    {
        await _addDeptValidator.ValidateAndThrowAsync(request, ct);

        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user == null)
            throw new NotFoundException("Không tìm thấy nhân sự.");

        var dept = await _departmentRepository.GetByIdAsync(request.DepartmentId, ct);
        if (dept == null || !dept.IsActive)
            throw new NotFoundException("Phòng ban không tồn tại hoặc đã bị vô hiệu hóa.");

        if (user.DepartmentId == request.DepartmentId)
            throw new ConflictException("Phòng ban này đang là phòng ban chính của nhân sự.");

        var exists = await _userDeptRepository.ExistsActiveSecondaryDepartmentAsync(userId, request.DepartmentId, ct);
        if (exists)
            throw new ConflictException("Nhân sự đang kiêm nhiệm phòng ban này rồi.");

        var userDept = UserDepartment.Create(userId, request.DepartmentId, isPrimary: false, request.StartDate,
            request.EndDate);
        await _userDeptRepository.AddAsync(userDept, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var actorName = await _notificationActorResolver.GetActorDisplayNameAsync(ct);
        await _notificationPublisher.PublishAsync(
            NotificationTriggers.UserSecondaryDeptAdd,
            _notificationActorResolver.BuildContext(userId),
            new
            {
                fullName = user.FullName,
                departmentName = dept.DepartmentName,
                actorName,
                userId,
                departmentId = dept.Id
            },
            cancellationToken: ct);

        var dto = _mapper.Map<UserDepartmentDto>(userDept);
        dto.DepartmentName = dept.DepartmentName;
        dto.DepartmentCode = dept.DepartmentCode;
        return dto;
    }

    public async Task RemoveSecondaryDepartmentAsync(Guid userId, Guid departmentId, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user == null)
            throw new NotFoundException("Không tìm thấy nhân sự.");

        var userDept = await _userDeptRepository.GetActiveSecondaryDepartmentAsync(userId, departmentId, ct);
        if (userDept == null)
            throw new NotFoundException("Không tìm thấy phòng ban kiêm nhiệm đang hoạt động của nhân sự này.");

        var dept = await _departmentRepository.GetByIdAsync(departmentId, ct);

        userDept.Terminate(DateOnly.FromDateTime(DateTime.UtcNow));
        await _unitOfWork.SaveChangesAsync(ct);

        var actorName = await _notificationActorResolver.GetActorDisplayNameAsync(ct);
        await _notificationPublisher.PublishAsync(
            NotificationTriggers.UserSecondaryDeptRemove,
            _notificationActorResolver.BuildContext(userId),
            new
            {
                fullName = user.FullName,
                departmentName = dept?.DepartmentName ?? departmentId.ToString(),
                actorName,
                userId,
                departmentId
            },
            cancellationToken: ct);
    }
}