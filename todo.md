# TODO - SignMap Project
## AI-Enabled Community-based Traffic Sign Location Management System

**Project Status:** In Progress  
**Last Updated:** 2024

---

## 📋 TỔNG QUAN

Dự án được chia thành các microservices và modules chính:
1. **API Gateway Service** - Entry point, routing, authentication
2. **User Service** - Authentication, user management, reputation system
3. **Traffic Sign Service** - Core business logic cho traffic signs
4. **Contribution Service** - Quản lý user contributions
5. **Voting Service** - Weighted voting mechanism
6. **AI Vision Service** - YOLO-based traffic sign detection & classification
7. **Notification Service** - Real-time notifications
8. **Payment Service** - Coin top-up, wallet management
9. **Admin Web App** - React-based admin panel
10. **Mobile App** - iOS/Android app với OpenStreetMap integration

---

## 🏗️ MICROSERVICES ARCHITECTURE

### 1. API Gateway Service
**Status:** ⚠️ Not Started  
**Priority:** High  
**Technology:** .NET 8 (recommended) hoặc Node.js/Express  
**Port:** 5000 (Development), 443 (Production)

#### Tasks:
- [ ] **1.1** Setup API Gateway Project
  - [ ] **1.1.1** Create .NET 8 Web API project
    - [ ] Tạo project: `ApiGateway`
    - [ ] Install packages:
      - `Microsoft.AspNetCore.OpenApi`
      - `Yarp.ReverseProxy` (hoặc `Ocelot` nếu prefer)
      - `Microsoft.AspNetCore.Authentication.JwtBearer`
      - `Swashbuckle.AspNetCore` (Swagger)
      - `Serilog.AspNetCore` (Logging)
      - `StackExchange.Redis` (Cache)
      - `AspNetCoreRateLimit` (Rate limiting)
  
  - [ ] **1.1.2** Project Structure
    ```
    ApiGateway/
    ├── Controllers/          # Gateway controllers (nếu cần)
    ├── Middleware/          # Custom middleware
    │   ├── AuthenticationMiddleware.cs
    │   ├── RateLimitMiddleware.cs
    │   ├── LoggingMiddleware.cs
    │   └── ErrorHandlingMiddleware.cs
    ├── Services/            # Gateway services
    │   ├── IServiceDiscovery.cs
    │   └── ServiceDiscovery.cs
    ├── Configuration/       # Configuration classes
    │   ├── GatewayConfig.cs
    │   └── ServiceEndpoints.cs
    ├── Filters/             # Action filters
    ├── Models/              # DTOs, ViewModels
    ├── Program.cs
    └── appsettings.json
    ```
  
  - [ ] **1.1.3** Configure Service Endpoints
    - [ ] Define service base URLs trong `appsettings.json`:
      ```json
      {
        "Services": {
          "UserService": "http://localhost:5001",
          "TrafficSignService": "http://localhost:5002",
          "ContributionService": "http://localhost:5003",
          "VoteService": "http://localhost:5004",
          "AIVisionService": "http://localhost:5005",
          "NotificationService": "http://localhost:5006",
          "PaymentService": "http://localhost:5007"
        }
      }
      ```
    - [ ] Create `ServiceEndpoints` configuration class
    - [ ] Support environment variables cho production
  
  - [ ] **1.1.4** Setup YARP Reverse Proxy
    - [ ] Configure YARP trong `Program.cs`
    - [ ] Define reverse proxy routes
    - [ ] Configure load balancing (nếu multiple instances)
    - [ ] Setup health checks cho backend services
  
  - [ ] **1.1.5** Service Discovery (Optional)
    - [ ] Research service discovery solution (Consul, Eureka, etc.)
    - [ ] Implement service registration/discovery
    - [ ] Dynamic endpoint resolution
  
