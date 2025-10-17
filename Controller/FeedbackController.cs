using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;

namespace WebAppTrafficSign.Controller
{
    /// API controller quản lý các phản hồi/feedback từ người dùng.  
    /// Cho phép thêm mới, xem, cập nhật trạng thái và xoá feedback.
    [Route("api/feedbacks")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var feedbacks = _context.Feedbacks.ToList();
            return Ok(feedbacks);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var feedback = _context.Feedbacks.Find(id);
            if (feedback == null)
                return NotFound();
            return Ok(feedback);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Feedback feedback)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _context.Feedbacks.Add(feedback);
            _context.SaveChanges();
            return Ok(feedback);
        }

        /// <summary>
        /// Cập nhật trạng thái hoặc nội dung phản hồi.  
        /// Trạng thái có thể là New, InProgress, Resolved.
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] Feedback updated)
        {
            var feedback = _context.Feedbacks.Find(id);
            if (feedback == null)
                return NotFound();
            feedback.Content    = updated.Content;
            feedback.Status     = updated.Status;
            feedback.UserId     = updated.UserId;
            // cập nhật thời gian giải quyết nếu cần
            feedback.ResolvedAt = updated.ResolvedAt;
            _context.SaveChanges();
            return Ok(feedback);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var feedback = _context.Feedbacks.Find(id);
            if (feedback == null)
                return NotFound();
            _context.Feedbacks.Remove(feedback);
            _context.SaveChanges();
            return Ok(feedback);
        }
    }
}