using Microsoft.AspNetCore.Mvc;
using TrafficSignService.DTOs;
using TrafficSignService.Services;

namespace TrafficSignService.Controllers
{
    /// API controller quản lý Traffic Signs theo requirement
    /// - Hiển thị real-time traffic sign locations (không tốn coin)
    /// - Search và filter (tốn 1 coin cho advanced filters)
    [Route("api/signs")]
    [ApiController]
    public class TrafficSignController : ControllerBase
    {
        private readonly ITrafficSignService _trafficSignService;

        public TrafficSignController(ITrafficSignService trafficSignService)
        {
            _trafficSignService = trafficSignService;
        }

        /// Lấy tất cả traffic signs active (hiển thị trên map) - KHÔNG TỐN COIN
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var signs = await _trafficSignService.GetAllAsync();
                return Ok(signs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Lấy traffic sign theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var sign = await _trafficSignService.GetByIdAsync(id);
                return Ok(sign);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Tìm kiếm traffic signs với các filter options
        /// - Nếu dùng Type filter hoặc Proximity filter -> TỐN 1 COIN
        /// - Nếu không có filter nào -> KHÔNG TỐN COIN (chỉ lấy tất cả active signs)
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] TrafficSignSearchRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var signs = await _trafficSignService.SearchAsync(request);
                return Ok(new { 
                    message = "Tìm kiếm thành công", 
                    count = signs.Count(),
                    data = signs 
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Filter traffic signs theo proximity (bán kính) - TỐN 1 COIN
        [HttpPost("filter/proximity")]
        public async Task<IActionResult> FilterByProximity([FromBody] TrafficSignProximityFilterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.RadiusKm <= 0)
                return BadRequest(new { message = "Bán kính phải lớn hơn 0" });

            try
            {
                var signs = await _trafficSignService.FilterByProximityAsync(request);
                return Ok(new { 
                    message = $"Đã tìm thấy {signs.Count()} biển báo trong bán kính {request.RadiusKm}km", 
                    count = signs.Count(),
                    data = signs 
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Filter traffic signs theo type - TỐN 1 COIN
        [HttpGet("filter/type/{type}")]
        public async Task<IActionResult> FilterByType([FromRoute] string type, [FromQuery] int? userId = null)
        {
            if (string.IsNullOrWhiteSpace(type))
                return BadRequest(new { message = "Type không được để trống" });

            try
            {
                var signs = await _trafficSignService.FilterByTypeAsync(type);
                return Ok(new { 
                    message = $"Đã tìm thấy {signs.Count()} biển báo loại {type}", 
                    count = signs.Count(),
                    data = signs 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Tạo traffic sign mới (Admin only hoặc từ approved contribution)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TrafficSignCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var sign = await _trafficSignService.CreateAsync(request);
                return Ok(new { message = "Tạo traffic sign thành công", data = sign });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Cập nhật traffic sign
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TrafficSignUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var sign = await _trafficSignService.UpdateAsync(id, request);
                return Ok(new { message = "Cập nhật traffic sign thành công", data = sign });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Xóa traffic sign
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var result = await _trafficSignService.DeleteAsync(id);
                if (result)
                    return Ok(new { message = "Xóa traffic sign thành công" });
                return NotFound(new { message = "Traffic sign not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}

