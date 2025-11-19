# SharedLibrary

Shared library chứa các DTOs, constants, và utilities dùng chung cho các microservices.

## Cấu trúc

```
SharedLibrary/
├── DTOs/              # Common DTOs cho inter-service communication
│   ├── UserResponse.cs
│   ├── CreditRequest.cs
│   ├── DebitRequest.cs
│   └── ErrorResponse.cs
├── Constants/         # Constants dùng chung
│   ├── ServicePorts.cs
│   ├── ServiceEndpoints.cs
│   └── StatusValues.cs
└── Utilities/        # Helper utilities
    └── HttpClientHelper.cs
```

## DTOs

### UserResponse
DTO chung cho thông tin User, được sử dụng trong inter-service communication.

### CreditRequest / DebitRequest
DTOs cho việc credit/debit coins vào wallet.

### ErrorResponse
DTO chuẩn cho error responses.

## Constants

### ServicePorts
Constants cho ports của các services:
- UserService: 5001
- TrafficSignService: 5002
- ContributionService: 5003
- VotingService: 5004
- NotificationService: 5005
- PaymentService: 5006
- FeedbackService: 5007
- ApiGateway: 5000

### ServiceEndpoints
Constants cho API endpoint paths.

### StatusValues
Common status values:
- Contribution: Pending, Approved, Rejected, UnderReview
- Payment: Pending, Completed, Failed, Cancelled
- Feedback: Pending, Reviewed, Resolved
- Notification: Unread, Read

## Utilities

### HttpClientHelper
Helper methods cho HTTP client operations:
- `ValidateUserExistsAsync`: Validate user tồn tại
- `GetUserAsync`: Lấy thông tin user

## Usage

### Thêm reference vào service:

```xml
<ItemGroup>
  <ProjectReference Include="..\SharedLibrary\SharedLibrary.csproj" />
</ItemGroup>
```

### Sử dụng trong code:

```csharp
using SharedLibrary.DTOs;
using SharedLibrary.Constants;
using SharedLibrary.Utilities;

// Sử dụng constants
var userServiceUrl = $"http://localhost:{ServicePorts.UserService}";

// Sử dụng DTOs
var user = await HttpClientHelper.GetUserAsync(httpClient, userId, userServiceUrl);

// Sử dụng status values
if (status == StatusValues.Pending) { ... }
```

## Lưu ý

- Shared library chỉ chứa các components thực sự dùng chung
- Không chứa business logic
- Không chứa database context hoặc dependencies nặng
- Tất cả classes đều public để các services có thể sử dụng

