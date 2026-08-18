namespace Infrastructure;

public sealed class SyncPosStoresJob(
    IPosStoreReader posReader,
    ApplicationDbContext db,
    INotificationRealtimeService realtime,
    ILogger<SyncPosStoresJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var ct = context.CancellationToken;

            var posStores = await posReader.GetAllStoresAsync(ct);
            var posStoreIds = posStores.Select(s => s.Id.ToString()).ToHashSet();

            var linkedPosIds = await db.Set<Store>()
                .Select(s => s.PosStoreId)
                .ToListAsync(ct);

            var newCount = posStoreIds.Except(linkedPosIds).Count();
            if (newCount == 0) return;

            logger.LogInformation("SyncPosStoresJob: phát hiện {Count} cửa hàng mới từ POS chưa import", newCount);

            var adminUserIds = await db.Set<UserRole>()
                .Where(ur => ur.Role!.RoleName == RoleConstants.Admin && ur.IsActive)
                .Select(ur => ur.UserId)
                .ToListAsync(ct);

            var payload = new
            {
                type = "POS_NEW_STORES_DETECTED",
                count = newCount,
                message = $"Phát hiện {newCount} cửa hàng mới từ POS chưa được import vào HRM"
            };

            foreach (var adminId in adminUserIds)
                await realtime.SendToUserAsync(adminId, payload, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "SyncPosStoresJob thất bại, sẽ thử lại lần sau");
        }
    }
}
