# VotingService

Microservice quản lý Votes cho hệ thống Traffic Sign.

## Port
- **HTTP**: 5004
- **HTTPS**: 7004

## Features
- Users can vote on contributions (upvote/downvote)
- Votes có weight (1.0 for full vote, có thể giảm dần)
- Mỗi user chỉ có thể vote 1 lần cho mỗi contribution (unique constraint)
- Validate User và Contribution tồn tại qua HTTP calls

## API Endpoints

### Votes
- `GET /api/votes/{id}` - Lấy vote theo ID
- `GET /api/votes/contribution/{contributionId}` - Lấy tất cả votes của một contribution
- `GET /api/votes/user/{userId}` - Lấy tất cả votes của một user
- `GET /api/votes/contribution/{contributionId}/summary` - Lấy tổng hợp votes
- `POST /api/votes` - Tạo vote mới
- `PUT /api/votes/{id}` - Cập nhật vote
- `DELETE /api/votes/{id}` - Xóa vote
- `POST /api/votes/filter` - Filter votes với các điều kiện

## Dependencies
- **UserService** (Port 5001) - Validate User tồn tại
- **ContributionService** (Port 5003) - Validate Contribution tồn tại

## Database
- Table: `Votes`
- Unique constraint: `(ContributionId, UserId)`

## Configuration
- Connection string: `DefaultConnection`
- Service endpoints: `UserService`, `ContributionService`

