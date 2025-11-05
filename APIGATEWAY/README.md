# API Gateway Service

API Gateway cho SignMap Traffic Sign Management System sử dụng YARP (Yet Another Reverse Proxy) và .NET 8.

## Features

- ✅ JWT Authentication & Authorization
- ✅ Role-Based Access Control (RBAC)
- ✅ Reverse Proxy với YARP
- ✅ Rate Limiting
- ✅ Request/Response Logging
- ✅ Error Handling
- ✅ Health Checks
- ✅ CORS Configuration
- ✅ Security Headers
- ✅ Request ID Correlation
- ✅ Token Refresh Endpoint

## Project Structure

```
APIGATEWAY/
├── Configuration/       # Configuration classes
├── Controllers/         # API controllers (Auth, Health)
├── Middleware/          # Custom middleware
├── Models/              # DTOs, ViewModels
├── Filters/             # Authorization filters
├── Program.cs           # Application entry point
└── appsettings.json     # Configuration
```

## Setup

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 hoặc VS Code

### Installation

1. Restore packages:
```bash
dotnet restore
```

2. Update `appsettings.json` với service endpoints thực tế

3. Run:
```bash
dotnet run
```

API Gateway sẽ chạy tại `http://localhost:5000`

## Configuration

### Service Endpoints

Cấu hình service endpoints trong `appsettings.json`:

```json
{
  "Services": {
    "UserService": "http://localhost:5001",
    "TrafficSignService": "http://localhost:5002",
    ...
  }
}
```

### JWT Configuration

```json
{
  "Gateway": {
    "Jwt": {
      "SecretKey": "your-secret-key",
      "Issuer": "WebAppTrafficSign",
      "Audience": "WebAppTrafficSignUsers"
    }
  }
}
```

### Rate Limiting

Cấu hình rate limiting trong `appsettings.json`:

```json
{
  "RateLimiting": {
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 100
      }
    ]
  }
}
```

## API Routes

### Public Routes (No Auth Required)
- `POST /api/auth/refresh` - Refresh access token
- `GET /api/health` - Health check
- `GET /health` - Health check endpoint

### Protected Routes (Auth Required)
- `/api/users/*` → User Service
- `/api/signs/*` → Traffic Sign Service
- `/api/contributions/*` → Contribution Service
- `/api/votes/*` → Vote Service
- `/api/ai/*` → AI Vision Service
- `/api/notifications/*` → Notification Service
- `/api/payments/*` → Payment Service
- `/api/wallets/*` → User Service (CoinWallet)

## Authentication

API Gateway sử dụng JWT Bearer tokens. Include token trong request header:

```
Authorization: Bearer {token}
```

## Health Checks

Check health của gateway và backend services:

```bash
curl http://localhost:5000/api/health
```

Response:
```json
{
  "status": "Healthy",
  "checks": {
    "gateway": "Healthy",
    "userService": "Healthy",
    ...
  },
  "timestamp": "2025-01-01T00:00:00Z"
}
```

## Logging

Logs được ghi vào:
- Console (development)
- File: `logs/apigateway-{date}.txt`

Log format sử dụng Serilog với structured logging.

## Rate Limiting

Rate limiting được áp dụng per IP và per user. Response headers:
- `X-RateLimit-Limit`: Limit
- `X-RateLimit-Remaining`: Remaining requests
- `X-RateLimit-Reset`: Reset time

## CORS

CORS được cấu hình trong `appsettings.json`. Default allows:
- Origins: `http://localhost:3000`, `http://localhost:19006`, `http://localhost:5173`
- Methods: GET, POST, PUT, DELETE, PATCH, OPTIONS
- Headers: Authorization, Content-Type, X-Request-Id

## Development

### Swagger UI

Access Swagger UI tại: `http://localhost:5000/swagger`

### Testing

Test với Postman hoặc curl:

```bash
# Health check
curl http://localhost:5000/api/health

# Protected route (requires JWT)
curl -H "Authorization: Bearer {token}" http://localhost:5000/api/users
```

## Production Deployment

1. Update `appsettings.json` với production service endpoints
2. Configure JWT secret key từ environment variables hoặc secret store
3. Enable HTTPS
4. Configure Redis cho caching (optional)
5. Setup monitoring và alerting

## Notes

- API Gateway proxy requests đến backend services
- Backend services cần expose tại ports được cấu hình
- JWT secret key phải match với backend services
- Rate limiting sử dụng in-memory cache (Redis recommended cho production)

