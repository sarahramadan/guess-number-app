using System.Diagnostics;

namespace Guess.Api.Middleware;

/// <summary>
/// Middleware for comprehensive request/response logging
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString("N")[..8];

        // Log request
        _logger.LogInformation(
            "Request {RequestId} started: {Method} {Path} from {ClientIp}",
            requestId,
            context.Request.Method,
            context.Request.Path,
            GetClientIpAddress(context)
        );

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Request {RequestId} failed: {Method} {Path} - {ErrorMessage}",
                requestId,
                context.Request.Method,
                context.Request.Path,
                ex.Message
            );
            throw;
        }
        finally
        {
            stopwatch.Stop();

            // Log response
            _logger.LogInformation(
                "Request {RequestId} completed: {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                requestId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds
            );

            // Log slow requests
            if (stopwatch.ElapsedMilliseconds > 5000) // 5 seconds
            {
                _logger.LogWarning(
                    "Slow request detected {RequestId}: {Method} {Path} took {ElapsedMs}ms",
                    requestId,
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds
                );
            }
        }
    }

    private static string GetClientIpAddress(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            return context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim() ?? "unknown";
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}