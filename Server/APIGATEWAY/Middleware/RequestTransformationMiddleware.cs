using System.Security.Claims;

namespace ApiGateway.Middleware;


/// Middleware để transform requests và add user context headers

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
            // Try to get user ID from various claim types (supports both JWT and Firebase)
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? context.User.FindFirst("firebase_uid")?.Value
                ?? context.User.FindFirst("sub")?.Value;

            // Try to get role from various claim types
            var userRole = context.User.FindFirst("RoleId")?.Value
                ?? context.User.FindFirst(ClaimTypes.Role)?.Value;

            // Try to get user name from various claim types
            var userName = context.User.FindFirst(ClaimTypes.Name)?.Value
                ?? context.User.FindFirst("name")?.Value;

            // Get email if available
            var userEmail = context.User.FindFirst(ClaimTypes.Email)?.Value
                ?? context.User.FindFirst("email")?.Value;

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

            if (!string.IsNullOrEmpty(userEmail))
            {
                context.Request.Headers["X-User-Email"] = userEmail;
            }

            // Add authentication method indicator
            var authMethod = context.User.FindFirst("firebase_uid") != null ? "Firebase" : "JWT";
            context.Request.Headers["X-Auth-Method"] = authMethod;
        }

        await _next(context);
    }
}

