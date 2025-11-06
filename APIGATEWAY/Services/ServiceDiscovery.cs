using ApiGateway.Configuration;

namespace ApiGateway.Services;


/// Service discovery implementation (basic - can be extended with Consul, Eureka, etc.)

public class ServiceDiscovery : IServiceDiscovery
{
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, string> _serviceRegistry;

    public ServiceDiscovery(IConfiguration configuration)
    {
        _configuration = configuration;
        _serviceRegistry = new Dictionary<string, string>();
        LoadServicesFromConfig();
    }

    private void LoadServicesFromConfig()
    {
        var services = _configuration.GetSection(ServiceEndpoints.SectionName).Get<ServiceEndpoints>();
        if (services != null)
        {
            _serviceRegistry["UserService"] = services.UserService;
            _serviceRegistry["TrafficSignService"] = services.TrafficSignService;
            _serviceRegistry["ContributionService"] = services.ContributionService;
            _serviceRegistry["VoteService"] = services.VoteService;
            _serviceRegistry["AIVisionService"] = services.AIVisionService;
            _serviceRegistry["NotificationService"] = services.NotificationService;
            _serviceRegistry["PaymentService"] = services.PaymentService;
        }
    }

    public Task<string?> GetServiceUrlAsync(string serviceName)
    {
        _serviceRegistry.TryGetValue(serviceName, out var url);
        return Task.FromResult<string?>(url);
    }

    public Task RegisterServiceAsync(string serviceName, string url)
    {
        _serviceRegistry[serviceName] = url;
        return Task.CompletedTask;
    }

    public Task UnregisterServiceAsync(string serviceName)
    {
        _serviceRegistry.Remove(serviceName);
        return Task.CompletedTask;
    }
}

