namespace API;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ApiResponse<object> response;

        if (exception is AppException appEx)
        {
            if (appEx.StatusCode >= 500)
                _logger.LogError(exception, "Server error: {Message}", appEx.Message);
            else
                _logger.LogWarning("Client error {StatusCode}: {Message}", appEx.StatusCode, appEx.Message);

            httpContext.Response.StatusCode = appEx.StatusCode;
            response = ApiResponse<object>.Fail(appEx.Message, appEx.StatusCode, appEx.Errors);
        }
        else if (exception is FluentValidation.ValidationException validationEx)
        {
            _logger.LogWarning("Validation failed: {Errors}", validationEx.Message);

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            var errors = validationEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            response = ApiResponse<object>.Fail("Validation failed", StatusCodes.Status400BadRequest, errors);
        }
        else
        {
            _logger.LogError(exception, "Unhandled exception");
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            response = ApiResponse<object>.Fail(ExceptionMessages.InternalError, StatusCodes.Status500InternalServerError);
        }

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}