# NotificationService

Notification Service quản lý các thông báo (notifications) cho users trong hệ thống Traffic Sign.

## Port
- **HTTP**: 5005
- **HTTPS**: 7005

## Features

- ✅ CRUD operations cho Notifications
- ✅ Real-time notifications qua SignalR
- ✅ Filter và pagination
- ✅ Đếm số notifications chưa đọc
- ✅ Đánh dấu đã đọc (single và all)
- ✅ Validate User tồn tại qua UserService

## API Endpoints

### Notifications

- `GET /api/notifications/user/{userId}` - Lấy tất cả notifications của user
- `GET /api/notifications/user/{userId}/unread` - Lấy notifications chưa đọc
- `GET /api/notifications/user/{userId}/unread/count` - Đếm số notifications chưa đọc
- `GET /api/notifications/{id}` - Lấy notification theo ID
- `POST /api/notifications` - Tạo notification mới
- `PUT /api/notifications/{id}/read` - Đánh dấu đã đọc
- `PUT /api/notifications/user/{userId}/read-all` - Đánh dấu tất cả đã đọc
- `POST /api/notifications/filter` - Filter notifications
- `DELETE /api/notifications/{id}` - Xóa notification

### SignalR Hub

- **Endpoint**: `/notificationHub`
- **Methods**:
  - `JoinUserGroup(int userId)` - Join group để nhận notifications của user
  - `LeaveUserGroup(int userId)` - Leave group
- **Client Events**:
  - `ReceiveNotification` - Nhận notification mới
  - `NotificationRead` - Notification đã được đọc
  - `AllNotificationsRead` - Tất cả notifications đã được đọc
  - `NotificationDeleted` - Notification đã bị xóa

## Inter-Service Communication

### Dependencies
- **UserService** (Port 5001) - Validate User tồn tại khi tạo notification

## Database

- **Table**: `Notifications`
- **Indexes**: 
  - `UserId`
  - `UserId, IsRead` (composite)

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "ServiceEndpoints": {
    "UserService": "http://localhost:5001"
  }
}
```

## Usage Example

### Create Notification (from other services)

```csharp
var request = new NotificationCreateRequest
{
    UserId = 1,
    Title = "Contribution Approved",
    Message = "Your contribution has been approved!"
};

var response = await httpClient.PostAsJsonAsync(
    "http://localhost:5005/api/notifications", 
    request
);
```

### Connect to SignalR (JavaScript)

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5005/notificationHub")
    .build();

connection.on("ReceiveNotification", (notification) => {
    console.log("New notification:", notification);
});

connection.start().then(() => {
    connection.invoke("JoinUserGroup", userId);
});
```

## Requirements

- .NET 8.0
- SQL Server
- UserService running on port 5001