- [ ] **1.2** Implement Authentication & Authorization
  - [ ] **1.2.1** JWT Token Validation
    - [ ] Install `Microsoft.AspNetCore.Authentication.JwtBearer`
    - [ ] Configure JWT authentication trong `Program.cs`:
      - JWT secret key từ configuration
      - Token validation parameters (issuer, audience, expiration)
      - Validate signing key
    - [ ] Create `JwtTokenValidationMiddleware`:
      - Validate token format
      - Check token expiration
      - Extract user claims (userId, role, etc.)
      - Attach user info to request context
  
  - [ ] **1.2.2** Role-Based Access Control (RBAC)
    - [ ] Define roles: `User`, `Staff`, `Admin`
    - [ ] Create `AuthorizeAttribute` extensions:
      - `[AuthorizeRole("Admin")]`
      - `[AuthorizeRole("Staff", "Admin")]`
    - [ ] Implement role-based route protection:
      - Public routes (login, register)
      - User routes (require authentication)
      - Staff routes (require Staff or Admin role)
      - Admin routes (require Admin role only)
    - [ ] Create authorization policies:
      ```csharp
      options.AddPolicy("AdminOnly", policy => 
        policy.RequireRole("Admin"));
      options.AddPolicy("StaffOrAdmin", policy => 
        policy.RequireRole("Staff", "Admin"));
      ```
  
  - [ ] **1.2.3** Token Refresh Mechanism
    - [ ] Create endpoint: `POST /api/auth/refresh`
    - [ ] Validate refresh token
    - [ ] Generate new access token
    - [ ] Update token expiration logic
    - [ ] Handle token refresh errors
  
  - [ ] **1.2.4** Token Blacklist (Optional)
    - [ ] Implement token blacklist (Redis)
    - [ ] Check blacklist trong middleware
    - [ ] Add token to blacklist on logout
  
  - [ ] **1.2.5** Authentication Routes
    - [ ] Public routes (không cần auth):
      - `POST /api/auth/register`
      - `POST /api/auth/login`
      - `GET /api/health`
    - [ ] Protected routes (cần auth):
      - Tất cả routes khác
  
- [ ] **1.3** Request Routing & Reverse Proxy
  - [ ] **1.3.1** Configure YARP Routes
    - [ ] Route `/api/users/*` → User Service
      - Forward to: `{UserService}/api/users/*`
      - Methods: GET, POST, PUT, DELETE, PATCH
      - Auth: Required (User, Staff, Admin)
    - [ ] Route `/api/signs/*` → Traffic Sign Service
      - Forward to: `{TrafficSignService}/api/signs/*`
      - Methods: GET (public read), POST/PUT/DELETE (require auth)
      - Auth: GET (optional), POST/PUT/DELETE (require Staff/Admin)
    - [ ] Route `/api/contributions/*` → Contribution Service
      - Forward to: `{ContributionService}/api/contributions/*`
      - Methods: GET, POST, PUT
      - Auth: Required (User, Staff, Admin)
    - [ ] Route `/api/votes/*` → Voting Service
      - Forward to: `{VoteService}/api/votes/*`
      - Methods: GET, POST, PUT, DELETE
      - Auth: Required (User, Staff, Admin)
    - [ ] Route `/api/ai/*` → AI Vision Service
      - Forward to: `{AIVisionService}/api/ai/*`
      - Methods: POST
      - Auth: Required (User, Staff, Admin)
      - Timeout: 60 seconds (AI processing có thể lâu)
    - [ ] Route `/api/notifications/*` → Notification Service
      - Forward to: `{NotificationService}/api/notifications/*`
      - Methods: GET, POST, PUT
      - Auth: Required (User, Staff, Admin)
    - [ ] Route `/api/payments/*` → Payment Service
      - Forward to: `{PaymentService}/api/payments/*`
      - Methods: GET, POST
      - Auth: Required (User, Staff, Admin)
    - [ ] Route `/api/wallets/*` → User Service (CoinWallet)
      - Forward to: `{UserService}/api/wallets/*`
      - Methods: GET, POST
      - Auth: Required (User, Staff, Admin)
  
  - [ ] **1.3.2** Request Transformation
    - [ ] Add user context headers:
      - `X-User-Id`: Từ JWT token
      - `X-User-Role`: Từ JWT token
      - `X-Request-Id`: Unique request ID
    - [ ] Forward original headers (Authorization, Content-Type, etc.)
    - [ ] Remove sensitive headers nếu cần
  
  - [ ] **1.3.3** Response Transformation
    - [ ] Standardize response format
    - [ ] Error response transformation
    - [ ] Add CORS headers nếu cần
  
  - [ ] **1.3.4** Load Balancing
    - [ ] Configure load balancing strategy (RoundRobin, LeastRequests, etc.)
    - [ ] Health check integration
    - [ ] Failover logic
  
  - [ ] **1.3.5** Circuit Breaker Pattern
    - [ ] Implement circuit breaker cho backend services
    - [ ] Fail fast khi service down
    - [ ] Retry logic với exponential backoff
    - [ ] Fallback responses
  
