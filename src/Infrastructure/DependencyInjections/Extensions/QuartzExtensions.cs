namespace Infrastructure;

internal static class QuartzExtensions
{
    public static IServiceCollection AddQuartzService(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.SchedulerId = "AUTO";
            q.SchedulerName = "BaHungERPScheduler";
            q.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 5; });
            q.UseInMemoryStore();

            // Sync store mới từ POS lúc 2:00 AM mỗi đêm
            var jobKey = new JobKey(nameof(SyncPosStoresJob));
            q.AddJob<SyncPosStoresJob>(opts => opts.WithIdentity(jobKey).StoreDurably());
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity($"{nameof(SyncPosStoresJob)}-trigger")
                .WithCronSchedule("0 0 2 * * ?"));
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
