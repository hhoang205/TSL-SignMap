namespace ApiGateway.Configuration;


/// Configuration class for service endpoints

public class ServiceEndpoints
{
    public const string SectionName = "Services";

    public string UserService { get; set; } = string.Empty;
    public string TrafficSignService { get; set; } = string.Empty;
    public string ContributionService { get; set; } = string.Empty;
    public string VoteService { get; set; } = string.Empty;
    public string AIVisionService { get; set; } = string.Empty;
    public string NotificationService { get; set; } = string.Empty;
    public string PaymentService { get; set; } = string.Empty;
}

