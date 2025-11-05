# Script to split monolith into microservices
# This script will create separate services for each domain

$rootPath = "Z:\Project Code Cshap\WebAppTrafficSign"
$services = @(
    @{
        Name = "UserService"
        Port = 5001
        Controllers = @("UserController", "CoinWalletController")
        Models = @("User", "CoinWallet")
        Services = @("UserService", "CoinWalletService", "TokenService", "EmailService")
        DTOs = @("UserDto", "CoinWalletDto")
        Mappers = @("UserMapper", "CoinWalletMapper")
    },
    @{
        Name = "TrafficSignService"
        Port = 5002
        Controllers = @("TrafficSignController")
        Models = @("TrafficSign")
        Services = @("TrafficSignService")
        DTOs = @("TrafficSignDto")
        Mappers = @("TrafficSignMapper")
    },
    @{
        Name = "ContributionService"
        Port = 5003
        Controllers = @("ContributionController")
        Models = @("Contribution")
        Services = @("ContributionService")
        DTOs = @("ContributionDto")
        Mappers = @("ContributionMapper")
    },
    @{
        Name = "VotingService"
        Port = 5004
        Controllers = @("VoteController")
        Models = @("Vote")
        Services = @("VoteService")
        DTOs = @("VoteDto")
        Mappers = @("VoteMapper")
    },
    @{
        Name = "NotificationService"
        Port = 5005
        Controllers = @("NotificationController")
        Models = @("Notification")
        Services = @("NotificationService", "EmailService")
        DTOs = @("NotificationDto")
        Mappers = @("NotificationMapper")
    },
    @{
        Name = "PaymentService"
        Port = 5006
        Controllers = @("PaymentController")
        Models = @("Payment")
        Services = @("PaymentService")
        DTOs = @("PaymentDto")
        Mappers = @("PaymentMapper")
    },
    @{
        Name = "FeedbackService"
        Port = 5007
        Controllers = @("FeedbackController")
        Models = @("Feedback")
        Services = @("FeedbackService")
        DTOs = @("FeedbackDto")
        Mappers = @("FeedbackMapper")
    }
)

Write-Host "Starting service split process..." -ForegroundColor Green

foreach ($service in $services) {
    $serviceName = $service.Name
    $servicePath = Join-Path $rootPath $serviceName
    
    Write-Host "`nCreating $serviceName..." -ForegroundColor Yellow
    
    # Create directory structure
    $dirs = @("Controllers", "Services", "Models", "DTOs", "Mapper", "Data", "Properties")
    foreach ($dir in $dirs) {
        $dirPath = Join-Path $servicePath $dir
        if (-not (Test-Path $dirPath)) {
            New-Item -ItemType Directory -Path $dirPath -Force | Out-Null
        }
    }
    
    Write-Host "Created directory structure for $serviceName" -ForegroundColor Gray
}

Write-Host "`nService split script completed!" -ForegroundColor Green
Write-Host "Note: You need to manually copy and update files for each service." -ForegroundColor Yellow

