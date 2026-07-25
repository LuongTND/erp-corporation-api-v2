namespace Infrastructure;

public sealed class OutboxProcessorHostedService(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxProcessorHostedService> logger) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);
    private const int BatchSize = 20;
    private const int MaxRetries = 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Outbox processor batch failed.");
            }

            await Task.Delay(PollInterval, stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var pending = await db.OutboxMessages
            .Where(m => m.ProcessedAt == null && m.RetryCount < MaxRetries)
            .OrderBy(m => m.CreatedAt)
            .Take(BatchSize)
            .ToListAsync(ct);

        foreach (var message in pending)
        {
            try
            {
                var notification = OutboxSerializer.Deserialize(message.Type, message.Payload);
                await mediator.Publish(notification, ct);

                message.ProcessedAt = DateTimeOffset.UtcNow;
                message.Error = null;
            }
            catch (Exception ex)
            {
                message.RetryCount++;
                message.Error = ex.Message.Length > 4000 ? ex.Message[..4000] : ex.Message;
                logger.LogWarning(ex, "Outbox message {Id} failed (retry {Retry})", message.Id, message.RetryCount);
            }
        }

        if (pending.Count > 0)
            await db.SaveChangesAsync(ct);
    }
}
