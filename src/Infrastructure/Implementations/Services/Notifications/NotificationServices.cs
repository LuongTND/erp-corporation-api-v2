using Application.Common.Exceptions;
using Application.Common.Mapping;
using Application.Common.Models;
using Application.Common.Notifications;
using Application.DTOs.Notifications;
using Application.Interfaces.Services.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.Notifications;
using Application.Interfaces.Services.Notifications;
using AutoMapper;
using FluentValidation;

namespace Infrastructure.Implementations.Services.Notifications;

public class NotificationEventTypeService : INotificationEventTypeService
{
    private readonly INotificationEventTypeRepository _eventTypeRepository;
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateNotificationEventTypeRequest> _createValidator;
    private readonly IValidator<UpdateNotificationEventTypeRequest> _updateValidator;
    private readonly IValidator<UpsertNotificationTemplateRequest> _templateValidator;
    private readonly IMapper _mapper;

    public NotificationEventTypeService(
        INotificationEventTypeRepository eventTypeRepository,
        INotificationTemplateRepository templateRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateNotificationEventTypeRequest> createValidator,
        IValidator<UpdateNotificationEventTypeRequest> updateValidator,
        IValidator<UpsertNotificationTemplateRequest> templateValidator,
        IMapper mapper)
    {
        _eventTypeRepository = eventTypeRepository;
        _templateRepository = templateRepository;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _templateValidator = templateValidator;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<NotificationEventTypeDto>> GetPagedAsync(PaginationQuery query, CancellationToken ct = default)
    {
        var result = await _eventTypeRepository.GetPagedAsync(query, module: null, isActive: null, ct);
        return PaginationMapper.Map<NotificationEventType, NotificationEventTypeDto>(result, _mapper);
    }

    public async Task<NotificationEventTypeDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _eventTypeRepository.GetByIdAsync(id, ct);
        if (entity == null)
            throw new NotFoundException("Không tìm thấy loại thông báo.");

        return _mapper.Map<NotificationEventTypeDto>(entity);
    }

    public async Task<NotificationEventTypeDto> CreateAsync(CreateNotificationEventTypeRequest request, CancellationToken ct = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);

        var code = request.EventCode.Trim().ToLowerInvariant();
        if (await _eventTypeRepository.ExistsByCodeAsync(code, ct))
            throw new ConflictException($"Mã loại thông báo '{code}' đã tồn tại.");

        var entity = NotificationEventType.Create(
            Guid.NewGuid(),
            code,
            request.Name.Trim(),
            request.Module.Trim(),
            request.DefaultTitleTemplate.Trim(),
            request.DefaultBodyTemplate.Trim(),
            request.Description?.Trim());

        await _eventTypeRepository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<NotificationEventTypeDto>(entity);
    }

    public async Task<NotificationEventTypeDto> UpdateAsync(Guid id, UpdateNotificationEventTypeRequest request, CancellationToken ct = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);

        var entity = await _eventTypeRepository.GetByIdAsync(id, ct);
        if (entity == null)
            throw new NotFoundException("Không tìm thấy loại thông báo cần cập nhật.");

        entity.Update(
            request.Name.Trim(),
            request.Module.Trim(),
            request.DefaultTitleTemplate.Trim(),
            request.DefaultBodyTemplate.Trim(),
            request.Description?.Trim(),
            request.IsActive);

        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<NotificationEventTypeDto>(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _eventTypeRepository.GetByIdAsync(id, ct);
        if (entity == null)
            throw new NotFoundException("Không tìm thấy loại thông báo.");

        var hasBindings = await _eventTypeRepository.HasActiveTriggerBindingsAsync(id, ct);
        if (hasBindings)
            throw new ConflictException("Không thể vô hiệu hóa loại thông báo đang được gán cho chức năng.");

        entity.IsActive = false;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<NotificationTemplateDto>> GetTemplatesAsync(Guid eventTypeId, CancellationToken ct = default)
    {
        var entity = await _eventTypeRepository.GetByIdAsync(eventTypeId, ct);
        if (entity == null)
            throw new NotFoundException("Không tìm thấy loại thông báo.");

        var templates = await _templateRepository.GetByEventTypeIdAsync(eventTypeId, ct);
        return _mapper.Map<List<NotificationTemplateDto>>(templates);
    }

    public async Task<NotificationTemplateDto> UpsertTemplateAsync(
        Guid eventTypeId,
        NotificationChannel channel,
        UpsertNotificationTemplateRequest request,
        CancellationToken ct = default)
    {
        await _templateValidator.ValidateAndThrowAsync(request, ct);

        var entity = await _eventTypeRepository.GetByIdAsync(eventTypeId, ct);
        if (entity == null)
            throw new NotFoundException("Không tìm thấy loại thông báo.");

        var template = await _templateRepository.FindByEventTypeAndChannelAsync(eventTypeId, channel, ct);
        if (template == null)
        {
            template = NotificationTemplate.Create(
                eventTypeId,
                channel,
                request.TitleTemplate.Trim(),
                request.BodyTemplate.Trim());
            template.IsActive = request.IsActive;
            await _templateRepository.AddAsync(template, ct);
        }
        else
        {
            template.Update(request.TitleTemplate.Trim(), request.BodyTemplate.Trim(), request.IsActive);
        }

        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<NotificationTemplateDto>(template);
    }
}

