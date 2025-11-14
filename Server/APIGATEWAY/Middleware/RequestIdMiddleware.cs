using System.Diagnostics;

namespace ApiGateway.Middleware;


/// Middleware để generate và forward request ID cho correlation tracking

public class RequestIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string RequestIdHeader = "X-Request-Id";
    private const string RequestIdKey = "RequestId";

    public RequestIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Generate or extract request ID
        var requestId = context.Request.Headers[RequestIdHeader].FirstOrDefault() 
            ?? Activity.Current?.Id 
            ?? Guid.NewGuid().ToString();

        // Add to context items for logging
        context.Items[RequestIdKey] = requestId;

        // Add to response headers
        context.Response.Headers[RequestIdHeader] = requestId;

        // Forward to backend services via headers
        context.Request.Headers[RequestIdHeader] = requestId;

        await _next(context);
    }
}

