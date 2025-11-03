using Microsoft.AspNetCore.Mvc;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;
namespace WebAppTrafficSign.Controller
{
    [Route("api/votes")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        readonly ApplicationDBContext _context;
        public VoteController(ApplicationDBContext context)
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
        public IActionResult GetById(int id)
        {
            var vote = _context.Votes.Find(id);
            if (vote == null)
                return NotFound();
            return Ok(vote);
        }
        [HttpPost]
        public IActionResult CreateVote([FromBody] Vote vote)
        {
            _context.Votes.Add(vote);
            _context.SaveChanges();
            return Ok(vote);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteVote(int id) {
            var vote = _context.Votes.Find(id);
            if (vote == null)
                return NotFound();
            _context.Votes.Remove(vote);
            _context.SaveChanges();
            return Ok(vote);
        }

    }
}
