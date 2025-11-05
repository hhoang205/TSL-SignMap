# Database ERD - SignMap

## Entity Relationship Diagram

```mermaid
erDiagram
    User ||--o| CoinWallet : "has one"
    User ||--o{ Contribution : "creates"
    User ||--o{ Vote : "casts"
    User ||--o{ Notification : "receives"
    User ||--o{ Feedback : "submits"
    User ||--o{ Payment : "makes"

    TrafficSign ||--o{ Contribution : "has"
    Contribution ||--o{ Vote : "receives"

    User {
        int Id PK
        string Username UK
        string Email UK
        string Password
        string Firstname
        string Lastname
        int RoleId
        string PhoneNumber
        float Reputation
        datetime CreatedAt
        datetime UpdatedAt
    }

    CoinWallet {
        int Id PK
        int UserId FK
        decimal Balance
        datetime CreatedAt
        datetime UpdatedAt
    }

    TrafficSign {
        int Id PK
        string Type
        geometry Location
        string Status
        string ImageUrl
        datetime ValidFrom
        datetime ValidTo
    }

    Contribution {
        int Id PK
        int UserId FK
        int SignId FK
        int TrafficSignId FK
        string Action
        string Description
        string Status
        string ImageUrl
        string Type
        double Latitude
        double Longitude
        datetime CreatedAt
    }

    Vote {
        int Id PK
        int UserId FK
        int ContributionId FK
        int Value
        bool IsUpvote
        float Weight
        datetime CreatedAt
    }

    Notification {
        int Id PK
        int UserId FK
        string Title
        string Message
        bool IsRead
        datetime CreatedAt
        datetime UpdatedAt
    }

    Feedback {
        int Id PK
        int UserId FK
        string Content
        string Status
        datetime CreatedAt
        datetime ResolvedAt
    }

    Payment {
        int Id PK
        int UserId FK
        decimal Amount
        datetime PaymentDate
        string PaymentMethod
        string Status
    }
```

## Mối quan hệ chi tiết

### User Relationships
- **1:1 với CoinWallet:** Mỗi user có một wallet
- **1:N với Contribution:** User có thể tạo nhiều contributions
- **1:N với Vote:** User có thể vote nhiều contributions
- **1:N với Notification:** User nhận nhiều notifications
- **1:N với Feedback:** User có thể submit nhiều feedbacks
- **1:N với Payment:** User có thể thực hiện nhiều payments

### Contribution Relationships
- **N:1 với User:** Mỗi contribution thuộc về một user
- **N:1 với TrafficSign:** Contribution có thể reference một TrafficSign (cho Update/Delete actions)
- **1:N với Vote:** Mỗi contribution nhận nhiều votes

### Constraints
- **Unique:** Username, Email trong User
- **Unique:** UserId trong CoinWallet (1:1 relationship)
- **Unique:** (ContributionId, UserId) trong Vote (một user chỉ vote một lần cho mỗi contribution)

## Indexes
- User.Username (Unique)
- User.Email (Unique)
- CoinWallet.UserId (Unique)
- Vote(ContributionId, UserId) (Unique)

