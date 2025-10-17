using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;

namespace WebAppTrafficSign.Controller
{
    /// <summary>
    /// API controller quản lý CRUD cho phiếu bầu (Vote).  
    /// Cho phép tạo, xem, cập nhật và xoá phiếu bầu cho góp ý.
    /// </summary>
    [Route("api/votes")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VoteController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var votes = _context.Votes.ToList();
            return Ok(votes);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var vote = _context.Votes.Find(id);
            if (vote == null)
                return NotFound();
            return Ok(vote);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Vote vote)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _context.Votes.Add(vote);
            _context.SaveChanges();
            return Ok(vote);
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] Vote updated)
        {
            var vote = _context.Votes.Find(id);
            if (vote == null)
                return NotFound();
            // Cập nhật các trường cần thiết
            vote.Value        = updated.Value;
            vote.Weight       = updated.Weight;
            vote.UserId       = updated.UserId;
            vote.ContributionId = updated.ContributionId;
            _context.SaveChanges();
            return Ok(vote);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var vote = _context.Votes.Find(id);
            if (vote == null)
                return NotFound();
            _context.Votes.Remove(vote);
            _context.SaveChanges();
            return Ok(vote);
        }
    }
}