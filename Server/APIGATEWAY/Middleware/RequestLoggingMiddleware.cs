using System.Diagnostics;

namespace ApiGateway.Middleware;

/// <summary>
/// Lightweight middleware that logs every upstream request flowing through the gateway.
/// </summary>
public sealed class RequestLoggingMiddleware
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
        var activity = Activity.Current ?? new Activity("GatewayRequest").Start();
        var watch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            watch.Stop();
            _logger.LogInformation(
                "Gateway {Method} {Path} responded {StatusCode} in {Elapsed} ms (TraceId: {TraceId})",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                watch.Elapsed.TotalMilliseconds,
                activity.TraceId);

            if (activity.Parent == null)
            {
                activity.Stop();
            }
        }
    }
}

public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        => app.UseMiddleware<RequestLoggingMiddleware>();
}

