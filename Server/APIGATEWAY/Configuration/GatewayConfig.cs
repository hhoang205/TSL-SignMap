namespace ApiGateway.Configuration;

/// Gateway configuration settings
public class GatewayConfig
{
    public JwtConfig Jwt { get; set; } = new();
    public FirebaseConfig Firebase { get; set; } = new();
    public RateLimitingConfig RateLimiting { get; set; } = new();
    public CorsConfig Cors { get; set; } = new();
    public RedisConfig Redis { get; set; } = new();
}

public class JwtConfig
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60 * 24 * 7; // 7 days
    public int RefreshExpirationDays { get; set; } = 30;
}

public class RateLimitingConfig
{
    public List<RateLimitRule> GeneralRules { get; set; } = new();
    public List<string> IpWhitelist { get; set; } = new();
    public List<string> UserWhitelist { get; set; } = new();
}

public class RateLimitRule
{
    public string Endpoint { get; set; } = string.Empty;
    public string Period { get; set; } = "1m";
    public int Limit { get; set; } = 100;
}

public class CorsConfig
{
    public List<string> AllowedOrigins { get; set; } = new();
    public List<string> AllowedMethods { get; set; } = new();
    public List<string> AllowedHeaders { get; set; } = new();
    public bool AllowCredentials { get; set; } = true;
    public int MaxAge { get; set; } = 3600;
}

public class RedisConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string InstanceName { get; set; } = "ApiGateway";
}

public class FirebaseConfig
{
    /// Firebase Project ID (required)
    public string ProjectId { get; set; } = string.Empty;
    
    /// Path to Firebase service account JSON file (optional, can use environment variable FIREBASE_CREDENTIALS)
    public string? ServiceAccountPath { get; set; }
    
    /// Enable Firebase authentication (default: false)
    public bool Enabled { get; set; } = false;
    
    /// Allow both JWT and Firebase tokens (default: true)
    /// If false, only Firebase tokens will be accepted when Firebase is enabled
    public bool AllowJwtFallback { get; set; } = true;
}

