namespace API;

[Route("api/notification-triggers")]
public class NotificationTriggersController : BaseApiController
{
    private readonly INotificationTriggerService _triggerService;

    public NotificationTriggersController(INotificationTriggerService triggerService)
    {
        _triggerService = triggerService;
    }

    [HttpGet]
    [AuthorizePermission("system.notification.trigger.read")]
    public async Task<ActionResult<PaginatedResult<NotificationTriggerBindingDto>>> GetAll(
        [FromQuery] PaginationQuery query,
        [FromQuery] string? module,
        CancellationToken ct)
    {
        var result = await _triggerService.GetPagedAsync(query, module, ct);
        return Ok(result);
    }

    [HttpGet("{triggerKey}")]
    [AuthorizePermission("system.notification.trigger.read")]
    public async Task<ActionResult<NotificationTriggerBindingDto>> GetByKey(string triggerKey, CancellationToken ct)
    {
        var result = await _triggerService.GetByTriggerKeyAsync(triggerKey, ct);
        return Ok(result);
    }

    [HttpPut("{triggerKey}")]
    [AuthorizePermission("system.notification.trigger.update")]
    public async Task<ActionResult<NotificationTriggerBindingDto>> UpdateBinding(
        string triggerKey,
        [FromBody] UpdateNotificationTriggerBindingRequest request,
        CancellationToken ct)
    {
        var result = await _triggerService.UpdateBindingAsync(triggerKey, request, ct);
        return Ok(result);
    }
}