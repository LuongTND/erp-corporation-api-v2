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
        });

        // Đăng ký Quartz Hosted Service để chạy scheduler
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
