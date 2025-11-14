using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// Refresh access token using refresh token

    [HttpPost("refresh")]
    [AllowAnonymous]
    public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new { error = "Refresh token is required" });
            }

            // Validate refresh token (simplified - in production, validate against database)
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _configuration["Gateway:Jwt:SecretKey"] 
                ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var key = Encoding.UTF8.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Gateway:Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Gateway:Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(request.RefreshToken, validationParameters, out validatedToken);

            // Generate new access token
            var newToken = GenerateAccessToken(principal.Claims);

            return Ok(new
            {
                accessToken = newToken,
                tokenType = "Bearer",
                expiresIn = 60 * 60 * 24 * 7 // 7 days in seconds
            });
        }
        catch (SecurityTokenExpiredException)
        {
            return Unauthorized(new { error = "Refresh token has expired" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return Unauthorized(new { error = "Invalid refresh token" });
        }
    }

    private string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secretKey = _configuration["Gateway:Jwt:SecretKey"] 
            ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var key = Encoding.UTF8.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["Gateway:Jwt:ExpirationMinutes"] ?? "10080")), // Default 7 days
            Issuer = _configuration["Gateway:Jwt:Issuer"],
            Audience = _configuration["Gateway:Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

