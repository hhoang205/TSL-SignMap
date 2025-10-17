using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;

namespace WebAppTrafficSign.Controller
{
    /// <summary>
    /// API controller quản lý các thông báo (Notification).  
    /// Cho phép xem, tạo, cập nhật trạng thái đọc và xoá thông báo.
    /// </summary>
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
        public IActionResult Create([FromBody] Notification notification)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _context.Notifications.Add(notification);
            _context.SaveChanges();
            return Ok(notification);
        }

        /// <summary>
        /// Cập nhật thông báo, ví dụ đánh dấu đã đọc.
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] Notification updated)
        {
            var notification = _context.Notifications.Find(id);
            if (notification == null)
                return NotFound();
            // Cập nhật các trường cần thiết
            notification.Title   = updated.Title;
            notification.Message = updated.Message;
            notification.UserId  = updated.UserId;
            // Nếu model có thuộc tính Read, cập nhật: notification.Read = updated.Read;
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