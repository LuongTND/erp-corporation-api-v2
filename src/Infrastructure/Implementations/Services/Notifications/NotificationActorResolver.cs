namespace Infrastructure;

[RegisterService(typeof(INotificationActorResolver))]
public class NotificationActorResolver : INotificationActorResolver
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;

    public NotificationActorResolver(ICurrentUserService currentUserService, IUserRepository userRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
    }

    public async Task<string> GetActorDisplayNameAsync(CancellationToken ct = default)
    {
        if (!_currentUserService.UserId.HasValue)
            return "Hệ thống";

        var user = await _userRepository.GetByIdAsync(_currentUserService.UserId.Value, ct);
        return user?.FullName ?? "Hệ thống";
    }

    public NotificationPublishContext BuildContext(Guid? subjectUserId = null) => new()
    {
        SubjectUserId = subjectUserId,
        ActorUserId = _currentUserService.UserId
    };
}