- [ ] **1.4** Rate Limiting & Caching
  - [ ] **1.4.1** Rate Limiting Implementation
    - [ ] Install `AspNetCoreRateLimit` package
    - [ ] Configure rate limit rules trong `appsettings.json`:
      ```json
      {
        "RateLimiting": {
          "GeneralRules": [
            {
              "Endpoint": "*",
              "Period": "1m",
              "Limit": 100
            },
            {
              "Endpoint": "POST:/api/contributions",
              "Period": "1h",
              "Limit": 20
            },
            {
              "Endpoint": "POST:/api/votes",
              "Period": "1m",
              "Limit": 30
            }
          ],
          "IpWhitelist": ["127.0.0.1"],
          "UserWhitelist": ["admin@example.com"]
        }
      }
      ```
    - [ ] Per-user rate limiting (dựa trên JWT userId)
    - [ ] Per-IP rate limiting (fallback)
    - [ ] Different limits cho different endpoints:
      - Public endpoints: 100 req/min
      - Contribution submission: 20 req/hour
      - Voting: 30 req/min
      - AI detection: 10 req/hour (expensive)
    - [ ] Rate limit response headers:
      - `X-RateLimit-Limit`
      - `X-RateLimit-Remaining`
      - `X-RateLimit-Reset`
  
  - [ ] **1.4.2** Caching Strategy
    - [ ] Install `StackExchange.Redis` hoặc `Microsoft.Extensions.Caching.StackExchangeRedis`
    - [ ] Configure Redis connection
    - [ ] Cache static data:
      - Traffic sign types (TTL: 24 hours)
      - User roles/permissions (TTL: 1 hour)
      - System configuration (TTL: 1 hour)
    - [ ] Cache GET responses:
      - `/api/signs` search results (TTL: 5 minutes)
      - User profile data (TTL: 1 minute)
    - [ ] Cache invalidation:
      - Invalidate cache khi có updates
      - Pattern-based invalidation
    - [ ] Cache key strategy:
      - `signs:search:{hash_of_query}`
      - `user:{userId}:profile`
      - `signs:types`
  
  - [ ] **1.4.3** Response Caching Middleware
    - [ ] Configure response caching
    - [ ] Cache headers (Cache-Control, ETag)
    - [ ] Conditional requests (If-None-Match)
  
