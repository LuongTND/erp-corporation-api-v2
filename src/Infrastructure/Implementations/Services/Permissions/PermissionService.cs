using FluentValidation;

namespace Infrastructure;

[RegisterService(typeof(IPermissionService))]
public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreatePermissionRequest> _createValidator;
    private readonly IValidator<UpdatePermissionRequest> _updateValidator;
    private readonly INotificationPublisher _notificationPublisher;
    private readonly INotificationActorResolver _notificationActorResolver;
    private readonly IMapper _mapper;

    public PermissionService(
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreatePermissionRequest> createValidator,
        IValidator<UpdatePermissionRequest> updateValidator,
        INotificationPublisher notificationPublisher,
        INotificationActorResolver notificationActorResolver,
        IMapper mapper)
    {
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _notificationPublisher = notificationPublisher;
        _notificationActorResolver = notificationActorResolver;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<PermissionDto>> GetPagedAsync(PaginationQuery query,
        CancellationToken ct = default)
    {
        var result = await _permissionRepository.GetPagedAsync(query, ct);
        return PaginationMapper.Map<Permission, PermissionDto>(result, _mapper);
    }

    public async Task<PermissionDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var permission = await _permissionRepository.GetByIdAsync(id, ct);
        if (permission == null)
            throw new NotFoundException("Không tìm thấy quyền.");

        return _mapper.Map<PermissionDto>(permission);
    }

    public async Task<PermissionDto> CreateAsync(CreatePermissionRequest request, CancellationToken ct = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);

        var code = request.PermissionCode.Trim().ToLowerInvariant();
        if (await _permissionRepository.ExistsByCodeAsync(code, ct))
            throw new ConflictException($"Mã quyền '{code}' đã tồn tại.");

        var permission = Permission.Create(
            code,
            request.PermissionName.Trim(),
            request.Module,
            request.Action,
            request.Resource.Trim(),
            request.Description?.Trim());

        await _permissionRepository.AddAsync(permission, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var actorName = await _notificationActorResolver.GetActorDisplayNameAsync(ct);
        await _notificationPublisher.PublishAsync(
            NotificationTriggers.PermissionCreate,
            _notificationActorResolver.BuildContext(),
            new
            {
                permissionName = permission.PermissionName,
                permissionCode = permission.PermissionCode,
                actorName,
                permissionId = permission.Id
            },
            cancellationToken: ct);

        return _mapper.Map<PermissionDto>(permission);
    }

    public async Task<PermissionDto> UpdateAsync(Guid id, UpdatePermissionRequest request,
        CancellationToken ct = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);

        var permission = await _permissionRepository.GetByIdAsync(id, ct);
        if (permission == null)
            throw new NotFoundException("Không tìm thấy quyền cần cập nhật.");

        if (!request.IsActive && permission.IsActive)
        {
            var isAssigned = await _permissionRepository.HasActiveRoleAssignmentsAsync(id, ct);
            if (isAssigned)
                throw new ConflictException("Không thể vô hiệu hóa quyền đang được gán cho vai trò hoạt động.");
        }

        permission.Update(request.PermissionName.Trim(), request.Description?.Trim(), request.IsActive);
        await _unitOfWork.SaveChangesAsync(ct);

        var actorName = await _notificationActorResolver.GetActorDisplayNameAsync(ct);
        await _notificationPublisher.PublishAsync(
            NotificationTriggers.PermissionUpdate,
            _notificationActorResolver.BuildContext(),
            new
            {
                permissionName = permission.PermissionName,
                permissionCode = permission.PermissionCode,
                actorName,
                permissionId = permission.Id
            },
            cancellationToken: ct);

        return _mapper.Map<PermissionDto>(permission);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var permission = await _permissionRepository.GetByIdAsync(id, ct);
        if (permission == null)
            throw new NotFoundException("Không tìm thấy quyền.");

        var isAssigned = await _permissionRepository.HasActiveRoleAssignmentsAsync(id, ct);
        if (isAssigned)
            throw new ConflictException("Không thể vô hiệu hóa quyền đang được gán cho vai trò hoạt động.");

        permission.IsActive = false;
        await _unitOfWork.SaveChangesAsync(ct);

        var actorName = await _notificationActorResolver.GetActorDisplayNameAsync(ct);
        await _notificationPublisher.PublishAsync(
            NotificationTriggers.PermissionDelete,
            _notificationActorResolver.BuildContext(),
            new
            {
                permissionName = permission.PermissionName,
                permissionCode = permission.PermissionCode,
                actorName,
                permissionId = permission.Id
            },
            cancellationToken: ct);
    }
}