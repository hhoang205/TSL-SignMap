# Scripts for UserService

## Create Admin User

Script để tạo tài khoản admin cho TSL Admin Panel.

### Cách sử dụng

#### Option 1: Sử dụng PowerShell script (Khuyến nghị)

```powershell
cd Server\UserService\Scripts
.\create-admin-simple.ps1
```

#### Option 2: Chạy trực tiếp console app

```powershell
cd Server\UserService\Scripts\CreateAdminUser
dotnet restore
dotnet build
dotnet run
```

### Thông tin đăng nhập Admin

Sau khi chạy script thành công, bạn có thể đăng nhập vào admin panel với:

- **Email**: `admin@tsl.com`
- **Password**: `Admin123@`

### Lưu ý

- Script sẽ kiểm tra xem admin user đã tồn tại chưa
- Nếu đã tồn tại, script sẽ cập nhật RoleId thành 2 (Admin) nếu chưa phải admin
- Admin user sẽ có 1000 coins trong wallet
- Đảm bảo database đã được tạo và migrations đã chạy trước khi chạy script

### Yêu cầu

- .NET 8.0 SDK
- SQL Server đang chạy
- Database TFSIGN đã được tạo
- Migrations đã được apply

