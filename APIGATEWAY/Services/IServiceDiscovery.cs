namespace ApiGateway.Services;

/// <summary>
/// Interface for service discovery (optional - for future implementation)
/// </summary>
public interface IServiceDiscovery
{
    Task<string?> GetServiceUrlAsync(string serviceName);
    Task RegisterServiceAsync(string serviceName, string url);
    Task UnregisterServiceAsync(string serviceName);
}

