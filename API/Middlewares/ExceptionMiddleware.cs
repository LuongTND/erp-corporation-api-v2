using Application.Common.Exceptions;
using Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail) = exception switch
        {
            ValidationException validation => (
                StatusCodes.Status400BadRequest,
                "Validation failed",
                string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))),
            NotFoundException notFound => (StatusCodes.Status404NotFound, "Not found", notFound.Message),
            ConflictException conflict => (StatusCodes.Status409Conflict, "Conflict", conflict.Message),
            DomainException domain => (StatusCodes.Status400BadRequest, "Domain error", domain.Message),
            _ => (StatusCodes.Status500InternalServerError, "Server error", "An unexpected error occurred.")
        };

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    }
}
