using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;

namespace WebAppTrafficSign.Controller
{
    /// API controller quản lý CRUD cho phiếu bầu (Vote).  
    /// Cho phép tạo, xem, cập nhật và xoá phiếu bầu cho góp ý.
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
        public IActionResult Create([FromBody] VoteDto voteDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var vote = new Vote
            {
                ContributionId = voteDto.ContributionId,
                UserId = voteDto.UserId,
                Value = voteDto.Value ? 1 : -1,
                IsUpvote = voteDto.Value,
                Weight = voteDto.Weight,
                CreatedAt = DateTime.Now
            };

            _context.Votes.Add(vote);
            _context.SaveChanges();
            return Ok(vote);
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] VoteDto voteDto)
        {
            var vote = _context.Votes.Find(id);
            if (vote == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Cập nhật các trường cần thiết
            vote.Value = voteDto.Value ? 1 : -1;
            vote.IsUpvote = voteDto.Value;
            vote.Weight = voteDto.Weight;
            vote.UserId = voteDto.UserId;
            vote.ContributionId = voteDto.ContributionId;

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