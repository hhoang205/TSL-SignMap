# FeedbackService

Microservice quản lý feedback và báo cáo từ users.

## Port
- **HTTP**: 5007
- **HTTPS**: 7007

## Features
- Users có thể submit feedback hoặc report issues với app hoặc database
- Hỗ trợ reporting inappropriate content hoặc misuse
- Admins có thể review và manage feedback status (Pending, Reviewed, Resolved)
- Filter và pagination
- Feedback summary statistics

## API Endpoints

### GET `/api/feedbacks/{id}`
Lấy feedback theo ID.

### GET `/api/feedbacks/user/{userId}`
Lấy tất cả feedbacks của một user.

### GET `/api/feedbacks/status/{status}`
Lấy tất cả feedbacks theo status (Pending, Reviewed, Resolved).

### GET `/api/feedbacks/summary` (Admin only)
Lấy tổng hợp feedbacks (statistics).

### POST `/api/feedbacks`
Tạo feedback mới.

**Request Body:**
```json
{
  "userId": 1,
  "content": "This is a feedback message"
}
```

### PUT `/api/feedbacks/{id}` (Admin only)
Cập nhật feedback (content và status).

**Request Body:**
```json
{
  "content": "Updated content",
  "status": "Reviewed"
}
```

### PUT `/api/feedbacks/{id}/status` (Admin only)
Cập nhật status của feedback.

**Request Body:**
```json
{
  "status": "Resolved",
  "autoResolve": true
}
```

### DELETE `/api/feedbacks/{id}` (Admin only)
Xóa feedback.

### POST `/api/feedbacks/filter` (Admin only)
Filter feedbacks với các điều kiện.

**Request Body:**
```json
{
  "userId": 1,
  "status": "Pending",
  "startDate": "2024-01-01T00:00:00Z",
  "endDate": "2024-12-31T23:59:59Z",
  "search": "keyword",
  "isResolved": false,
  "pageNumber": 1,
  "pageSize": 20
}
```

## Inter-Service Communication

### UserService (Port 5001)
- **Validate User**: `GET /api/users/{userId}` - Validate user tồn tại khi tạo feedback
- **Get Username**: `GET /api/users/{userId}` - Lấy username để hiển thị trong FeedbackDto

## Database
- **Database Name**: `WebAppTrafficSign_Feedback`
- **Table**: `Feedbacks`

## Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WebAppTrafficSign_Feedback;..."
  },
  "ServiceEndpoints": {
    "UserService": "http://localhost:5001"
  }
}
```

## Authentication
Tất cả endpoints yêu cầu JWT authentication. Một số endpoints yêu cầu Admin role.

## Status Values
- `Pending`: Feedback mới được tạo, chưa được review
- `Reviewed`: Feedback đã được admin review
- `Resolved`: Feedback đã được giải quyết

## Auto Resolve
Khi update status sang "Resolved" với `autoResolve: true`, hệ thống sẽ tự động set `ResolvedAt` = current time.

