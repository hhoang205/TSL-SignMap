using System.Text;
using ApiGateway.Configuration;
using ApiGateway.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StackExchange.Redis;
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/apigateway-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API Gateway",
        Version = "v1",
        Description = "API Gateway for SignMap Traffic Sign Management System"
    });

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configuration
var serviceEndpoints = builder.Configuration.GetSection(ServiceEndpoints.SectionName).Get<ServiceEndpoints>()
    ?? new ServiceEndpoints();
var gatewayConfig = builder.Configuration.GetSection("Gateway").Get<GatewayConfig>()
    ?? new GatewayConfig();

// JWT Authentication
var jwtSecretKey = gatewayConfig.Jwt.SecretKey 
    ?? builder.Configuration["Jwt:SecretKey"] 
    ?? throw new InvalidOperationException("JWT SecretKey must be configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ValidateIssuer = true,
        ValidIssuer = gatewayConfig.Jwt.Issuer ?? builder.Configuration["Jwt:Issuer"] ?? "WebAppTrafficSign",
        ValidateAudience = true,
        ValidAudience = gatewayConfig.Jwt.Audience ?? builder.Configuration["Jwt:Audience"] ?? "WebAppTrafficSignUsers",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("StaffOrAdmin", policy => policy.RequireRole("Staff", "Admin"));
    options.AddPolicy("UserOrStaffOrAdmin", policy => policy.RequireRole("User", "Staff", "Admin"));
});

// CORS
var corsConfig = gatewayConfig.Cors;
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        if (corsConfig.AllowedOrigins != null && corsConfig.AllowedOrigins.Any())
        {
            policy.WithOrigins(corsConfig.AllowedOrigins.ToArray());
        }
        else
        {
            // Default dev origins
            policy.WithOrigins("http://localhost:3000", "http://localhost:19006", "http://localhost:5173");
        }

        if (corsConfig.AllowedMethods != null && corsConfig.AllowedMethods.Any())
        {
            policy.WithMethods(corsConfig.AllowedMethods.ToArray());
        }
        else
        {
            policy.WithMethods("GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS");
        }

        if (corsConfig.AllowedHeaders != null && corsConfig.AllowedHeaders.Any())
        {
            policy.WithHeaders(corsConfig.AllowedHeaders.ToArray());
        }
        else
        {
            policy.WithHeaders("Authorization", "Content-Type", "X-Request-Id");
        }

        if (corsConfig.AllowCredentials)
        {
            policy.AllowCredentials();
        }

        policy.SetPreflightMaxAge(TimeSpan.FromSeconds(corsConfig.MaxAge));
    });
});

// Redis Cache (optional)
if (!string.IsNullOrEmpty(gatewayConfig.Redis.ConnectionString))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = gatewayConfig.Redis.ConnectionString;
        options.InstanceName = gatewayConfig.Redis.InstanceName;
    });
}
else
{
    // Use in-memory cache if Redis not configured
    builder.Services.AddMemoryCache();
}

// Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("RateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

// HTTP Client Factory
builder.Services.AddHttpClient();

// Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1");
    });
}

// Middleware pipeline
app.UseHttpsRedirection();

// Security headers (early in pipeline)
app.UseMiddleware<SecurityHeadersMiddleware>();

// CORS (early in pipeline)
app.UseCors("DefaultPolicy");

// Request ID (early for correlation)
app.UseMiddleware<RequestIdMiddleware>();

// Logging
app.UseMiddleware<LoggingMiddleware>();

// Request transformation (add user context)
app.UseMiddleware<RequestTransformationMiddleware>();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Rate limiting
app.UseIpRateLimiting();

// Response compression
app.UseResponseCompression();

// Error handling (should be before controllers)
app.UseMiddleware<ErrorHandlingMiddleware>();

// Controllers
app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

// Reverse proxy (must be last)
app.MapReverseProxy();

// Configure reverse proxy routes dynamically
ConfigureReverseProxyRoutes(app, serviceEndpoints);

app.Run();

// Helper method to configure reverse proxy routes
static void ConfigureReverseProxyRoutes(WebApplication app, ServiceEndpoints endpoints)
{
    // Routes are configured in appsettings.json under "ReverseProxy" section
    // This method can be used for dynamic route configuration if needed
    Log.Information("Reverse proxy routes configured");
}

