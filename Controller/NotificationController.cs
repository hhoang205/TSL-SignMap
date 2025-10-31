using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;

namespace WebAppTrafficSign.Controller
{
    /// API controller quản lý các thông báo (Notification).  
    /// Cho phép xem, tạo, cập nhật trạng thái đọc và xoá thông báo.
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var notifications = _context.Notifications.ToList();
            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var notification = _context.Notifications.Find(id);
            if (notification == null)
                return NotFound();
            return Ok(notification);
        }

        [HttpPost]
        public IActionResult Create([FromBody] NotificationDto notificationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var notification = new Notification
            {
                UserId = notificationDto.UserId,
                Title = notificationDto.Title,
                Message = notificationDto.Message,
                IsRead = notificationDto.IsRead,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Notifications.Add(notification);
            _context.SaveChanges();
            return Ok(notification);
        }

        /// Cập nhật thông báo, ví dụ đánh dấu đã đọc.
        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] NotificationDto notificationDto)
        {
            var notification = _context.Notifications.Find(id);
            if (notification == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Cập nhật các trường cần thiết
            notification.Title = notificationDto.Title;
            notification.Message = notificationDto.Message;
            notification.UserId = notificationDto.UserId;
            notification.IsRead = notificationDto.IsRead;
            notification.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            return Ok(notification);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var notification = _context.Notifications.Find(id);
            if (notification == null)
                return NotFound();
            _context.Notifications.Remove(notification);
            _context.SaveChanges();
            return Ok(notification);
        }
    }
}