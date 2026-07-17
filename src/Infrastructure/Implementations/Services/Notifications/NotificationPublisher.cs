using Microsoft.Extensions.Logging;

namespace Infrastructure;

[RegisterService(typeof(INotificationPublisher))]
public class NotificationPublisher : INotificationPublisher
{
    private readonly INotificationTriggerBindingRepository _triggerRepository;
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly IUserNotificationRepository _userNotificationRepository;
    private readonly INotificationRecipientResolver _recipientResolver;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationRealtimeSender _realtimeSender;
    private readonly IMapper _mapper;
    private readonly ILogger<NotificationPublisher> _logger;

    public NotificationPublisher(
        INotificationTriggerBindingRepository triggerRepository,
        INotificationTemplateRepository templateRepository,
        IUserNotificationRepository userNotificationRepository,
        INotificationRecipientResolver recipientResolver,
        IUnitOfWork unitOfWork,
        INotificationRealtimeSender realtimeSender,
        IMapper mapper,
        ILogger<NotificationPublisher> logger)
    {
        _triggerRepository = triggerRepository;
        _templateRepository = templateRepository;
        _userNotificationRepository = userNotificationRepository;
        _recipientResolver = recipientResolver;
        _unitOfWork = unitOfWork;
        _realtimeSender = realtimeSender;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task PublishAsync(
        string triggerKey,
        NotificationPublishContext context,
        object data,
        string? linkUrlOverride = null,
        CancellationToken cancellationToken = default)
    {
        var binding = await _triggerRepository.GetByTriggerKeyWithEventTypeAsync(triggerKey, cancellationToken);
        if (binding == null || !binding.IsActive || binding.EventTypeId == null || binding.EventType == null)
        {
            _logger.LogDebug("Skip notification for trigger {TriggerKey}: binding missing, inactive, or unassigned.",
                triggerKey);
            return;
        }

        var rules = NotificationRecipientRulesJson.Deserialize(binding.RecipientRulesJson);
        var recipientUserIds = await _recipientResolver.ResolveAsync(rules, context, cancellationToken);
        if (recipientUserIds.Count == 0)
        {
            _logger.LogDebug("Skip notification for trigger {TriggerKey}: no recipients resolved.", triggerKey);
            return;
        }

        var eventType = binding.EventType;
        if (!eventType.IsActive)
        {
            _logger.LogDebug("Skip notification for trigger {TriggerKey}: event type inactive.", triggerKey);
            return;
        }

        var template = await _templateRepository.GetByEventTypeAndChannelAsync(
            eventType.Id, NotificationChannel.InApp, cancellationToken);

        var titleTemplate = template?.TitleTemplate ?? eventType.DefaultTitleTemplate;
        var bodyTemplate = template?.BodyTemplate ?? eventType.DefaultBodyTemplate;

        var title = TemplateRenderer.Render(titleTemplate, data);
        var body = TemplateRenderer.Render(bodyTemplate, data);
        var linkUrl = linkUrlOverride;

        if (string.IsNullOrWhiteSpace(linkUrl) && !string.IsNullOrWhiteSpace(binding.LinkUrlTemplate))
            linkUrl = TemplateRenderer.Render(binding.LinkUrlTemplate, data);

        var createdNotifications = new List<UserNotification>();

        foreach (var userId in recipientUserIds.Distinct())
        {
            var notification = UserNotification.Create(
                userId,
                binding.TriggerKey,
                eventType.Id,
                title,
                body,
                linkUrl);

            await _userNotificationRepository.AddAsync(notification, cancellationToken);
            createdNotifications.Add(notification);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var notification in createdNotifications)
        {
            var unreadCount =
                await _userNotificationRepository.GetUnreadCountAsync(notification.UserId, cancellationToken);
            var dto = _mapper.Map<UserNotificationDto>(notification);
            await _realtimeSender.SendToUserAsync(
                notification.UserId,
                new RealtimeNotificationPayload
                {
                    Notification = dto,
                    UnreadCount = unreadCount
                },
                cancellationToken);
        }
    }
}