- [ ] **1.5** Error Handling & Logging
  - [ ] **1.5.1** Centralized Error Handling
    - [ ] Create `ErrorHandlingMiddleware`:
      - Catch all exceptions
      - Transform exceptions to standard error response
      - Log errors
      - Return appropriate HTTP status codes
    - [ ] Standard error response format:
      ```json
      {
        "error": {
          "code": "ERROR_CODE",
          "message": "User-friendly message",
          "details": "Technical details (dev only)",
          "timestamp": "2025-01-01T00:00:00Z",
          "requestId": "unique-request-id"
        }
      }
      ```
    - [ ] Handle specific exceptions:
      - `UnauthorizedException` → 401
      - `ForbiddenException` → 403
      - `NotFoundException` → 404
      - `ValidationException` → 400
      - `RateLimitExceededException` → 429
      - `ServiceUnavailableException` → 503
      - Generic exceptions → 500 (không expose details trong production)
  
  - [ ] **1.5.2** Request/Response Logging
    - [ ] Install `Serilog` hoặc `NLog`
    - [ ] Configure structured logging:
      - Request method, path, query string
      - Request headers (sanitize sensitive data)
      - Request body (sanitize passwords)
      - Response status code
      - Response time
      - User ID (từ JWT)
      - IP address
      - Request ID
    - [ ] Log levels:
      - Information: Normal requests
      - Warning: 4xx errors
      - Error: 5xx errors, exceptions
      - Debug: Detailed tracing (dev only)
    - [ ] Log to multiple sinks:
      - Console (development)
      - File (production)
      - Application Insights / CloudWatch (cloud)
  
  - [ ] **1.5.3** Health Check Endpoints
    - [ ] Install `Microsoft.Extensions.Diagnostics.HealthChecks`
    - [ ] Create health check endpoint: `GET /api/health`
      - Return 200 nếu gateway healthy
      - Return 503 nếu có issues
    - [ ] Check backend services health:
      - User Service health
      - Traffic Sign Service health
      - Contribution Service health
      - Vote Service health
      - AI Vision Service health
      - Notification Service health
      - Payment Service health
    - [ ] Health check response:
      ```json
      {
        "status": "Healthy|Degraded|Unhealthy",
        "checks": {
          "gateway": "Healthy",
          "userService": "Healthy",
          "trafficSignService": "Healthy",
          ...
        },
        "timestamp": "2025-01-01T00:00:00Z"
      }
      ```
    - [ ] Configure health check intervals
    - [ ] Setup alerting cho unhealthy services
  
  - [ ] **1.5.4** Request ID & Correlation
    - [ ] Generate unique request ID cho mỗi request
    - [ ] Add request ID to headers: `X-Request-Id`
    - [ ] Forward request ID to backend services
    - [ ] Include request ID trong logs
    - [ ] Include request ID trong error responses
  
- [ ] **1.6** CORS Configuration
  - [ ] **1.6.1** Configure CORS Policy
    - [ ] Allow origins:
      - `http://localhost:3000` (Admin Web App dev)
      - `http://localhost:19006` (React Native dev)
      - Production domains (configure từ appsettings)
    - [ ] Allow methods: GET, POST, PUT, DELETE, PATCH, OPTIONS
    - [ ] Allow headers: Authorization, Content-Type, X-Request-Id
    - [ ] Allow credentials: true
    - [ ] Max age: 3600 seconds
  
  - [ ] **1.6.2** CORS Middleware
    - [ ] Apply CORS middleware early trong pipeline
    - [ ] Handle preflight requests (OPTIONS)
  
- [ ] **1.7** Security Enhancements
  - [ ] **1.7.1** HTTPS Enforcement
    - [ ] Redirect HTTP to HTTPS
    - [ ] HSTS headers
    - [ ] SSL/TLS configuration
  
  - [ ] **1.7.2** Security Headers
    - [ ] Add security headers middleware:
      - `X-Content-Type-Options: nosniff`
      - `X-Frame-Options: DENY`
      - `X-XSS-Protection: 1; mode=block`
      - `Strict-Transport-Security: max-age=31536000`
      - `Content-Security-Policy`
  
  - [ ] **1.7.3** Input Validation
    - [ ] Validate request size limits
    - [ ] Validate file upload size (max 10MB cho images)
    - [ ] Sanitize user input
    - [ ] Validate query parameters
  
  - [ ] **1.7.4** API Versioning (Optional)
    - [ ] Support API versioning: `/api/v1/...`
    - [ ] Version negotiation
    - [ ] Deprecation warnings
  
