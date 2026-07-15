using FluentValidation;

namespace Infrastructure;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateRoleRequest> _updateValidator;
    private readonly INotificationPublisher _notificationPublisher;
    private readonly INotificationActorResolver _notificationActorResolver;
    private readonly IMapper _mapper;

    public RoleService(
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateRoleRequest> updateValidator,
        INotificationPublisher notificationPublisher,
        INotificationActorResolver notificationActorResolver,
        IMapper mapper)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _updateValidator = updateValidator;
        _notificationPublisher = notificationPublisher;
        _notificationActorResolver = notificationActorResolver;
        _mapper = mapper;
    }

    public async Task<RoleDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(id, ct);

        if (role == null)
            throw new NotFoundException("Không tìm thấy vai trò.");

        return _mapper.Map<RoleDto>(role);
    }

    public async Task<PaginatedResult<RoleDto>> GetPagedAsync(PaginationQuery query, CancellationToken ct = default)
    {
        var result = await _roleRepository.GetPagedWithPermissionsAsync(query, ct);
        return PaginationMapper.Map<Role, RoleDto>(result, _mapper);
    }

    public async Task<PaginatedResult<PermissionDto>> GetPagedPermissionsAsync(PaginationQuery query,
        CancellationToken ct = default)
    {
        var result = await _roleRepository.GetPagedPermissionsAsync(query, ct);
        return PaginationMapper.Map<Permission, PermissionDto>(result, _mapper);
    }

    public async Task<RoleDto> CreateAsync(CreateRoleRequest request, CancellationToken ct = default)
    {
        var roleNameUpper = request.RoleName.ToUpperInvariant();

        if (await _roleRepository.ExistsByNameAsync(roleNameUpper, ct))
            throw new ConflictException($"Tên vai trò kĩ thuật '{request.RoleName}' đã tồn tại.");

        var role = Role.Create(
            roleNameUpper,
            request.DisplayName,
            request.Description,
            isSystemRole: false, // Thao tác từ UI chỉ tạo Custom Role
            bypassDataScope: request.BypassDataScope
        );

        await _roleRepository.AddAsync(role, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var actorName = await _notificationActorResolver.GetActorDisplayNameAsync(ct);
        await _notificationPublisher.PublishAsync(
            NotificationTriggers.RoleCreate,
            _notificationActorResolver.BuildContext(),
            new
            {
                displayName = role.DisplayName,
                roleName = role.RoleName,
                actorName,
                roleId = role.Id
            },
            cancellationToken: ct);

        return await GetByIdAsync(role.Id, ct);
    }

    public async Task UpdatePermissionsAsync(Guid id, UpdateRolePermissionsRequest request,
        CancellationToken ct = default)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(id, ct);

        if (role == null)
            throw new NotFoundException("Không tìm thấy vai trò.");

        // Kiểm tra danh sách PermissionIds đầu vào
        var validPermissions = await _roleRepository.GetPermissionsByIdsAsync(request.PermissionIds, ct);

        if (validPermissions.Count != request.PermissionIds.Count)
            throw new NotFoundException("Một hoặc nhiều quyền hạn không hợp lệ hoặc đã bị vô hiệu hóa.");

        // Xóa sạch các liên kết cũ và tạo liên kết mới
        await _roleRepository.UpdateRolePermissionsAsync(role, request.PermissionIds, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        var actorName = await _notificationActorResolver.GetActorDisplayNameAsync(ct);
        await _notificationPublisher.PublishAsync(
            NotificationTriggers.RolePermissionsUpdate,
            _notificationActorResolver.BuildContext(),
            new
            {
                displayName = role.DisplayName,
                roleName = role.RoleName,
                actorName,
                roleId = role.Id
            },
            cancellationToken: ct);
    }

    public async Task<RoleDto> UpdateAsync(Guid id, UpdateRoleRequest request, CancellationToken ct = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);

        var role = await _roleRepository.GetByIdAsync(id, ct);
        if (role == null)
            throw new NotFoundException("Không tìm thấy vai trò cần cập nhật.");

        if (role.IsSystemRole && !request.IsActive)
            throw new ConflictException("Không thể vô hiệu hóa vai trò hệ thống.");

        role.Update(request.DisplayName, request.Description, request.BypassDataScope);
        role.IsActive = request.IsActive;

        await _unitOfWork.SaveChangesAsync(ct);

        var actorName = await _notificationActorResolver.GetActorDisplayNameAsync(ct);
        await _notificationPublisher.PublishAsync(
            NotificationTriggers.RoleUpdate,
            _notificationActorResolver.BuildContext(),
            new
            {
                displayName = role.DisplayName,
                roleName = role.RoleName,
                actorName,
                roleId = role.Id
            },
            cancellationToken: ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var role = await _roleRepository.GetByIdAsync(id, ct);
        if (role == null)
            throw new NotFoundException("Không tìm thấy vai trò.");

        if (role.IsSystemRole)
            throw new ConflictException("Không thể xóa vai trò hệ thống.");

        // Check if role is currently assigned to any active user
        var isUsed = await _roleRepository.HasActiveUsersInRoleAsync(id, ct);
        if (isUsed)
            throw new ConflictException("Không thể xóa vai trò đang được gán cho nhân sự hoạt động.");

        role.IsActive = false;
        await _unitOfWork.SaveChangesAsync(ct);

        var actorName = await _notificationActorResolver.GetActorDisplayNameAsync(ct);
        await _notificationPublisher.PublishAsync(
            NotificationTriggers.RoleDelete,
            _notificationActorResolver.BuildContext(),
            new
            {
                displayName = role.DisplayName,
                roleName = role.RoleName,
                actorName,
                roleId = role.Id
            },
            cancellationToken: ct);
    }
}