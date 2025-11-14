using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiGateway.Filters;


/// Custom authorization attribute để check roles

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AuthorizeRoleAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string[] _allowedRoles;

    public AuthorizeRoleAttribute(params string[] allowedRoles)
    {
        _allowedRoles = allowedRoles;
        Roles = string.Join(",", allowedRoles);
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Check if user has any of the required roles
        var hasRole = _allowedRoles.Any(role => 
            user.IsInRole(role) || 
            user.HasClaim("RoleId", role) ||
            user.HasClaim(ClaimTypes.Role, role));

        if (!hasRole)
        {
            context.Result = new ForbidResult();
        }
    }
}

// Helper extension để check claims
internal static class ClaimsPrincipalExtensions
{
    internal static bool HasClaim(this ClaimsPrincipal user, string claimType, string claimValue)
    {
        return user.HasClaim(claimType, claimValue);
    }
}

