# UserService

Microservice quản lý người dùng, authentication và coin wallet.

## Port
- HTTP: 5001
- HTTPS: 7001

## Chức năng

### User Management
- Đăng ký tài khoản (nhận 20 coin ban đầu)
- Đăng nhập (xác thực thông tin, token do Firebase cung cấp)
- Quản lý profile
- Đổi mật khẩu
- Reset password qua email

### Coin Wallet
- Xem số dư
- Credit/Debit coins
- Admin điều chỉnh coins
- Top-up coins (tích hợp với PaymentService)

## API Endpoints

### User APIs
- `POST /api/users/register` - Đăng ký
- `POST /api/users/login` - Đăng nhập
- `GET /api/users/{id}` - Lấy thông tin user
- `GET /api/users/{id}/profile` - Lấy profile đầy đủ
- `PUT /api/users/{id}` - Cập nhật profile
- `POST /api/users/{id}/change-password` - Đổi mật khẩu
- `POST /api/users/forgot-password` - Reset password
- `POST /api/users/{id}/wallet/top-up` - Nạp coin
- `DELETE /api/users/{id}` - Xóa user

### Wallet APIs
- `GET /api/wallets/{id}` - Lấy wallet theo ID
- `GET /api/wallets/user/{userId}` - Lấy wallet theo UserId
- `GET /api/wallets/user/{userId}/balance` - Lấy số dư
- `POST /api/wallets` - Tạo wallet
- `PUT /api/wallets/{id}` - Cập nhật wallet
- `POST /api/wallets/user/{userId}/credit` - Cộng coin
- `POST /api/wallets/user/{userId}/debit` - Trừ coin
- `POST /api/wallets/user/{userId}/adjust` - Admin điều chỉnh
- `POST /api/wallets/filter` - Filter wallets

## Models

- `User` - Thông tin người dùng
- `CoinWallet` - Ví coin của user

## Services

- `UserService` - Business logic cho user
- `CoinWalletService` - Business logic cho wallet
- `EmailService` - Gửi email (mock implementation)

## Dependencies

- PaymentService (HTTP) - Cho top-up coins
- ContributionService (HTTP) - Cho user profile stats
- VotingService (HTTP) - Cho user profile stats

## Database

- Connection string trong `appsettings.json`
- DbContext: `UserDbContext`
- Tables: `Users`, `CoinWallets`

## Running

```bash
cd UserService
dotnet restore
dotnet run
```

Swagger UI: http://localhost:5001/swagger

## TODO

- [ ] Implement HTTP client cho PaymentService
- [ ] Implement HTTP client cho ContributionService
- [ ] Implement HTTP client cho VotingService
- [ ] Add authentication middleware
- [ ] Add rate limiting
- [ ] Add logging
- [ ] Add unit tests

