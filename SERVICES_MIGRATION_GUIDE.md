# Hướng dẫn tách codebase thành Microservices

## Tổng quan

Codebase hiện tại đã được tách thành các microservices riêng biệt. Mỗi service có:
- Cấu trúc folder riêng
- DbContext riêng (chỉ chứa các DbSet liên quan)
- Namespace riêng
- Port riêng

## Cấu trúc Services

### 1. UserService (Port 5001)
**Chức năng:** User management, Authentication, Coin Wallet
**Models:** User, CoinWallet
**Controllers:** UserController, CoinWalletController
**Services:** UserService, CoinWalletService, TokenService, EmailService

### 2. TrafficSignService (Port 5002)
**Chức năng:** Traffic Sign CRUD, Search, Filter
**Models:** TrafficSign
**Controllers:** TrafficSignController
**Services:** TrafficSignService

### 3. ContributionService (Port 5003)
**Chức năng:** User contributions, submissions
**Models:** Contribution
**Controllers:** ContributionController
**Services:** ContributionService
**Dependencies:** UserService (HTTP), TrafficSignService (HTTP), CoinWalletService (HTTP), NotificationService (HTTP), AI Vision Service (HTTP)

### 4. VotingService (Port 5004)
**Chức năng:** Voting mechanism, weighted voting
**Models:** Vote
**Controllers:** VoteController
**Services:** VoteService
**Dependencies:** UserService (HTTP - để lấy reputation)

### 5. NotificationService (Port 5005)
**Chức năng:** Real-time notifications
**Models:** Notification
**Controllers:** NotificationController
**Services:** NotificationService, EmailService
**Dependencies:** SignalR Hub

### 6. PaymentService (Port 5006)
**Chức năng:** Payment gateway, coin top-up
**Models:** Payment
**Controllers:** PaymentController
**Services:** PaymentService
**Dependencies:** CoinWalletService (HTTP - để credit coins)

### 7. FeedbackService (Port 5007)
**Chức năng:** User feedback and reporting
**Models:** Feedback
**Controllers:** FeedbackController
**Services:** FeedbackService

## Các bước tách Service

### Bước 1: Tạo cấu trúc folder
```
ServiceName/
  Controllers/
  Services/
  Models/
  DTOs/
  Mapper/
  Data/
  Properties/
```

### Bước 2: Copy và cập nhật namespace
- Copy Models từ `WebAppTrafficSign.Models` → `ServiceName.Models`
- Copy DTOs từ `WebAppTrafficSign.DTOs` → `ServiceName.DTOs`
- Copy Mappers từ `WebAppTrafficSign.Mapper` → `ServiceName.Mapper`
- Copy Services từ `WebAppTrafficSign.Services` → `ServiceName.Services`
- Copy Controllers từ `WebAppTrafficSign.Controller` → `ServiceName.Controllers`

### Bước 3: Tạo DbContext riêng
- Chỉ include các DbSet liên quan đến service
- Update namespace
- Remove các relationships không cần thiết

### Bước 4: Tạo Program.cs
- Configure DbContext với connection string riêng (hoặc shared)
- Register services
- Configure Swagger
- Set port riêng

### Bước 5: Tạo appsettings.json
- Connection string (có thể shared hoặc riêng)
- JWT config (nếu cần)
- Service endpoints (cho inter-service communication)

### Bước 6: Cập nhật dependencies
- Thay direct dependencies bằng HTTP client calls
- Sử dụng HttpClient hoặc Refit để gọi các services khác

## Inter-Service Communication

### HTTP Client Pattern
```csharp
// In Program.cs
builder.Services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceEndpoints:PaymentService"]);
});

// Service client
public class PaymentServiceClient : IPaymentServiceClient
{
    private readonly HttpClient _httpClient;
    
    public async Task<PaymentDto> CreatePaymentAsync(PaymentCreateRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/payments", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PaymentDto>();
    }
}
```

## API Gateway Configuration

Cập nhật `APIGATEWAY/Configuration/ServiceEndpoints.cs`:
```csharp
public static class ServiceEndpoints
{
    public const string UserService = "http://localhost:5001";
    public const string TrafficSignService = "http://localhost:5002";
    public const string ContributionService = "http://localhost:5003";
    public const string VotingService = "http://localhost:5004";
    public const string NotificationService = "http://localhost:5005";
    public const string PaymentService = "http://localhost:5006";
    public const string FeedbackService = "http://localhost:5007";
}
```

## Database Strategy

### Option 1: Shared Database
- Tất cả services dùng chung 1 database
- Mỗi service chỉ truy cập các tables liên quan
- Đơn giản nhưng không thực sự microservices

### Option 2: Database per Service
- Mỗi service có database riêng
- Cần sync data qua events hoặc API calls
- Đúng với microservices architecture

**Recommendation:** Bắt đầu với Option 1, migrate sang Option 2 sau.

## Testing

Mỗi service có thể test độc lập:
```bash
cd UserService
dotnet run
# Test tại http://localhost:5001/swagger
```

## Deployment

### Development
- Chạy tất cả services cùng lúc
- Sử dụng API Gateway để route requests

### Production
- Deploy mỗi service riêng biệt
- Sử dụng Docker containers
- Service discovery và load balancing

## Next Steps

1. ✅ UserService - Hoàn thành
2. ⏳ TrafficSignService - Cần tách
3. ⏳ ContributionService - Cần tách + HTTP clients
4. ⏳ VotingService - Cần tách + HTTP clients
5. ⏳ NotificationService - Cần tách + SignalR
6. ⏳ PaymentService - Cần tách + HTTP clients
7. ⏳ FeedbackService - Cần tách
8. ⏳ Cập nhật API Gateway routes
9. ⏳ Tạo Docker compose cho tất cả services
10. ⏳ Setup inter-service communication

