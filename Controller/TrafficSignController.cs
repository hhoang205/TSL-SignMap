using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;

namespace WebAppTrafficSign.Controller
{
    /// API controller quản lý CRUD cho bảng TrafficSign.  
    /// Cho phép thêm mới biển báo, lấy danh sách, sửa đổi và xoá biển báo.
    [Route("api/signs")]
    [ApiController]
    public class TrafficSignController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TrafficSignController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var signs = _context.TrafficSigns.ToList();
            return Ok(signs);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var sign = _context.TrafficSigns.Find(id);
            if (sign == null)
                return NotFound();
            return Ok(sign);
        }

        [HttpPost]
        public IActionResult Create([FromBody] TrafficSign sign)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.TrafficSigns.Add(sign);
            _context.SaveChanges();
            return Ok(sign);
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] TrafficSign updated)
        {
            var sign = _context.TrafficSigns.Find(id);
            if (sign == null)
                return NotFound();

            // Cập nhật các trường cần thiết
            sign.Type      = updated.Type;
            sign.Location  = updated.Location;
            sign.Status    = updated.Status;
            sign.ValidFrom = updated.ValidFrom;
            // Nếu có ValidTo, update; sắp xếp field tuỳ model

            _context.SaveChanges();
            return Ok(sign);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var sign = _context.TrafficSigns.Find(id);
            if (sign == null)
                return NotFound();
            _context.TrafficSigns.Remove(sign);
            _context.SaveChanges();
            return Ok(sign);
        }
    }
}