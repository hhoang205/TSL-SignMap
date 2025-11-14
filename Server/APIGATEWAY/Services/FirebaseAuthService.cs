using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using ApiGateway.Configuration;
using System.Security.Claims;

namespace ApiGateway.Services;

/// <summary>
/// Service để xử lý Firebase Authentication
/// </summary>
public interface IFirebaseAuthService
{
    /// <summary>
    /// Verify Firebase ID token và trả về claims
    /// </summary>
    Task<ClaimsPrincipal?> VerifyTokenAsync(string idToken);
    
    /// <summary>
    /// Initialize Firebase Admin SDK
    /// </summary>
    Task InitializeAsync();
}

public class FirebaseAuthService : IFirebaseAuthService
{
    private readonly FirebaseConfig _config;
    private readonly ILogger<FirebaseAuthService> _logger;
    private bool _initialized = false;

    public FirebaseAuthService(
        IOptions<GatewayConfig> gatewayConfig,
        ILogger<FirebaseAuthService> logger)
    {
        _config = gatewayConfig.Value.Firebase;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        if (_initialized || !_config.Enabled)
        {
            return;
        }

        try
        {
            if (string.IsNullOrEmpty(_config.ProjectId))
            {
                throw new InvalidOperationException("Firebase ProjectId must be configured");
            }

            // Check if FirebaseApp already exists
            if (FirebaseApp.DefaultInstance == null)
            {
                var options = new AppOptions
                {
                    ProjectId = _config.ProjectId
                };

                // Try to load service account credentials
                if (!string.IsNullOrEmpty(_config.ServiceAccountPath))
                {
                    if (File.Exists(_config.ServiceAccountPath))
                    {
                        options.Credential = GoogleCredential.FromFile(_config.ServiceAccountPath);
                        _logger.LogInformation("Firebase initialized with service account file: {Path}", _config.ServiceAccountPath);
                    }
                    else
                    {
                        _logger.LogWarning("Firebase service account file not found: {Path}. Using default credentials.", _config.ServiceAccountPath);
                    }
                }
                else
                {
                    // Try environment variable
                    var credentialsJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS");
                    if (!string.IsNullOrEmpty(credentialsJson))
                    {
                        options.Credential = GoogleCredential.FromJson(credentialsJson);
                        _logger.LogInformation("Firebase initialized with credentials from FIREBASE_CREDENTIALS environment variable");
                    }
                    else
                    {
                        // Use default credentials (for GCP environments)
                        _logger.LogInformation("Firebase initialized with default credentials (ProjectId: {ProjectId})", _config.ProjectId);
                    }
                }

                FirebaseApp.Create(options);
            }

            _initialized = true;
            _logger.LogInformation("Firebase Admin SDK initialized successfully for project: {ProjectId}", _config.ProjectId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Firebase Admin SDK");
            throw;
        }

        await Task.CompletedTask;
    }

    public async Task<ClaimsPrincipal?> VerifyTokenAsync(string idToken)
    {
        if (!_config.Enabled || !_initialized)
        {
            return null;
        }

        try
        {
            var auth = FirebaseAuth.DefaultInstance;
            var decodedToken = await auth.VerifyIdTokenAsync(idToken);

            // Create claims from Firebase token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, decodedToken.Uid),
                new Claim(ClaimTypes.Name, decodedToken.Claims.GetValueOrDefault("name", decodedToken.Uid)?.ToString() ?? decodedToken.Uid),
                new Claim(ClaimTypes.Email, decodedToken.Claims.GetValueOrDefault("email", "")?.ToString() ?? ""),
            };

            // Add email verified claim
            if (decodedToken.Claims.TryGetValue("email_verified", out var emailVerified))
            {
                claims.Add(new Claim("email_verified", emailVerified?.ToString() ?? "false"));
            }

            // Add custom claims if present
            if (decodedToken.Claims.TryGetValue("role", out var role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role?.ToString() ?? "User"));
            }

            // Add Firebase-specific claims
            claims.Add(new Claim("firebase_uid", decodedToken.Uid));
            claims.Add(new Claim("firebase_project_id", _config.ProjectId));

            // Add all other claims from Firebase token
            foreach (var claim in decodedToken.Claims)
            {
                if (!claims.Any(c => c.Type == claim.Key))
                {
                    claims.Add(new Claim(claim.Key, claim.Value?.ToString() ?? ""));
                }
            }

            var identity = new ClaimsIdentity(claims, "Firebase");
            return new ClaimsPrincipal(identity);
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogWarning(ex, "Firebase token verification failed: {Error}", ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying Firebase token");
            return null;
        }
    }
}

