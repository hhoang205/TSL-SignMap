# PaymentService

Payment Service quản lý các giao dịch thanh toán (payments) và tích hợp với UserService để credit coins vào wallet.

## Port
- **HTTP**: 5006
- **HTTPS**: 7006

## Features

- ✅ CRUD operations cho Payments
- ✅ Filter và pagination
- ✅ Payment summary statistics
- ✅ Tự động credit coins vào wallet khi payment completed
- ✅ Validate User tồn tại qua UserService
- ✅ Hỗ trợ các payment methods: Credit Card, PayPal, etc.
- ✅ Payment status: Pending, Completed, Failed

## API Endpoints

### Payments

- `GET /api/payments/{id}` - Lấy payment theo ID
- `GET /api/payments/user/{userId}` - Lấy tất cả payments của user
- `GET /api/payments/status/{status}` - Lấy payments theo status
- `GET /api/payments/summary` - Lấy tổng hợp payments
- `POST /api/payments` - Tạo payment mới
- `PUT /api/payments/{id}` - Cập nhật payment
- `PUT /api/payments/{id}/status` - Cập nhật status của payment
- `POST /api/payments/filter` - Filter payments
- `DELETE /api/payments/{id}` - Xóa payment

## Inter-Service Communication

### Dependencies
- **UserService** (Port 5001) - Validate User tồn tại và credit/debit coins vào wallet

### HTTP Calls to UserService

1. **Validate User**: `GET /api/users/{userId}`
2. **Credit Coins**: `POST /api/wallets/user/{userId}/credit`
3. **Debit Coins**: `POST /api/wallets/user/{userId}/debit`
4. **Get Username**: `GET /api/users/{userId}` (để populate Username trong PaymentDto)

## Payment Flow

1. User tạo payment request với status "Pending"
2. Payment được lưu vào database
3. Khi payment status chuyển sang "Completed":
   - PaymentService tự động gọi UserService để credit coins vào wallet
   - Số coins = số tiền (theo tỷ lệ $1 = 10 Coins, được xử lý ở UserService)
4. Nếu payment status thay đổi từ Completed → khác, coins sẽ không bị trừ lại (có thể điều chỉnh theo nghiệp vụ)

## Database

- **Table**: `Payments`
- **Indexes**: 
  - `UserId`
  - `Status`
  - `PaymentDate`
  - `UserId, Status` (composite)

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "ServiceEndpoints": {
    "UserService": "http://localhost:5001"
  },
  "JwtSettings": {
    "SecretKey": "...",
    "Issuer": "WebAppTrafficSign",
    "Audience": "WebAppTrafficSign"
  }
}
```

## Usage Example

### Create Payment

```csharp
var request = new PaymentCreateRequest
{
    UserId = 1,
    Amount = 10.00m,
    PaymentMethod = "Credit Card",
    Status = "Pending"
};

var response = await httpClient.PostAsJsonAsync(
    "http://localhost:5006/api/payments", 
    request
);
```

### Update Payment Status to Completed

```csharp
var request = new PaymentStatusUpdateRequest
{
    Status = "Completed"
};

var response = await httpClient.PutAsJsonAsync(
    "http://localhost:5006/api/payments/1/status", 
    request
);
// PaymentService sẽ tự động credit coins vào wallet
```

### Filter Payments

```csharp
var request = new PaymentFilterRequest
{
    UserId = 1,
    Status = "Completed",
    StartDate = DateTime.UtcNow.AddMonths(-1),
    PageNumber = 1,
    PageSize = 20
};

var response = await httpClient.PostAsJsonAsync(
    "http://localhost:5006/api/payments/filter", 
    request
);
```

## Requirements

- .NET 8.0
- SQL Server
- UserService running on port 5001

## Payment Status

- **Pending**: Payment đang chờ xử lý
- **Completed**: Payment đã hoàn tất, coins đã được credit vào wallet
- **Failed**: Payment thất bại

## Notes

- Khi payment status chuyển sang "Completed", coins sẽ tự động được credit vào wallet
- Nếu thay đổi amount của payment đã completed, chênh lệch sẽ được điều chỉnh trong wallet
- Xóa payment không hoàn lại coins (có thể điều chỉnh theo nghiệp vụ)