public class NotificationTriggerService : INotificationTriggerService
{
    private readonly INotificationTriggerBindingRepository _triggerRepository;
    private readonly INotificationEventTypeRepository _eventTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateNotificationTriggerBindingRequest> _updateValidator;
    private readonly IMapper _mapper;

    public NotificationTriggerService(
        INotificationTriggerBindingRepository triggerRepository,
        INotificationEventTypeRepository eventTypeRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateNotificationTriggerBindingRequest> updateValidator,
        IMapper mapper)
    {
        _triggerRepository = triggerRepository;
        _eventTypeRepository = eventTypeRepository;
        _unitOfWork = unitOfWork;
        _updateValidator = updateValidator;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<NotificationTriggerBindingDto>> GetPagedAsync(
        PaginationQuery query,
        string? module,
        CancellationToken ct = default)
    {
        var result = await _triggerRepository.GetPagedAsync(query, module, ct);
        return PaginationMapper.Map<NotificationTriggerBinding, NotificationTriggerBindingDto>(result, _mapper);
    }

    public async Task<NotificationTriggerBindingDto> GetByTriggerKeyAsync(string triggerKey, CancellationToken ct = default)
    {
        var entity = await _triggerRepository.GetByTriggerKeyWithEventTypeAsync(triggerKey, ct);
        if (entity == null)
            throw new NotFoundException("Không tìm thấy điểm kích hoạt thông báo.");

        return _mapper.Map<NotificationTriggerBindingDto>(entity);
    }

    public async Task<NotificationTriggerBindingDto> UpdateBindingAsync(
        string triggerKey,
        UpdateNotificationTriggerBindingRequest request,
        CancellationToken ct = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);

        var entity = await _triggerRepository.GetByTriggerKeyWithEventTypeAsync(triggerKey, ct);
        if (entity == null)
            throw new NotFoundException("Không tìm thấy điểm kích hoạt thông báo.");

        if (request.EventTypeId.HasValue)
        {
            var eventType = await _eventTypeRepository.GetByIdAsync(request.EventTypeId.Value, ct);
            if (eventType == null || !eventType.IsActive)
                throw new NotFoundException("Loại thông báo không tồn tại hoặc đã bị vô hiệu hóa.");
        }

        entity.AssignEventType(
            request.EventTypeId,
            request.LinkUrlTemplate?.Trim(),
            request.IsActive,
            request.RecipientRules != null
                ? NotificationRecipientRulesJson.Serialize(NormalizeRecipientRules(request.RecipientRules))
                : entity.RecipientRulesJson);

        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _triggerRepository.GetByTriggerKeyWithEventTypeAsync(triggerKey, ct);
        return _mapper.Map<NotificationTriggerBindingDto>(updated!);
    }

    private static NotificationRecipientRulesDto NormalizeRecipientRules(NotificationRecipientRulesDto rules) =>
        new()
        {
            IncludeSubjectUser = rules.IncludeSubjectUser,
            IncludeActor = rules.IncludeActor,
            IncludeSuperAdmins = rules.IncludeSuperAdmins,
            IncludeDepartmentManager = rules.IncludeDepartmentManager,
            RoleIds = rules.RoleIds.Distinct().ToList(),
            UserIds = rules.UserIds.Distinct().ToList()
        };
}

public class UserNotificationService : IUserNotificationService
{
    private readonly IUserNotificationRepository _userNotificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UserNotificationService(
        IUserNotificationRepository userNotificationRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _userNotificationRepository = userNotificationRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<UserNotificationDto>> GetMyPagedAsync(
        PaginationQuery query,
        bool? isRead,
        CancellationToken ct = default)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");

        var result = await _userNotificationRepository.GetPagedForUserAsync(userId, query, isRead, ct);
        return PaginationMapper.Map<UserNotification, UserNotificationDto>(result, _mapper);
    }

    public async Task<UnreadNotificationCountDto> GetMyUnreadCountAsync(CancellationToken ct = default)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");

        var count = await _userNotificationRepository.GetUnreadCountAsync(userId, ct);
        return new UnreadNotificationCountDto { Count = count };
    }

    public async Task<UserNotificationDto> MarkReadAsync(Guid id, CancellationToken ct = default)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");

        var notification = await _userNotificationRepository.GetByIdForUserAsync(id, userId, ct);
        if (notification == null)
            throw new NotFoundException("Không tìm thấy thông báo.");

        notification.MarkRead();
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<UserNotificationDto>(notification);
    }

    public async Task MarkAllReadAsync(CancellationToken ct = default)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");

        await _userNotificationRepository.MarkAllReadAsync(userId, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
