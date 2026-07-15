namespace Infrastructure;

public class NotificationEventTypeRepository : GenericRepository<NotificationEventType>,
    INotificationEventTypeRepository
{
    public NotificationEventTypeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PaginatedResult<NotificationEventType>> GetPagedAsync(
        PaginationQuery query,
        string? module,
        bool? isActive,
        CancellationToken ct = default)
    {
        var q = DbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(module))
            q = q.Where(x => x.Module == module);

        if (isActive.HasValue)
            q = q.Where(x => x.IsActive == isActive.Value);

        return await q
            .OrderBy(x => x.EventCode)
            .ToPaginatedResultAsync(query, ct);
    }

    public Task<NotificationEventType?> GetByCodeAsync(string eventCode, CancellationToken ct = default)
    {
        var code = eventCode.ToLowerInvariant();
        return DbSet.FirstOrDefaultAsync(x => x.EventCode == code, ct);
    }

    public Task<bool> ExistsByCodeAsync(string eventCode, CancellationToken ct = default)
    {
        var code = eventCode.ToLowerInvariant();
        return DbSet.AnyAsync(x => x.EventCode == code, ct);
    }

    public Task<bool> ExistsByCodeExcludeIdAsync(string eventCode, Guid excludeId, CancellationToken ct = default)
    {
        var code = eventCode.ToLowerInvariant();
        return DbSet.AnyAsync(x => x.EventCode == code && x.Id != excludeId, ct);
    }

    public Task<bool> HasActiveTriggerBindingsAsync(Guid eventTypeId, CancellationToken ct = default) =>
        Context.NotificationTriggerBindings.AnyAsync(x => x.EventTypeId == eventTypeId && x.IsActive, ct);
}

public class NotificationTemplateRepository : GenericRepository<NotificationTemplate>, INotificationTemplateRepository
{
    public NotificationTemplateRepository(AppDbContext context) : base(context)
    {
    }

    public Task<NotificationTemplate?> GetByEventTypeAndChannelAsync(
        Guid eventTypeId,
        NotificationChannel channel,
        CancellationToken ct = default) =>
        DbSet.FirstOrDefaultAsync(x => x.EventTypeId == eventTypeId && x.Channel == channel && x.IsActive, ct);

    public Task<NotificationTemplate?> FindByEventTypeAndChannelAsync(
        Guid eventTypeId,
        NotificationChannel channel,
        CancellationToken ct = default) =>
        DbSet.FirstOrDefaultAsync(x => x.EventTypeId == eventTypeId && x.Channel == channel, ct);

    public async Task<IReadOnlyList<NotificationTemplate>> GetByEventTypeIdAsync(Guid eventTypeId,
        CancellationToken ct = default) =>
        await DbSet.AsNoTracking()
            .Where(x => x.EventTypeId == eventTypeId)
            .OrderBy(x => x.Channel)
            .ToListAsync(ct);
}

public class NotificationTriggerBindingRepository : GenericRepository<NotificationTriggerBinding>,
    INotificationTriggerBindingRepository
{
    public NotificationTriggerBindingRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PaginatedResult<NotificationTriggerBinding>> GetPagedAsync(
        PaginationQuery query,
        string? module,
        CancellationToken ct = default)
    {
        var q = DbSet.AsNoTracking()
            .Include(x => x.EventType)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(module))
            q = q.Where(x => x.Module == module);

        return await q
            .OrderBy(x => x.Module)
            .ThenBy(x => x.TriggerKey)
            .ToPaginatedResultAsync(query, ct);
    }

    public Task<NotificationTriggerBinding?> GetByTriggerKeyAsync(string triggerKey, CancellationToken ct = default)
    {
        var key = triggerKey.ToLowerInvariant();
        return DbSet.FirstOrDefaultAsync(x => x.TriggerKey == key, ct);
    }

    public Task<NotificationTriggerBinding?> GetByTriggerKeyWithEventTypeAsync(string triggerKey,
        CancellationToken ct = default)
    {
        var key = triggerKey.ToLowerInvariant();
        return DbSet
            .Include(x => x.EventType)
            .FirstOrDefaultAsync(x => x.TriggerKey == key, ct);
    }
}

public class UserNotificationRepository : GenericRepository<UserNotification>, IUserNotificationRepository
{
    public UserNotificationRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PaginatedResult<UserNotification>> GetPagedForUserAsync(
        Guid userId,
        PaginationQuery query,
        bool? isRead,
        CancellationToken ct = default)
    {
        var q = DbSet.AsNoTracking()
            .Where(x => x.UserId == userId);

        if (isRead.HasValue)
            q = q.Where(x => x.IsRead == isRead.Value);

        return await q
            .OrderByDescending(x => x.CreatedAt)
            .ToPaginatedResultAsync(query, ct);
    }

    public Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default) =>
        DbSet.CountAsync(x => x.UserId == userId && !x.IsRead, ct);

    public Task<UserNotification?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken ct = default) =>
        DbSet.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct);

    public async Task MarkAllReadAsync(Guid userId, CancellationToken ct = default)
    {
        var unread = await DbSet
            .Where(x => x.UserId == userId && !x.IsRead)
            .ToListAsync(ct);

        var now = DateTime.UtcNow;
        foreach (var item in unread)
        {
            item.MarkRead();
        }
    }
}