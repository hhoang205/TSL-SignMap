using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Http;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HealthController> _logger;

    public HealthController(
        HealthCheckService healthCheckService,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<HealthController> logger)
    {
        _healthCheckService = healthCheckService;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// Health check endpoint for API Gateway
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetHealth()
    {
        var healthResult = await _healthCheckService.CheckHealthAsync();
        var serviceChecks = await CheckBackendServicesHealthAsync();

        var response = new
        {
            status = healthResult.Status == HealthStatus.Healthy ? "Healthy" 
                : healthResult.Status == HealthStatus.Degraded ? "Degraded" 
                : "Unhealthy",
            checks = new Dictionary<string, string>
            {
                { "gateway", healthResult.Status.ToString() }
            }.Concat(serviceChecks),
            timestamp = DateTime.UtcNow
        };

        var statusCode = healthResult.Status == HealthStatus.Healthy ? 200 : 503;
        return StatusCode(statusCode, response);
    }

    private async Task<Dictionary<string, string>> CheckBackendServicesHealthAsync()
    {
        var services = new Dictionary<string, string>();
        var serviceEndpoints = _configuration.GetSection("Services").GetChildren();

        foreach (var service in serviceEndpoints)
        {
            var serviceName = service.Key;
            var serviceUrl = service.Value;

            if (string.IsNullOrEmpty(serviceUrl))
                continue;

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(3);

                var healthUrl = $"{serviceUrl.TrimEnd('/')}/api/health";
                var response = await client.GetAsync(healthUrl);

                services[serviceName] = response.IsSuccessStatusCode ? "Healthy" : "Unhealthy";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Health check failed for {ServiceName}", serviceName);
                services[serviceName] = "Unhealthy";
            }
        }

        return services;
    }
}

