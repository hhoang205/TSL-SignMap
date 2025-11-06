# TrafficSignService

Microservice quản lý traffic signs.

## Port
- HTTP: 5002
- HTTPS: 7002

## Chức năng

- Hiển thị real-time traffic sign locations (không tốn coin)
- Search và filter traffic signs
- Advanced filters (type, proximity) tốn 1 coin (gọi UserService để debit)

## API Endpoints

- `GET /api/signs` - Lấy tất cả active signs (không tốn coin)
- `GET /api/signs/{id}` - Lấy sign theo ID
- `POST /api/signs/search` - Tìm kiếm (tốn 1 coin nếu dùng advanced filters)
- `POST /api/signs/filter/proximity` - Filter theo bán kính (tốn 1 coin)
- `GET /api/signs/filter/type/{type}` - Filter theo type (tốn 1 coin)
- `POST /api/signs` - Tạo sign mới
- `PUT /api/signs/{id}` - Cập nhật sign
- `DELETE /api/signs/{id}` - Xóa sign

## Dependencies

- UserService (HTTP) - Để check balance và debit coins cho advanced filters

## Database

- Connection string trong `appsettings.json`
- DbContext: `TrafficSignDbContext`
- Tables: `TrafficSigns`

## Running

```bash
cd TrafficSignService
dotnet restore
dotnet run
```

Swagger UI: http://localhost:5002/swagger

