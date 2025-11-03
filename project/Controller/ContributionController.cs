using Microsoft.AspNetCore.Mvc;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;
namespace WebAppTrafficSign.Controller
{
    [Route("api/contributions")]
    [ApiController]
    public class ContributionController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public ContributionController(ApplicationDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var contributions = _context.Contributions.ToList();
            return Ok(contributions);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var contribution = _context.Contributions.Find(id);
            if (contribution == null)
                return NotFound();
            return Ok(contribution);
        }
        [HttpPost]
        public IActionResult CreateContribution([FromBody] Contribution contribution)
        {
            _context.Contributions.Add(contribution);
            _context.SaveChanges();
            return Ok(contribution);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteContribution(int id)
        {
            var contribution = _context.Contributions.Find(id);
            if (contribution == null)
                return NotFound();
            _context.Contributions.Remove(contribution);
            _context.SaveChanges();
            return Ok(contribution);
        }
    }
}