- [ ] **1.8** Performance & Monitoring
  - [ ] **1.8.1** Request/Response Compression
    - [ ] Enable response compression (gzip, brotli)
    - [ ] Compress large responses
  
  - [ ] **1.8.2** Metrics & Observability
    - [ ] Request count metrics
    - [ ] Response time metrics
    - [ ] Error rate metrics
    - [ ] Rate limit hit metrics
    - [ ] Integration với Application Insights / Prometheus
    - [ ] Custom dashboards
  
  - [ ] **1.8.3** Distributed Tracing
    - [ ] Implement distributed tracing (OpenTelemetry)
    - [ ] Trace requests across services
    - [ ] Correlation IDs
  
- [ ] **1.9** Testing
  - [ ] **1.9.1** Unit Tests
    - [ ] Test middleware logic
    - [ ] Test authentication/authorization
    - [ ] Test rate limiting
    - [ ] Test error handling
  
  - [ ] **1.9.2** Integration Tests
    - [ ] Test routing to backend services
    - [ ] Test request/response transformation
    - [ ] Test health checks
    - [ ] Test CORS policies
  
  - [ ] **1.9.3** Load Testing
    - [ ] Load test với high traffic
    - [ ] Test rate limiting under load
    - [ ] Test circuit breaker behavior
  
- [ ] **1.10** Documentation
  - [ ] **1.10.1** Swagger/OpenAPI Documentation
    - [ ] Configure Swagger UI
    - [ ] Document all routes
    - [ ] Include authentication info
    - [ ] Example requests/responses
  
  - [ ] **1.10.2** API Gateway Documentation
    - [ ] Architecture overview
    - [ ] Configuration guide
    - [ ] Deployment guide
    - [ ] Troubleshooting guide
  
- [ ] **1.11** Deployment
  - [ ] **1.11.1** Docker Configuration
    - [ ] Create Dockerfile
    - [ ] Multi-stage build
    - [ ] Health check trong Dockerfile
  
  - [ ] **1.11.2** Configuration Management
    - [ ] Environment-specific configs (dev, staging, prod)
    - [ ] Secrets management (Azure Key Vault, AWS Secrets Manager)
    - [ ] Configuration validation on startup
  
  - [ ] **1.11.3** Deployment Scripts
    - [ ] CI/CD pipeline configuration
    - [ ] Deployment automation
    - [ ] Rollback procedures

---

### 2. AI Vision Service (Microservice)
**Status:** ⚠️ Not Started  
**Priority:** High  
**Technology:** Python (FastAPI/Flask) + YOLO

#### Tasks:
- [ ] **2.1** Setup AI Service Project
  - [ ] Tạo Python project structure
  - [ ] Setup FastAPI/Flask framework
  - [ ] Cấu hình Docker container
  
- [ ] **2.2** YOLO Model Integration
  - [ ] Download/install YOLOv8 hoặc YOLOv5
  - [ ] Train/customize model cho Vietnamese traffic signs
  - [ ] Load model và inference pipeline
  
- [ ] **2.3** Image Processing API
  - [ ] Endpoint: `POST /api/ai/detect`
    - Input: Image file (multipart/form-data)
    - Output: Detected signs với bounding boxes, confidence scores, classifications
  - [ ] Endpoint: `POST /api/ai/classify`
    - Input: Cropped sign image
    - Output: Sign type classification với confidence
  
- [ ] **2.4** Integration với Contribution Service
  - [ ] Auto-detect signs khi user upload image
  - [ ] Auto-classify sign type
  - [ ] Store detection results trong Contribution model
  
- [ ] **2.5** Model Management
  - [ ] Model versioning
  - [ ] Model update mechanism
  - [ ] Performance metrics tracking

---

### 3. Notification Service (Microservice)
**Status:** ⚠️ Partially Implemented (có NotificationService.cs nhưng chưa real-time)  
**Priority:** Medium

#### Tasks:
- [ ] **3.1** Real-time Notification System
  - [ ] Setup SignalR hoặc WebSocket service
  - [ ] Implement notification hub
  - [ ] Push notifications khi contribution được approve/reject
  
