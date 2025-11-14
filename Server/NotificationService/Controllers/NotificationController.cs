using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NotificationService.DTOs;
using NotificationService.Hubs;
using NotificationService.Services;

namespace NotificationService.Controllers
{
    /// API controller quản lý các thông báo (Notification).  
    /// Cho phép xem, tạo, cập nhật trạng thái đọc và xoá thông báo.
    /// Theo requirement: Users receive real-time notifications about approved changes and submission outcomes.
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            INotificationService notificationService,
            IHubContext<NotificationHub> hubContext,
            ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _hubContext = hubContext;
            _logger = logger;
        }

        /// Lấy tất cả notifications của user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId([FromRoute] int userId)
        {
            try
            {
                var notifications = await _notificationService.GetByUserIdAsync(userId);
                return Ok(new { message = "Lấy danh sách notifications thành công", data = notifications });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications for user {UserId}", userId);
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Lấy notifications chưa đọc của user
        [HttpGet("user/{userId}/unread")]
        public async Task<IActionResult> GetUnreadByUserId([FromRoute] int userId)
        {
            try
            {
                var notifications = await _notificationService.GetUnreadByUserIdAsync(userId);
                return Ok(new { message = "Lấy danh sách notifications chưa đọc thành công", data = notifications });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread notifications for user {UserId}", userId);
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Đếm số notifications chưa đọc của user
        [HttpGet("user/{userId}/unread/count")]
        public async Task<IActionResult> GetUnreadCount([FromRoute] int userId)
        {
            try
            {
                var count = await _notificationService.GetUnreadCountAsync(userId);
                return Ok(new NotificationUnreadCountResponse
                {
                    UserId = userId,
                    UnreadCount = count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count for user {UserId}", userId);
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Lấy notification theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var notification = await _notificationService.GetByIdAsync(id);
                return Ok(new { message = "Lấy notification thành công", data = notification });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification {Id}", id);
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Tạo notification mới (thường được gọi bởi các service khác)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var notification = await _notificationService.CreateAsync(request);
                
                // Send real-time notification via SignalR
                await _hubContext.Clients.Group($"user_{request.UserId}")
                    .SendAsync("ReceiveNotification", notification);

                return Ok(new { message = "Tạo notification thành công", data = notification });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification for user {UserId}", request.UserId);
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Đánh dấu notification là đã đọc
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead([FromRoute] int id)
        {
            try
            {
                var notification = await _notificationService.MarkAsReadAsync(id);
                
                // Notify client via SignalR
                await _hubContext.Clients.Group($"user_{notification.UserId}")
                    .SendAsync("NotificationRead", notification.Id);

                return Ok(new { message = "Đánh dấu đã đọc thành công", data = notification });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {Id} as read", id);
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Đánh dấu tất cả notifications của user là đã đọc
        [HttpPut("user/{userId}/read-all")]
        public async Task<IActionResult> MarkAllAsRead([FromRoute] int userId)
        {
            try
            {
                var result = await _notificationService.MarkAllAsReadAsync(userId);
                
                // Notify client via SignalR
                await _hubContext.Clients.Group($"user_{userId}")
                    .SendAsync("AllNotificationsRead", userId);

                return Ok(new { message = "Đánh dấu tất cả đã đọc thành công", success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read for user {UserId}", userId);
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Filter notifications theo các criteria
        [HttpPost("filter")]
        public async Task<IActionResult> Filter([FromBody] NotificationFilterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var notifications = await _notificationService.FilterAsync(request);
                return Ok(new { message = "Filter notifications thành công", data = notifications });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering notifications");
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Xóa notification
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                // Get notification before deleting to notify client
                var notification = await _notificationService.GetByIdAsync(id);
                var userId = notification.UserId;

                var result = await _notificationService.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = "Notification not found" });

                // Notify client via SignalR
                await _hubContext.Clients.Group($"user_{userId}")
                    .SendAsync("NotificationDeleted", id);

                return Ok(new { message = "Xóa notification thành công", success = true });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification {Id}", id);
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}

