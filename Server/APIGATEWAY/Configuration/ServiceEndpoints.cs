namespace ApiGateway.Configuration;

/// <summary>
/// Provides default downstream endpoints for each microservice so that the gateway
/// can fall back to sensible values when configuration is incomplete.
/// </summary>
public static class ServiceEndpoints
{
    public const string UserService = "http://user-service:5001";
    public const string TrafficSignService = "http://traffic-sign-service:5002";
    public const string ContributionService = "http://contribution-service:5003";
    public const string VotingService = "http://voting-service:5004";
    public const string NotificationService = "http://notification-service:5005";
    public const string PaymentService = "http://payment-service:5006";
    public const string FeedbackService = "http://feedback-service:5007";

    public static GatewayConfig.ServiceRegistry WithDefaults(GatewayConfig.ServiceRegistry? registry)
    {
        registry ??= new GatewayConfig.ServiceRegistry();

        registry.UserService = string.IsNullOrWhiteSpace(registry.UserService)
            ? UserService
            : registry.UserService;
        registry.TrafficSignService = string.IsNullOrWhiteSpace(registry.TrafficSignService)
            ? TrafficSignService
            : registry.TrafficSignService;
        registry.ContributionService = string.IsNullOrWhiteSpace(registry.ContributionService)
            ? ContributionService
            : registry.ContributionService;
        registry.VotingService = string.IsNullOrWhiteSpace(registry.VotingService)
            ? VotingService
            : registry.VotingService;
        registry.NotificationService = string.IsNullOrWhiteSpace(registry.NotificationService)
            ? NotificationService
            : registry.NotificationService;
        registry.PaymentService = string.IsNullOrWhiteSpace(registry.PaymentService)
            ? PaymentService
            : registry.PaymentService;
        registry.FeedbackService = string.IsNullOrWhiteSpace(registry.FeedbackService)
            ? FeedbackService
            : registry.FeedbackService;

        return registry;
    }
}