- [ ] **3.2** Email Notifications
  - [ ] Verify EmailService implementation
  - [ ] Send email khi contribution status thay đổi
  - [ ] Send welcome email khi user register
  
- [ ] **3.3** Mobile Push Notifications
  - [ ] Setup Firebase Cloud Messaging (FCM) hoặc Apple Push Notification (APN)
  - [ ] Register device tokens
  - [ ] Send push notifications cho mobile app

---

## 🔧 BACKEND IMPROVEMENTS

### 4. User Service Enhancements
**Status:** ✅ Partially Complete (có UserService, UserController)  
**Priority:** Medium

#### Tasks:
- [ ] **4.1** Reputation System Implementation
  - [ ] Calculate reputation based on:
    - Approved contributions (+X points)
    - Aligned votes (+Y points)
    - Time as user (seniority bonus)
  - [ ] Update reputation khi contribution được approve
  - [ ] Update reputation khi vote aligned với final decision
  
- [ ] **4.2** User Expertise Tracking
  - [ ] Track user expertise areas (e.g., "Highway signs", "Urban signs")
  - [ ] Calculate expertise score based on contribution history
  - [ ] Use expertise trong voting weight calculation

---

### 5. Voting Service Enhancements
**Status:** ✅ Partially Complete (có VoteService, VoteController)  
**Priority:** High

#### Tasks:
- [ ] **5.1** Weighted Voting Algorithm
  - [ ] Implement vote weight calculation:
    - Base weight = 1.0
    - Reputation multiplier: `1 + (reputation / 100)`
    - Proximity multiplier: `1 + (proximity_bonus)` nếu user gần location
    - Expertise multiplier: `1 + (expertise_match)` nếu user có expertise về sign type
  - [ ] Calculate weighted vote score cho contribution
  
- [ ] **5.2** Auto-Approval/Rejection Logic
  - [ ] Implement logic:
    - After 5+ votes OR 7 days:
      - Score > 70% → Auto-approve (nếu chưa có admin review)
      - Score < 30% → Auto-reject
      - Score 30-70% → Flag for admin review
  - [ ] Schedule background job để check và process
  
- [ ] **5.3** Vote Aligned Reward
  - [ ] Khi contribution được approve/reject:
    - Check votes aligned với final decision
    - Award 1 coin per aligned vote (max 5 coins/day per user)
    - Update user reputation

---

### 6. Contribution Service Enhancements
**Status:** ✅ Partially Complete (có ContributionService, ContributionController)  
**Priority:** High

#### Tasks:
- [ ] **6.1** Image Upload Integration
  - [ ] Setup file storage (Azure Blob Storage, AWS S3, hoặc local storage)
  - [ ] Endpoint để upload images
  - [ ] Auto-resize/optimize images
  - [ ] Store ImageUrl trong Contribution model
  
- [ ] **6.2** AI Detection Integration
  - [ ] Call AI Vision Service khi user upload image
  - [ ] Pre-fill contribution type, location (nếu có GPS) từ AI results
  - [ ] Store AI detection results
  
- [ ] **6.3** Coin Charging
  - [ ] Verify 5 coins deduction khi submit contribution
  - [ ] Check balance before allowing submission
  
- [ ] **6.4** Auto-Convert to TrafficSign
  - [ ] Khi contribution approved:
    - Create/update TrafficSign từ contribution data
    - Award 10+ coins to user
    - Send notification

---

### 7. Traffic Sign Service Enhancements
**Status:** ✅ Partially Complete (có TrafficSignService, TrafficSignController)  
**Priority:** Medium

#### Tasks:
- [ ] **7.1** OpenStreetMap Integration
  - [ ] Research OpenStreetMap API/Overpass API
  - [ ] Optional: Import existing signs từ OSM
  - [ ] Display signs trên map (handled by frontend)
  
- [ ] **7.2** Search & Filter with Coin Charging
  - [ ] Verify basic search (không tốn coin)
  - [ ] Implement advanced filter (type, proximity) - tốn 1 coin
  - [ ] Check balance before advanced search
  
