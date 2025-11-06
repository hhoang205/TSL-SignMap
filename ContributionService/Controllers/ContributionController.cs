using Microsoft.AspNetCore.Mvc;
using ContributionService.DTOs;
using ContributionService.Services;

namespace ContributionService.Controllers
{
    /// API controller quản lý Contributions theo requirement
    /// - Users submit contributions (tốn 5 coins)
    /// - Admin approve/reject contributions
    /// - Approved contributions -> convert to TrafficSign + reward coins (10+)
    [Route("api/contributions")]
    [ApiController]
    public class ContributionController : ControllerBase
    {
        private readonly IContributionService _contributionService;

        public ContributionController(IContributionService contributionService)
        {
            _contributionService = contributionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var contributions = await _contributionService.GetAllAsync();
                return Ok(new { message = "Lấy danh sách contributions thành công", count = contributions.Count(), data = contributions });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var contribution = await _contributionService.GetByIdAsync(id);
                return Ok(new { message = "Lấy contribution thành công", data = contribution });
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

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus([FromRoute] string status)
        {
            try
            {
                var contributions = await _contributionService.GetByStatusAsync(status);
                return Ok(new { message = $"Lấy contributions với status '{status}' thành công", count = contributions.Count(), data = contributions });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId([FromRoute] int userId)
        {
            try
            {
                var contributions = await _contributionService.GetByUserIdAsync(userId);
                return Ok(new { message = $"Lấy contributions của user {userId} thành công", count = contributions.Count(), data = contributions });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost("filter")]
        public async Task<IActionResult> Filter([FromBody] ContributionFilterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var contributions = await _contributionService.FilterAsync(request);
                return Ok(new { message = "Filter contributions thành công", count = contributions.Count(), data = contributions });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] ContributionCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var contribution = await _contributionService.SubmitAsync(request);
                return Ok(new { message = $"Đã submit contribution thành công. Đã trừ {5} coin.", data = contribution });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
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

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve([FromRoute] int id, [FromBody] ContributionReviewRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var contribution = await _contributionService.ApproveAsync(id, request);
                decimal rewardAmount = request.RewardAmount ?? 10m;
                return Ok(new { message = $"Đã approve contribution #{id}. User nhận {rewardAmount} coin reward.", data = contribution });
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

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject([FromRoute] int id, [FromBody] ContributionReviewRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var contribution = await _contributionService.RejectAsync(id, request);
                return Ok(new { message = $"Đã reject contribution #{id}. User đã được thông báo.", data = contribution });
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ContributionCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var contribution = await _contributionService.UpdateAsync(id, request);
                return Ok(new { message = $"Đã cập nhật contribution #{id} thành công", data = contribution });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var result = await _contributionService.DeleteAsync(id);
                if (result)
                    return Ok(new { message = $"Đã xóa contribution #{id} thành công" });
                return NotFound(new { message = "Contribution not found" });
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
    }
}

