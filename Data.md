USE [TFSIGN];
GO

/* =========================
   0. Xoá dữ liệu & reset ID
   ========================= */

DELETE FROM dbo.Votes;
DELETE FROM dbo.Contributions;
DELETE FROM dbo.Payments;
DELETE FROM dbo.Feedbacks;
DELETE FROM dbo.Notifications;
DELETE FROM dbo.CoinWallets;
DELETE FROM dbo.TrafficSigns;
DELETE FROM dbo.Users;
GO

DBCC CHECKIDENT ('dbo.Users', RESEED, 0);
DBCC CHECKIDENT ('dbo.CoinWallets', RESEED, 0);
DBCC CHECKIDENT ('dbo.TrafficSigns', RESEED, 0);
DBCC CHECKIDENT ('dbo.Contributions', RESEED, 0);
DBCC CHECKIDENT ('dbo.Votes', RESEED, 0);
DBCC CHECKIDENT ('dbo.Payments', RESEED, 0);
DBCC CHECKIDENT ('dbo.Feedbacks', RESEED, 0);
DBCC CHECKIDENT ('dbo.Notifications', RESEED, 0);
GO

/* =========================
   1. Users + CoinWallets
   ========================= */
-- Gợi ý: tạo admin bằng script CreateAdminUser để có password hash đúng.
-- Ở đây tạm seed thêm 2 user thường + 1 staff với password giả (tuỳ bạn xử lý hash).

INSERT INTO dbo.Users
    (Username, Password, Lastname, Firstname, RoleId, Email, PhoneNumber, Reputation, CreatedAt, UpdatedAt)
VALUES
    ('admin',  'Admin123@_PLAIN_OR_HASH', 'Admin', 'System', 2, 'admin@tsl.com', '0900000001', 5.0, GETUTCDATE(), GETUTCDATE()),
    ('staff1', 'Staff123@_PLAIN_OR_HASH', 'Staff', 'Nguyen', 1, 'staff1@tsl.com', '0900000002', 3.5, GETUTCDATE(), GETUTCDATE()),
    ('user1',  'User123@_PLAIN_OR_HASH',  'User',  'Le',     0, 'user1@tsl.com', '0900000003', 1.2, GETUTCDATE(), GETUTCDATE());
GO
-- Giờ Id sẽ là: admin=1, staff1=2, user1=3

INSERT INTO dbo.CoinWallets (UserId, Balance, CreatedAt, UpdatedAt)
VALUES
    (1, 1000.00, GETUTCDATE(), NULL),
    (2,  200.00, GETUTCDATE(), NULL),
    (3,   50.00, GETUTCDATE(), NULL);
GO

/* =========================
   2. TrafficSigns
   ========================= */

INSERT INTO dbo.TrafficSigns
    (Type, Location, Status, ImageUrl, ValidFrom, ValidTo, Traffic)
VALUES
    (N'No Parking',
     geometry::STPointFromText('POINT(106.7000 10.7769)', 4326),
     'Active',
     'https://example.com/signs/no-parking-1.jpg',
     GETUTCDATE(),
     DATEADD(year, 5, GETUTCDATE()),
     '["Car","Motorbike"]'),

    (N'Speed Limit 50',
     geometry::STPointFromText('POINT(106.6800 10.7900)', 4326),
     'Active',
     'https://example.com/signs/speed-50-1.jpg',
     GETUTCDATE(),
     DATEADD(year, 5, GETUTCDATE()),
     '["Car","Truck"]'),

    (N'One Way',
     geometry::STPointFromText('POINT(106.6950 10.7700)', 4326),
     'Under Maintenance',
     'https://example.com/signs/one-way-1.jpg',
     GETUTCDATE(),
     DATEADD(year, 3, GETUTCDATE()),
     '["Car","Bus","Motorbike"]');
GO
-- Id biển báo: 1,2,3

/* =========================
   3. Contributions
   ========================= */

INSERT INTO dbo.Contributions
    (UserId, SignId, TrafficSignId, Action, Description, Status, ImageUrl,
     CreatedAt, Type, Latitude, Longitude)