- [ ] **7.3** Hungarian Algorithm Implementation
  - [ ] Implement algorithm để detect:
    - Modifications (signs updated)
    - Insertions (new signs)
    - Deletions (missing signs)
  - [ ] Compare current dataset với historical data
  - [ ] Generate change reports

---

### 8. Payment Service Enhancements
**Status:** ✅ Partially Complete (có PaymentService, PaymentController)  
**Priority:** Medium

#### Tasks:
- [ ] **8.1** Payment Gateway Integration
  - [ ] Integrate payment gateway (VnPay, MoMo, PayPal, etc.)
  - [ ] Handle payment callbacks
  - [ ] Update payment status
  - [ ] Credit coins khi payment successful
  
- [ ] **8.2** Coin Top-up Flow
  - [ ] Rate: $1 = 10 coins (hoặc VND equivalent)
  - [ ] Create payment record
  - [ ] Process payment
  - [ ] Credit wallet upon success

---

## 📱 MOBILE APP DEVELOPMENT

### 9. Mobile App (iOS/Android)
**Status:** ⚠️ Not Started  
**Priority:** High  
**Technology:** React Native hoặc Flutter

#### Tasks:
- [ ] **9.1** Project Setup
  - [ ] Choose framework (React Native recommended)
  - [ ] Setup project structure
  - [ ] Configure API client
  
- [ ] **9.2** Authentication & User Management
  - [ ] Login/Register screens
  - [ ] JWT token storage
  - [ ] User profile management
  - [ ] Coin wallet display
  
- [ ] **9.3** Map Integration
  - [ ] Integrate OpenStreetMap hoặc Mapbox
  - [ ] Display traffic signs on map
  - [ ] Real-time updates khi signs được approve
  - [ ] Map markers với sign types
  
- [ ] **9.4** Contribution Features
  - [ ] Submit new sign contribution
  - [ ] Camera integration để capture sign images
  - [ ] GPS location capture
  - [ ] Preview AI detection results
  - [ ] Submit contribution (tốn 5 coins)
  
- [ ] **9.5** Voting Features
  - [ ] View pending contributions
  - [ ] Upvote/downvote contributions
  - [ ] View voting history
  - [ ] Display vote alignment rewards
  
- [ ] **9.6** Search & Filter
  - [ ] Basic search (free)
  - [ ] Advanced filter với coin charging (1 coin)
  - [ ] Filter by type, proximity
  
- [ ] **9.7** Notifications
  - [ ] Push notifications setup
  - [ ] In-app notification list
  - [ ] Real-time updates
  
- [ ] **9.8** Payment Integration
  - [ ] Coin top-up screen
  - [ ] Payment gateway integration
  - [ ] Transaction history

---

## 🌐 FRONTEND WEB APP (ADMIN)

### 10. Admin Web App Enhancements
**Status:** ✅ Partially Complete (có React app với 9 pages)  
**Priority:** Medium

#### Tasks:
- [ ] **10.1** Complete Missing Features
  - [ ] Verify all API integrations work
  - [ ] Fix any bugs
  - [ ] Add loading states
  - [ ] Error handling improvements
  
- [ ] **10.2** Voting Management
  - [ ] Display vote weights
  - [ ] Show weighted scores
  - [ ] Override vote outcomes (admin only)
  
- [ ] **10.3** Analytics Dashboard
  - [ ] User activity charts
  - [ ] Contribution statistics
  - [ ] Voting trends
  - [ ] Coin economy metrics
  
- [ ] **10.4** Real-time Updates
  - [ ] WebSocket/SignalR integration
  - [ ] Real-time contribution updates
  - [ ] Live notification feed

---

## 🔐 SECURITY & INFRASTRUCTURE

### 11. Security Enhancements
**Status:** ⚠️ Partially Implemented (có JWT, password hashing)  
**Priority:** High

#### Tasks:
- [ ] **11.1** API Security
  - [ ] CORS configuration
  - [ ] Rate limiting per endpoint
  - [ ] Input validation & sanitization
  - [ ] SQL injection prevention (EF Core đã có)
  
