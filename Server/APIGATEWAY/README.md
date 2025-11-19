# API Gateway (Ocelot)

Gateway dành cho hệ thống **AI-Enabled Community-based Traffic Sign Location Management System**. Dự án này gom toàn bộ microservices phía sau thành một entrypoint duy nhất, xử lý:

- CORS và logging tập trung
- Firebase JWT authentication / authorization
- Điều phối request bằng [Ocelot](https://github.com/ThreeMammals/Ocelot)
- Forward WebSocket traffic tới SignalR hub (NotificationService)

## Kiến trúc & cấu hình

| Service | Downstream | Mặc định (Docker) | Local override (Development) |
|---------|------------|-------------------|------------------------------|
| User / Wallet | `/api/users`, `/api/wallets` | `http://user-service:5001` | `http://localhost:5001` |
| Traffic Sign | `/api/signs` | `http://traffic-sign-service:5002` | `http://localhost:5002` |
| Contribution | `/api/contributions` | `http://contribution-service:5003` | `http://localhost:5003` |
| Voting | `/api/votes` | `http://voting-service:5004` | `http://localhost:5004` |
| Notification REST | `/api/notifications` | `http://notification-service:5005` | `http://localhost:5005` |
| Notification Hub | `/notificationHub` | `http://notification-service:5005` | `http://localhost:5005` |
| Payment | `/api/payments` | `http://payment-service:5006` | `http://localhost:5006` |
| Feedback | `/api/feedbacks` | `http://feedback-service:5007` | `http://localhost:5007` |

Các giá trị này được đặt trong `appsettings*.json` (section `Gateway:Services`). File `Configuration/ServiceEndpoints.cs` cung cấp fallback để tránh thiếu cấu hình.

### Firebase Authentication

- `Gateway:Firebase:ProjectId` = Firebase project đang dùng (hiện tại `tfsignmangage`).
- Scheme mặc định đặt là `Firebase`; Ocelot routes nào có `AuthenticationOptions.AuthenticationProviderKey = "Firebase"` sẽ yêu cầu token hợp lệ.
- Public routes (đăng ký / đăng nhập / quên mật khẩu) được định nghĩa riêng với `Priority` cao hơn trong `ocelot.json`.

### CORS

- `Gateway:Cors:AllowedOrigins` (array). `Development` đã bật sẵn `http://localhost:5173` và `http://localhost:3000`.
- Nếu mảng rỗng, gateway fallback sang `AllowAnyOrigin`.

### Health check

`GET /api/health` trả về environment + downstream URLs. Dockerfile và docker-compose dùng endpoint này cho health probe.

## Chạy gateway

```bash
# Restore & run từ root solution
dotnet restore Server/APIGATEWAY/ApiGateway.csproj
dotnet run --project Server/APIGATEWAY/ApiGateway.csproj

# hoặc chạy trong docker (docker-compose ở root)
docker compose up api-gateway
```

> Lưu ý: Khi chạy ngoài Docker, các microservices cũng cần chạy trên `localhost:500x` (đã pre-config trong `appsettings.Development.json`). Nếu bạn khởi chạy service ở port khác, cập nhật lại config tương ứng.

## Route map quan trọng (`ocelot.json`)

- `POST /api/users/register|login|forgot-password` → UserService (public)
- `* /api/users/**` (các HTTP verb) → UserService (yêu cầu token)
- `* /api/wallets/**` → UserService (yêu cầu token)
- `* /api/signs/**` → TrafficSignService (yêu cầu token)
- `* /api/contributions/**` → ContributionService (yêu cầu token)
- `* /api/votes/**` → VotingService (yêu cầu token)
- `* /api/notifications/**` → NotificationService (yêu cầu token)
- `GET /notificationHub` → SignalR hub (yêu cầu token, hỗ trợ WebSocket)
- `* /api/payments/**` → PaymentService (yêu cầu token)
- `* /api/feedbacks/**` → FeedbackService (yêu cầu token)

Tất cả routes secure đều forward header `Authorization` sang service downstream.

## Logging

`Middleware/RequestLoggingMiddleware` log mỗi request với thời gian xử lý và `TraceId`. Bạn có thể mở rộng để push sang hệ thống observability (Seq, ELK, v.v.).

## Tham khảo thêm

- `ApiGateway/` ở root repo cung cấp ví dụ tối giản khi cần so sánh.
- `SERVICES_MIGRATION_GUIDE.md` & `MICROSERVICES_SPLIT_SUMMARY.md` mô tả đầy đủ vai trò từng service.

