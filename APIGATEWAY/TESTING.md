# API Gateway Testing Guide

## 🧪 Test Scripts

### PowerShell Script (Windows)
```powershell
.\test-api-gateway.ps1
```

### Bash Script (Linux/Mac)
```bash
chmod +x test-api-gateway.sh
./test-api-gateway.sh
```

## 🚀 Manual Testing

### 1. Start the API Gateway

```bash
cd APIGATEWAY
dotnet run
```

The gateway will start on `http://localhost:5000`

### 2. Test Health Check Endpoint

```bash
# Using curl
curl http://localhost:5000/api/health

# Using PowerShell
Invoke-RestMethod -Uri http://localhost:5000/api/health -Method GET
```

**Expected Response:**
```json
{
  "status": "healthy",
  "timestamp": "2024-01-01T00:00:00Z",
  "services": [
    {
      "name": "UserService",
      "status": "unknown",
      "url": "http://localhost:5001"
    },
    ...
  ]
}
```

### 3. Test Swagger UI

Open browser: `http://localhost:5000/swagger`

You should see the Swagger UI with API documentation.

### 4. Test CORS

```bash
# Test preflight request
curl -X OPTIONS http://localhost:5000/api/health \
  -H "Origin: http://localhost:3000" \
  -H "Access-Control-Request-Method: GET" \
  -H "Access-Control-Request-Headers: Authorization" \
  -v
```

**Expected:** Status 200 or 204 with CORS headers

### 5. Test Rate Limiting

```bash
# Send multiple rapid requests
for i in {1..10}; do
  curl http://localhost:5000/api/health
  echo "Request $i"
done
```

**Expected:** After limit, you should get 429 Too Many Requests

### 6. Test Security Headers

```bash
curl -I http://localhost:5000/api/health
```

**Expected Headers:**
- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `X-XSS-Protection: 1; mode=block`
- `Referrer-Policy: strict-origin-when-cross-origin`

### 7. Test Request ID

```bash
curl -I http://localhost:5000/api/health
```

**Expected Header:**
- `X-Request-Id: <unique-id>`

### 8. Test Error Handling

```bash
# Test invalid endpoint
curl http://localhost:5000/api/invalid-endpoint
```

**Expected:** 404 Not Found with error response

### 9. Test Proxy Routes (Requires Backend Services)

```bash
# Test users endpoint (requires authentication)
curl http://localhost:5000/api/users

# Test signs endpoint
curl http://localhost:5000/api/signs

# Test contributions endpoint (requires authentication)
curl http://localhost:5000/api/contributions
```

**Expected:**
- If backend is running: Response from backend service
- If backend is not running: 502 Bad Gateway or 503 Service Unavailable
- If authentication required: 401 Unauthorized

### 10. Test Authentication (Requires Valid JWT Token)

```bash
# Get token from your auth service first, then:
curl -H "Authorization: Bearer YOUR_TOKEN_HERE" http://localhost:5000/api/users
```

### 11. Test Token Refresh

```bash
curl -X POST http://localhost:5000/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{"refreshToken": "your-refresh-token-here"}'
```

## 📋 Test Checklist

- [ ] Health check endpoint returns 200
- [ ] Swagger UI loads correctly
- [ ] CORS preflight requests work
- [ ] Rate limiting works (429 after limit)
- [ ] Security headers are present
- [ ] Request ID is generated and returned
- [ ] Error handling returns proper error format
- [ ] Proxy routes forward to backend services
- [ ] Authentication required endpoints return 401 without token
- [ ] Token refresh endpoint works

## 🔍 Debugging

### Check Logs

Logs are written to:
- Console output
- File: `logs/apigateway-YYYY-MM-DD.txt`

### Common Issues

1. **502 Bad Gateway**: Backend service is not running
   - Solution: Start the backend service on the configured port

2. **401 Unauthorized**: JWT token is missing or invalid
   - Solution: Get a valid token from your auth service

3. **429 Too Many Requests**: Rate limit exceeded
   - Solution: Wait for the rate limit window to reset

4. **CORS errors**: Origin not allowed
   - Solution: Add your origin to `appsettings.json` → `Gateway.Cors.AllowedOrigins`

## 🧪 Integration Testing

For full integration testing, you need:

1. **API Gateway** running on port 5000
2. **Backend Services** running on ports 5001-5007:
   - User Service: 5001
   - Traffic Sign Service: 5002
   - Contribution Service: 5003
   - Vote Service: 5004
   - AI Vision Service: 5005
   - Notification Service: 5006
   - Payment Service: 5007

### Test Full Flow

```bash
# 1. Health check
curl http://localhost:5000/api/health

# 2. Login (if you have auth endpoint)
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "user", "password": "pass"}'

# 3. Use token to access protected endpoint
curl -H "Authorization: Bearer TOKEN" http://localhost:5000/api/users

# 4. Create contribution
curl -X POST http://localhost:5000/api/contributions \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title": "Test", "description": "Test contribution"}'
```

## 📊 Performance Testing

### Load Testing with Apache Bench

```bash
# 100 requests, 10 concurrent
ab -n 100 -c 10 http://localhost:5000/api/health
```

### Load Testing with wrk

```bash
# 100 requests, 10 threads, 30 seconds
wrk -t10 -c100 -d30s http://localhost:5000/api/health
```

## ✅ Expected Results

All tests should pass when:
- ✅ API Gateway is running
- ✅ Backend services are running (for proxy tests)
- ✅ Valid JWT tokens are available (for auth tests)
- ✅ Configuration is correct in `appsettings.json`

