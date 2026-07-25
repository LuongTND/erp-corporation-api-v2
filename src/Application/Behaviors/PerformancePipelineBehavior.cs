namespace Application;

public class PerformancePipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;
    private readonly long _thresholdMs;
    private const long DefaultThresholdMs = 500;

    public PerformancePipelineBehavior(ILogger<TRequest> logger, IConfiguration configuration)
    {
        _logger = logger;
        _thresholdMs = configuration.GetValue<long>("Application:PerformanceThresholdMs", DefaultThresholdMs);
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var timer = Stopwatch.StartNew();
        var response = await next(cancellationToken);
        timer.Stop();

        if (timer.ElapsedMilliseconds >= _thresholdMs)
            _logger.LogWarning("Slow request: {Name} ({ElapsedMilliseconds}ms) {@Request}",
                typeof(TRequest).Name, timer.ElapsedMilliseconds, request);

        return response;
    }
}
