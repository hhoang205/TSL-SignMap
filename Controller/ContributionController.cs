using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;

namespace WebAppTrafficSign.Controller
{
    /// API controller cung cấp CRUD cho các góp ý (Contribution).  
    /// Góp ý bao gồm hành động thêm/sửa/xoá biển báo do người dùng gửi lên.  
    [Route("api/contributions")]
    [ApiController]
    public class ContributionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContributionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            // Lấy toàn bộ góp ý. Sử dụng ToList() vì chúng ta không cần load navigation property ở đây.
            var contributions = _context.Contributions.ToList();
            return Ok(contributions);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var contribution = _context.Contributions.Find(id);
            if (contribution == null)
                return NotFound();
            return Ok(contribution);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ContributionDto contributionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var contribution = new Contribution
            {
                UserId = contributionDto.UserId,
                SignId = contributionDto.SignId,
                TrafficSignId = contributionDto.SignId,
                Action = contributionDto.Action,
                Description = contributionDto.Description,
                ImageUrl = contributionDto.ImageUrl,
                Status = contributionDto.Status,
                CreatedAt = DateTime.Now
            };

            _context.Contributions.Add(contribution);
            _context.SaveChanges();
            return Ok(contribution);
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] ContributionDto contributionDto)
        {
            var contribution = _context.Contributions.Find(id);
            if (contribution == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Cập nhật các trường cần thiết
            contribution.Action = contributionDto.Action;
            contribution.Description = contributionDto.Description;
            contribution.ImageUrl = contributionDto.ImageUrl;
            contribution.Status = contributionDto.Status;
            contribution.SignId = contributionDto.SignId;
            contribution.TrafficSignId = contributionDto.SignId;
            contribution.UserId = contributionDto.UserId;

            _context.SaveChanges();
            return Ok(contribution);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
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