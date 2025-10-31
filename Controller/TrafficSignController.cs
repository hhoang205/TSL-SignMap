using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;
using NetTopologySuite.Geometries;

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
        public IActionResult Create([FromBody] TrafficSignDto signDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sign = new TrafficSign
            {
                Type = signDto.Type,
                Location = new Point(signDto.Longitude, signDto.Latitude) { SRID = 4326 },
                Status = signDto.Status,
                ImageUrl = signDto.ImageUrl,
                ValidFrom = signDto.ValidFrom,
                ValidTo = signDto.ValidTo ?? DateTime.MaxValue
            };

            _context.TrafficSigns.Add(sign);
            _context.SaveChanges();
            return Ok(sign);
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] TrafficSignDto signDto)
        {
            var sign = _context.TrafficSigns.Find(id);
            if (sign == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Cập nhật các trường cần thiết
            sign.Type = signDto.Type;
            sign.Location = new Point(signDto.Longitude, signDto.Latitude) { SRID = 4326 };
            sign.Status = signDto.Status;
            sign.ValidFrom = signDto.ValidFrom;
            sign.ValidTo = signDto.ValidTo ?? DateTime.MaxValue;
            sign.ImageUrl = signDto.ImageUrl;

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