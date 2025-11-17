using ApiGateway.Configuration;
using ApiGateway.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

const string CorsPolicyName = "GatewayCors";
const string AuthScheme = "Firebase";

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.Configure<GatewayConfig>(builder.Configuration.GetSection("Gateway"));

var gatewayConfig = builder.Configuration.GetSection("Gateway").Get<GatewayConfig>()
                    ?? throw new InvalidOperationException("Gateway configuration is missing.");

gatewayConfig.Services = ServiceEndpoints.WithDefaults(gatewayConfig.Services);

if (string.IsNullOrWhiteSpace(gatewayConfig.Firebase.ProjectId))
{
    throw new InvalidOperationException("Gateway:Firebase:ProjectId is required.");
}

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = AuthScheme;
        options.DefaultChallengeScheme = AuthScheme;
    })
    .AddJwtBearer(AuthScheme, options =>
    {
        options.Authority = $"https://securetoken.google.com/{gatewayConfig.Firebase.ProjectId}";
        options.Audience = gatewayConfig.Firebase.ProjectId;
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{gatewayConfig.Firebase.ProjectId}",
            ValidateAudience = true,
            ValidAudience = gatewayConfig.Firebase.ProjectId,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = null;
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(AuthScheme)
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        var origins = gatewayConfig.Cors.AllowedOrigins;

        if (origins is { Length: > 0 })
        {
            policy.WithOrigins(origins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseCors(CorsPolicyName);
app.UseRequestLogging();
app.UseWebSockets();

app.MapGet("/api/health", () => Results.Ok(new
    {
        status = "Healthy",
        environment = app.Environment.EnvironmentName,
        services = new
        {
            gatewayConfig.Services.UserService,
            gatewayConfig.Services.TrafficSignService,
            gatewayConfig.Services.ContributionService,
            gatewayConfig.Services.VotingService,
            gatewayConfig.Services.NotificationService,
            gatewayConfig.Services.PaymentService,
            gatewayConfig.Services.FeedbackService
        }
    }))
    .WithName("GatewayHealth")
    .WithTags("Diagnostics");

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

app.Run();