VALUES
    -- #1: user1 đề xuất thêm biển cấm đỗ mới
    (3, 0, 0, 'Add',
     N'Thêm biển cấm đỗ gần trường học vì thường xuyên kẹt xe.',
     'Pending',
     'https://example.com/contributions/no-parking-school.jpg',
     GETUTCDATE(),
     'No Parking', 10.8010, 106.7005),

    -- #2: staff1 cập nhật biển giới hạn tốc độ (biển Id=2)
    (2, 2, 2, 'Update',
     N'Tăng giới hạn tốc độ từ 50 lên 60 km/h do đường đã mở rộng.',
     'Approved',
     'https://example.com/contributions/speed-50-1-update.jpg',
     DATEADD(day,-2,GETUTCDATE()),
     'Speed Limit 60', 10.7900, 106.6800),

    -- #3: user1 đề xuất xoá biển đường một chiều (biển Id=3)
    (3, 3, 3, 'Delete',
     N'Đề xuất gỡ biển đường một chiều vì đã đổi hướng lưu thông.',
     'Rejected',
     'https://example.com/contributions/one-way-remove.jpg',
     DATEADD(day,-5,GETUTCDATE()),
     'One Way', 10.7700, 106.6950);
GO
-- Id góp ý: 1,2,3

/* =========================
   4. Votes
   ========================= */

INSERT INTO dbo.Votes
    (UserId, ContributionId, Value, IsUpvote, Weight, CreatedAt)
VALUES
    -- Vote cho góp ý 1
    (1, 1, 1, 1, 1.0, DATEADD(hour,-5,GETUTCDATE())),  -- admin
    (2, 1, 1, 1, 0.8, DATEADD(hour,-4,GETUTCDATE())),  -- staff1

    -- Vote cho góp ý 2
    (1, 2, 1, 1, 1.0, DATEADD(hour,-3,GETUTCDATE())),  -- admin upvote
    (3, 2,-1, 0, 0.5, DATEADD(hour,-2,GETUTCDATE()));  -- user1 downvote
GO

/* =========================
   5. Payments
   ========================= */

INSERT INTO dbo.Payments
    (UserId, Amount, PaymentDate, PaymentMethod, Status)
VALUES
    (1, 100.00, DATEADD(day,-7,GETUTCDATE()), 'Credit Card',   'Completed'),
    (1,  50.00, DATEADD(day,-1,GETUTCDATE()), 'Credit Card',   'Completed'),
    (2,  20.00, DATEADD(day,-3,GETUTCDATE()), 'PayPal',        'Completed'),
    (3,  10.00, DATEADD(day,-2,GETUTCDATE()), 'Bank Transfer', 'Failed');
GO

/* =========================
   6. Feedbacks
   ========================= */

INSERT INTO dbo.Feedbacks
    (UserId, Content, Status, CreatedAt, ResolvedAt)
VALUES
    (3, N'Ứng dụng rất hữu ích nhưng tốc độ tải bản đồ hơi chậm.',
        'Pending',  DATEADD(day,-1,GETUTCDATE()), NULL),

    (2, N'Nên thêm chức năng lọc biển báo theo loại và trạng thái.',
        'Reviewed', DATEADD(day,-5,GETUTCDATE()), DATEADD(day,-2,GETUTCDATE())),

    (1, N'Cần dashboard tổng hợp số lượng góp ý theo khu vực.',
        'Resolved', DATEADD(day,-10,GETUTCDATE()), DATEADD(day,-7,GETUTCDATE()));
GO

/* =========================
   7. Notifications (demo đơn giản)
   ========================= */

INSERT INTO dbo.Notifications
    (UserId, Title, Message, IsRead, CreatedAt)
VALUES
    (3, N'Góp ý của bạn đã được nhận', N'Chúng tôi đang xem xét góp ý về biển cấm đỗ.', 0, DATEADD(hour,-6,GETUTCDATE())),
    (2, N'Góp ý đã được duyệt',       N'Góp ý cập nhật tốc độ đã được admin phê duyệt.', 1, DATEADD(hour,-12,GETUTCDATE())),
    (1, N'Báo cáo hệ thống',          N'Hệ thống có 3 góp ý mới trong tuần này.',        0, DATEADD(day,-1,GETUTCDATE()));
GO