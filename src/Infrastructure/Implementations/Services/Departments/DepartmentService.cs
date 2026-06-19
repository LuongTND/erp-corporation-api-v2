using Application.Common.Exceptions;
using Application.Common.Mapping;
using Application.Common.Models;
using Application.Constants;
using Application.DTOs.Departments;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.Departments;
using Application.Interfaces.Repositories.Users;
using Application.Interfaces.Services.Departments;
using Application.Interfaces.Services.Notifications;
using AutoMapper;

namespace Infrastructure.Implementations.Services.Departments;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserDepartmentRepository _userDeptRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationPublisher _notificationPublisher;
    private readonly INotificationActorResolver _notificationActorResolver;
    private readonly IMapper _mapper;

    public DepartmentService(
        IDepartmentRepository departmentRepository,
        IUserRepository userRepository,
        IUserDepartmentRepository userDeptRepository,
        IUnitOfWork unitOfWork,
        INotificationPublisher notificationPublisher,
        INotificationActorResolver notificationActorResolver,
        IMapper mapper)
    {
        _departmentRepository = departmentRepository;
        _userRepository = userRepository;
        _userDeptRepository = userDeptRepository;
        _unitOfWork = unitOfWork;
        _notificationPublisher = notificationPublisher;
        _notificationActorResolver = notificationActorResolver;
        _mapper = mapper;
    }

    public async Task<DepartmentDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var dept = await _departmentRepository.GetByIdWithDetailsAsync(id, ct);

        if (dept == null)
            throw new NotFoundException("Không tìm thấy phòng ban.");

        return _mapper.Map<DepartmentDto>(dept);
    }

    public async Task<PaginatedResult<DepartmentDto>> GetPagedAsync(PaginationQuery query, CancellationToken ct = default)
    {
        var result = await _departmentRepository.GetPagedWithDetailsAsync(query, ct);
        return PaginationMapper.Map<Department, DepartmentDto>(result, _mapper);
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentRequest request, CancellationToken ct = default)
    {
        var codeUpper = request.DepartmentCode.ToUpperInvariant();

        if (await _departmentRepository.ExistsByCodeAsync(codeUpper, ct))
            throw new ConflictException($"Mã phòng ban '{request.DepartmentCode}' đã tồn tại.");

        if (request.ParentDepartmentId.HasValue)
        {
            var parent = await _departmentRepository.GetByIdAsync(request.ParentDepartmentId.Value, ct);
            if (parent == null || !parent.IsActive)
                throw new NotFoundException("Phòng ban cha không tồn tại hoặc đã bị vô hiệu hóa.");
        }

        if (request.ManagerId.HasValue)
        {
            var manager = await _userRepository.GetByIdAsync(request.ManagerId.Value, ct);
            if (manager == null || !manager.IsActive)
                throw new NotFoundException("Nhân sự quản lý không tồn tại hoặc đã nghỉ việc.");
        }

        var dept = Department.Create(
            request.DepartmentName,
            codeUpper,
            request.ParentDepartmentId,
            request.ManagerId,
            request.Description,
            request.CheckInTimeTarget,
            request.CheckOutTimeTarget
        );

        await _departmentRepository.AddAsync(dept, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var actorName = await _notificationActorResolver.GetActorDisplayNameAsync(ct);
        await _notificationPublisher.PublishAsync(
            NotificationTriggers.DepartmentCreate,
            _notificationActorResolver.BuildContext(),
            new
            {
                departmentName = dept.DepartmentName,
                departmentCode = dept.DepartmentCode,
                actorName,
                departmentId = dept.Id
            },
            cancellationToken: ct);

        return await GetByIdAsync(dept.Id, ct);
    }

    public async Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentRequest request, CancellationToken ct = default)
    {
        var dept = await _departmentRepository.GetByIdAsync(id, ct);
        if (dept == null)
            throw new NotFoundException("Không tìm thấy phòng ban cần cập nhật.");

        var codeUpper = request.DepartmentCode.ToUpperInvariant();
        if (dept.DepartmentCode != codeUpper && await _departmentRepository.ExistsByCodeExcludeIdAsync(codeUpper, id, ct))
            throw new ConflictException($"Mã phòng ban '{request.DepartmentCode}' đã tồn tại.");

        if (request.ParentDepartmentId.HasValue)
        {
            var parent = await _departmentRepository.GetByIdAsync(request.ParentDepartmentId.Value, ct);
            if (parent == null || !parent.IsActive)
                throw new NotFoundException("Phòng ban cha không tồn tại hoặc đã bị vô hiệu hóa.");

            await EnsureNoDepartmentCycleAsync(id, request.ParentDepartmentId.Value, ct);
        }

        if (request.ManagerId.HasValue)
        {
            var manager = await _userRepository.GetByIdAsync(request.ManagerId.Value, ct);
            if (manager == null || !manager.IsActive)
                throw new NotFoundException("Nhân sự quản lý không tồn tại hoặc đã nghỉ việc.");
        }

        dept.Update(
            request.DepartmentName,
            codeUpper,
            request.ParentDepartmentId,
            request.ManagerId,
            request.Description,
            request.CheckInTimeTarget,
            request.CheckOutTimeTarget
        );
        dept.IsActive = request.IsActive;

        await _unitOfWork.SaveChangesAsync(ct);

        var actorName = await _notificationActorResolver.GetActorDisplayNameAsync(ct);
        await _notificationPublisher.PublishAsync(
            NotificationTriggers.DepartmentUpdate,
            _notificationActorResolver.BuildContext(),
            new
            {
                departmentName = dept.DepartmentName,
                departmentCode = dept.DepartmentCode,
                actorName,
                departmentId = dept.Id
            },
            cancellationToken: ct);

        return await GetByIdAsync(id, ct);
    }

    private async Task EnsureNoDepartmentCycleAsync(Guid departmentId, Guid newParentId, CancellationToken ct)
    {
        if (departmentId == newParentId)
            throw new ConflictException("Không thể chọn phòng ban cha là chính nó.");

        var currentParentId = newParentId;
        while (currentParentId != Guid.Empty)
        {
            if (currentParentId == departmentId)
                throw new ConflictException("Tạo chu trình phân cấp: Phòng ban cha được chọn đang là con hoặc cháu của phòng ban hiện tại.");

            var parent = await _departmentRepository.GetByIdAsync(currentParentId, ct);
            currentParentId = parent?.ParentDepartmentId ?? Guid.Empty;
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var dept = await _departmentRepository.GetByIdAsync(id, ct);
        if (dept == null)
            throw new NotFoundException("Không tìm thấy phòng ban.");

        var hasActiveUsers = await _userRepository.HasActiveUsersInDepartmentAsync(id, ct);
        if (hasActiveUsers)
            throw new ConflictException("Không thể xóa phòng ban đang có nhân sự hoạt động.");

        var hasSecondaryUsers = await _userDeptRepository.HasActiveSecondaryUsersInDepartmentAsync(id, ct);
        if (hasSecondaryUsers)
            throw new ConflictException("Không thể xóa phòng ban đang có nhân sự kiêm nhiệm hoạt động.");

        var hasChildDepts = await _departmentRepository.HasActiveChildDepartmentsAsync(id, ct);
        if (hasChildDepts)
            throw new ConflictException("Không thể xóa phòng ban đang có phòng ban con hoạt động.");

        dept.IsActive = false;
        await _unitOfWork.SaveChangesAsync(ct);

        var actorName = await _notificationActorResolver.GetActorDisplayNameAsync(ct);
        await _notificationPublisher.PublishAsync(
            NotificationTriggers.DepartmentDelete,
            _notificationActorResolver.BuildContext(),
            new
            {
                departmentName = dept.DepartmentName,
                departmentCode = dept.DepartmentCode,
                actorName,
                departmentId = dept.Id
            },
            cancellationToken: ct);
    }
}
