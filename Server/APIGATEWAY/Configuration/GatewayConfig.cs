namespace ApiGateway.Configuration;

/// <summary>
/// Strongly typed configuration object for API Gateway settings.
/// </summary>
public sealed class GatewayConfig
{
    public FirebaseSettings Firebase { get; init; } = new();
    public CorsSettings Cors { get; init; } = new();
    public ServiceRegistry Services { get; set; } = new();

    public sealed class FirebaseSettings
    {
        public string ProjectId { get; init; } = string.Empty;
    }

    public sealed class CorsSettings
    {
        public string[] AllowedOrigins { get; init; } = Array.Empty<string>();
    }

    public sealed class ServiceRegistry
    {
        public string UserService { get; set; } = string.Empty;
        public string TrafficSignService { get; set; } = string.Empty;
        public string ContributionService { get; set; } = string.Empty;
        public string VotingService { get; set; } = string.Empty;
        public string NotificationService { get; set; } = string.Empty;
        public string PaymentService { get; set; } = string.Empty;
        public string FeedbackService { get; set; } = string.Empty;
    }
}