- [ ] **11.2** Authentication Security
  - [ ] Password strength requirements
  - [ ] Token expiration & refresh
  - [ ] Multi-factor authentication (optional)
  
- [ ] **11.3** Data Protection
  - [ ] Encrypt sensitive data
  - [ ] Secure file uploads
  - [ ] GDPR compliance (if needed)

---

### 12. Infrastructure & DevOps
**Status:** ⚠️ Not Started  
**Priority:** Medium

#### Tasks:
- [ ] **12.1** Database
  - [ ] Setup production SQL Server
  - [ ] Database migrations strategy
  - [ ] Backup & recovery plan
  
- [ ] **12.2** Deployment
  - [ ] Deploy API services to Azure/AWS
  - [ ] Deploy AI service to cloud (GPU instance)
  - [ ] Deploy admin web app
  - [ ] Setup CI/CD pipeline
  
- [ ] **12.3** Monitoring & Logging
  - [ ] Application Insights hoặc similar
  - [ ] Error tracking (Sentry, etc.)
  - [ ] Performance monitoring
  - [ ] Log aggregation

---

## 📚 DOCUMENTATION

### 13. Documentation
**Status:** ⚠️ Partially Complete  
**Priority:** Medium

#### Tasks:
- [ ] **13.1** Technical Documentation
  - [ ] API documentation (Swagger đã có, cần verify)
  - [✅] Architecture diagram
  - [ ] Database schema documentation
  - [ ] Microservices communication flow
  
- [ ] **13.2** User Documentation
  - [ ] User guide for mobile app
  - [ ] Admin manual
  - [ ] Installation guide
  - [ ] Deployment guide
  
- [ ] **13.3** Development Documentation
  - [ ] Setup guide
  - [ ] Coding standards
  - [ ] Testing strategy
  - [ ] Contribution guidelines

---

## 🧪 TESTING

### 14. Testing
**Status:** ⚠️ Not Started  
**Priority:** Medium

#### Tasks:
- [ ] **14.1** Unit Tests
  - [ ] Service layer tests
  - [ ] Controller tests
  - [ ] Utility function tests
  
- [ ] **14.2** Integration Tests
  - [ ] API endpoint tests
  - [ ] Database integration tests
  - [ ] Microservices communication tests
  
- [ ] **14.3** E2E Tests
  - [ ] Mobile app flows
  - [ ] Admin panel flows
  - [ ] Complete user journeys

---

## 📊 PRIORITY SUMMARY

### High Priority (Must Have)
1. ✅ API Gateway Service
2. ✅ AI Vision Service
3. ✅ Voting Service Enhancements (weighted voting)
4. ✅ Mobile App Development
5. ✅ Contribution Service Enhancements (image upload, AI integration)

### Medium Priority (Should Have)
1. ✅ Notification Service (real-time)
2. ✅ Payment Gateway Integration
3. ✅ Admin Web App Enhancements
4. ✅ Security Enhancements
5. ✅ OpenStreetMap Integration

### Low Priority (Nice to Have)
1. ✅ Hungarian Algorithm
2. ✅ Advanced Analytics
3. ✅ Multi-factor Authentication
4. ✅ Comprehensive Testing Suite

---

## 📝 NOTES

- **Current Status:** Backend API đã có cơ bản, Admin web app đã có UI, cần hoàn thiện logic và tích hợp microservices
- **Next Steps:** 
  1. Setup API Gateway
  2. Implement AI Vision Service
  3. Enhance Voting Service với weighted algorithm
  4. Develop Mobile App
  5. Integrate real-time notifications

- **Technology Stack:**
  - Backend: .NET 8 (C#)
  - Database: SQL Server
  - Frontend Admin: React + Material-UI
  - Mobile: React Native (recommended) hoặc Flutter
  - AI: Python + YOLO
  - API Gateway: .NET hoặc Node.js

---

**Last Updated:** 2025

