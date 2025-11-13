using System.Collections.Concurrent;
using System.Net;

namespace Guess.Api.Middleware;

/// <summary>
/// Simple rate limiting middleware to prevent abuse
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private static readonly ConcurrentDictionary<string, ClientRequestInfo> _clients = new();

    // Rate limiting configuration
    private readonly int _requestsPerMinute = 60;
    private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1);

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = GetClientIpAddress(context);
        var key = $"{clientIp}:{context.Request.Path}";

        var clientInfo = _clients.GetOrAdd(key, _ => new ClientRequestInfo());

        lock (clientInfo)
        {
            var now = DateTime.UtcNow;

            // Clean up old requests
            clientInfo.RequestTimes.RemoveAll(time => now - time > _timeWindow);

            // Check if rate limit exceeded
            if (clientInfo.RequestTimes.Count >= _requestsPerMinute)
            {
                _logger.LogWarning("Rate limit exceeded for client {ClientIp} on path {Path}", clientIp, context.Request.Path);
                
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.Headers["Retry-After"] = _timeWindow.TotalSeconds.ToString();
                return;
            }

            // Add current request
            clientInfo.RequestTimes.Add(now);
        }

        await _next(context);
    }

    private static string GetClientIpAddress(HttpContext context)
    {
        // Check for forwarded header first (for load balancers/proxies)
        if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            return context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim() ?? "unknown";
        }

        if (context.Request.Headers.ContainsKey("X-Real-IP"))
        {
            return context.Request.Headers["X-Real-IP"].FirstOrDefault() ?? "unknown";
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private class ClientRequestInfo
    {
        public List<DateTime> RequestTimes { get; } = new();
    }
}