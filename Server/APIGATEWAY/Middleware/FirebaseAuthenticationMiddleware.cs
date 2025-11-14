using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using ApiGateway.Services;

namespace ApiGateway.Middleware;

/// Middleware để xử lý Firebase Authentication
public class FirebaseAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<FirebaseAuthenticationMiddleware> _logger;

    public FirebaseAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<FirebaseAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IFirebaseAuthService firebaseAuthService)
    {
        // Skip if already authenticated
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            await _next(context);
            return;
        }

        // Try to get Firebase token from Authorization header
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            
            // Try to verify as Firebase token
            var firebasePrincipal = await firebaseAuthService.VerifyTokenAsync(token);
            if (firebasePrincipal != null)
            {
                context.User = firebasePrincipal;
                _logger.LogDebug("Firebase token authenticated successfully for user: {UserId}", 
                    firebasePrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }
        }

        await _next(context);
    }
}

