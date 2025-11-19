# System Architecture Diagram - SignMap

## Tổng quan kiến trúc hệ thống

```mermaid
graph TB
    subgraph "Client Layer"
        MA[Mobile App<br/>React Native]
        WEB[Admin Web App<br/>React + Material-UI]
    end

    subgraph "API Gateway Layer"
        GW[API Gateway<br/>.NET/Node.js<br/>- Authentication<br/>- Routing<br/>- Rate Limiting]
    end

    subgraph "Microservices Layer"
        US[User Service<br/>.NET 8<br/>- Auth<br/>- User Mgmt<br/>- Reputation]
        TS[Traffic Sign Service<br/>.NET 8<br/>- CRUD Operations<br/>- Search & Filter]
        CS[Contribution Service<br/>.NET 8<br/>- Submit/Review<br/>- Auto-convert]
        VS[Voting Service<br/>.NET 8<br/>- Weighted Voting<br/>- Auto-approval]
        AI[AI Vision Service<br/>Python + FastAPI<br/>- YOLO Detection<br/>- Classification]
        NS[Notification Service<br/>.NET 8 + SignalR<br/>- Real-time<br/>- Email<br/>- Push]
        PS[Payment Service<br/>.NET 8<br/>- Payment Gateway<br/>- Coin Top-up]
    end

    subgraph "Data Layer"
        DB[(SQL Server<br/>- Users<br/>- Traffic Signs<br/>- Contributions<br/>- Votes<br/>- Payments)]
        STORAGE[File Storage<br/>Azure Blob/S3<br/>- Images<br/>- Documents]
    end

    subgraph "External Services"
        OSM[OpenStreetMap API<br/>Map Integration]
        EMAIL[Email Service<br/>SMTP/SendGrid]
        FCM[Firebase Cloud Messaging<br/>Push Notifications]
        PAY[Payment Gateway<br/>VnPay/MoMo/PayPal]
    end

    MA -->|HTTPS| GW
    WEB -->|HTTPS| GW

    GW -->|Route| US
    GW -->|Route| TS
    GW -->|Route| CS
    GW -->|Route| VS
    GW -->|Route| AI
    GW -->|Route| NS
    GW -->|Route| PS

    US --> DB
    TS --> DB
    CS --> DB
    VS --> DB
    NS --> DB
    PS --> DB

    CS -->|Upload| STORAGE
    TS -->|Read| STORAGE
    AI -->|Read| STORAGE

    CS -->|Call| AI
    TS -->|Fetch| OSM
    NS -->|Send| EMAIL
    NS -->|Send| FCM
    PS -->|Process| PAY

    style GW fill:#4A90E2
    style AI fill:#FF6B6B
    style DB fill:#50C878
    style STORAGE fill:#FFA500
```

## Chi tiết các thành phần

### API Gateway
- **Chức năng:** Entry point duy nhất, xử lý authentication, routing, rate limiting
- **Công nghệ:** .NET 8 hoặc Node.js/Express

### Microservices
- **User Service:** Quản lý users, authentication, reputation system
- **Traffic Sign Service:** CRUD operations, search với coin charging
- **Contribution Service:** Submit contributions, admin review, auto-convert to TrafficSign
- **Voting Service:** Weighted voting algorithm, auto-approval/rejection
- **AI Vision Service:** YOLO-based detection và classification
- **Notification Service:** Real-time notifications (SignalR), email, push
- **Payment Service:** Payment gateway integration, coin wallet management

### Data Layer
- **SQL Server:** Primary database cho tất cả entities
- **File Storage:** Azure Blob Storage hoặc AWS S3 cho images

### External Services
- **OpenStreetMap:** Map integration và display
- **Email Service:** SMTP hoặc SendGrid
- **FCM:** Push notifications cho mobile
- **Payment Gateway:** VnPay, MoMo, PayPal

