using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;
using System.Security.Claims;

namespace WebAppTrafficSign.Controller
{
   
    [Route("api/feedbacks")]
    [ApiController]
    [Authorize]
    public class FeedBackController: ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public FeedBackController(ApplicationDBContext context)
        {
            _context = context;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var feedbacks = _context.FeedBacks.ToList();
            return Ok(feedbacks);
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var feedback = _context.FeedBacks.Find(id);
            if (feedback == null)
                return NotFound();
            return Ok(feedback);
        }


        [HttpPost]
        public IActionResult CreateFeedback([FromBody] Feedback feedback)
        {
            var userId = User.FindFirstValue("id");
            var username=User.Identity?.Name;

            feedback.UserId = int.Parse(userId);
            feedback.CreatedAt = DateTime.UtcNow;

            _context.FeedBacks.Add(feedback);
            _context.SaveChanges();
            return Ok(feedback);
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteFeedback(int id)
        {
            var feedback = _context.FeedBacks.Find(id);
            if (feedback == null)
                return NotFound();
            _context.FeedBacks.Remove(feedback);
            _context.SaveChanges();
            return Ok(feedback);
        }

    }
}
