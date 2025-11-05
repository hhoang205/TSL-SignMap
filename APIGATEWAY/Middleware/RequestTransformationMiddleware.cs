using System.Security.Claims;

namespace ApiGateway.Middleware;

/// <summary>
/// Middleware để transform requests và add user context headers
/// </summary>
public class RequestTransformationMiddleware
{
    private readonly RequestDelegate _next;

    public RequestTransformationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add user context headers if authenticated
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = context.User.FindFirst("RoleId")?.Value; // Assuming RoleId is in claims
            var userName = context.User.FindFirst(ClaimTypes.Name)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                context.Request.Headers["X-User-Id"] = userId;
            }

            if (!string.IsNullOrEmpty(userRole))
            {
                context.Request.Headers["X-User-Role"] = userRole;
            }

            if (!string.IsNullOrEmpty(userName))
            {
                context.Request.Headers["X-User-Name"] = userName;
            }
        }

        await _next(context);
    }
}

