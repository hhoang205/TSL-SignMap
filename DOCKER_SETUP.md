# Docker Setup Guide

Hướng dẫn setup và chạy các microservices bằng Docker.

## 📋 Prerequisites

- Docker Desktop (Windows/Mac) hoặc Docker Engine (Linux)
- Docker Compose v2.0+
- Ít nhất 4GB RAM available
- Ports 5000-5007 và 1433 phải available

## 🚀 Quick Start

### 1. Build và Start tất cả services

```bash
docker-compose up -d --build
```

Lệnh này sẽ:
- Build Docker images cho tất cả services
- Start SQL Server container
- Start tất cả 7 microservices
- Start API Gateway
- Tạo network và volumes cần thiết

### 2. Kiểm tra services đang chạy

```bash
docker-compose ps
```

### 3. Xem logs

```bash
# Xem logs của tất cả services
docker-compose logs -f

# Xem logs của một service cụ thể
docker-compose logs -f user-service
docker-compose logs -f api-gateway
```

### 4. Stop services

```bash
docker-compose down
```

### 5. Stop và xóa volumes (database data)

```bash
docker-compose down -v
```

## 🏗️ Services và Ports

| Service | Container Name | Port | Health Check |
|---------|---------------|------|--------------|
| API Gateway | api-gateway | 5000 | http://localhost:5000/api/health |
| User Service | user-service | 5001 | http://localhost:5001/health |
| Traffic Sign Service | traffic-sign-service | 5002 | http://localhost:5002/health |
| Contribution Service | contribution-service | 5003 | http://localhost:5003/health |
| Voting Service | voting-service | 5004 | http://localhost:5004/health |
| Notification Service | notification-service | 5005 | http://localhost:5005/health |
| Payment Service | payment-service | 5006 | http://localhost:5006/health |
| Feedback Service | feedback-service | 5007 | http://localhost:5007/health |
| SQL Server | traffic-sign-sqlserver | 1433 | Internal health check |

## 🔧 Configuration

### Environment Variables

Các services được configure qua environment variables trong `docker-compose.yml`:

- **ConnectionStrings__DefaultConnection**: SQL Server connection string
- **ServiceEndpoints__[ServiceName]**: URLs của các services khác
- **ASPNETCORE_ENVIRONMENT**: Development/Production
- **ASPNETCORE_URLS**: URLs mà service listen

### Database Connection

SQL Server container:
- **Server**: `sqlserver` (internal) hoặc `localhost` (external)
- **Port**: `1433`
- **Username**: `SA`
- **Password**: `Admin123@`
- **Database**: `TFSIGN`

Connection string format:
```
Server=sqlserver,1433;Database=TFSIGN;User Id=SA;Password=Admin123@;TrustServerCertificate=True;MultipleActiveResultSets=True;
```

## 📝 Development Workflow

### 1. Development với Hot Reload

Tạo file `docker-compose.override.yml` (copy từ `docker-compose.override.yml.example`):

```bash
cp docker-compose.override.yml.example docker-compose.override.yml
```

File này sẽ mount source code vào containers để enable hot reload.

### 2. Rebuild một service cụ thể

```bash
docker-compose build user-service
docker-compose up -d user-service
```

### 3. Restart một service

```bash
docker-compose restart user-service
```

### 4. Execute commands trong container

```bash
# Run migrations
docker-compose exec user-service dotnet ef database update

# Access SQL Server
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P Admin123@
```

## 🐛 Troubleshooting

### Services không start

1. Kiểm tra logs:
```bash
docker-compose logs [service-name]
```

2. Kiểm tra ports đã được sử dụng:
```bash
netstat -ano | findstr :5001
```

3. Kiểm tra SQL Server đã ready:
```bash
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P Admin123@ -Q "SELECT 1"
```

### Database connection issues

1. Đảm bảo SQL Server container đã healthy:
```bash
docker-compose ps sqlserver
```

2. Kiểm tra connection string trong environment variables

3. Test connection từ service container:
```bash
docker-compose exec user-service dotnet ef dbcontext info
```

### Build failures

1. Clear Docker cache:
```bash
docker system prune -a
```

2. Rebuild without cache:
```bash
docker-compose build --no-cache
```

### Port conflicts

Nếu ports đã được sử dụng, có thể:
1. Stop services đang dùng ports đó
2. Hoặc thay đổi ports trong `docker-compose.yml`

## 🔒 Production Considerations

### Security

1. **Change default passwords**: Đổi `SA_PASSWORD` trong production
2. **Use secrets**: Sử dụng Docker secrets cho sensitive data
3. **Network isolation**: Tách networks cho production
4. **SSL/TLS**: Enable HTTPS cho production

### Performance

1. **Resource limits**: Set CPU và memory limits cho containers
2. **Database optimization**: Tune SQL Server settings
3. **Caching**: Enable Redis caching nếu cần
4. **Load balancing**: Setup load balancer cho high availability

### Monitoring

1. **Health checks**: Tất cả services đã có health checks
2. **Logging**: Centralize logs với ELK stack hoặc similar
3. **Metrics**: Setup Prometheus/Grafana
4. **Tracing**: Implement distributed tracing

## 📚 Additional Resources

- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
- [SQL Server Docker Image](https://hub.docker.com/_/microsoft-mssql-server)

## 🎯 Next Steps

1. ✅ Setup Docker containers
2. Run database migrations
3. Test inter-service communication
4. Setup CI/CD pipeline
5. Configure monitoring and logging
6. Setup production environment

