using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FeedbackService.DTOs;
using FeedbackService.Services;

namespace FeedbackService.Controllers
{
    /// API controller quản lý Feedbacks theo requirement
    /// - Users can submit feedback or report issues with the app or database
    /// - Reporting inappropriate content or misuse is supported
    /// - Admins can review and manage feedback status (Pending, Reviewed, Resolved)
    [Route("api/feedbacks")]
    [ApiController]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        /// Lấy feedback theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var feedback = await _feedbackService.GetByIdAsync(id);
                return Ok(feedback);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Lấy tất cả feedbacks của một user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId([FromRoute] int userId)
        {
            try
            {
                var feedbacks = await _feedbackService.GetByUserIdAsync(userId);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Lấy tất cả feedbacks theo status
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus([FromRoute] string status)
        {
            try
            {
                var feedbacks = await _feedbackService.GetByStatusAsync(status);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Lấy tổng hợp feedbacks
        [HttpGet("summary")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSummary()
        {
            try
            {
                var summary = await _feedbackService.GetSummaryAsync();
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Tạo feedback mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FeedbackCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var feedback = await _feedbackService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = feedback.Id }, feedback);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Cập nhật feedback (content và status)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] FeedbackUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var feedback = await _feedbackService.UpdateAsync(id, request);
                return Ok(feedback);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Cập nhật status của feedback
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] FeedbackStatusUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var feedback = await _feedbackService.UpdateStatusAsync(id, request);
                return Ok(feedback);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Xóa feedback
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var result = await _feedbackService.DeleteAsync(id);
                if (result)
                    return Ok(new { message = "Feedback deleted successfully" });
                return NotFound(new { message = "Feedback not found" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Filter feedbacks với các điều kiện
        [HttpPost("filter")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Filter([FromBody] FeedbackFilterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var feedbacks = await _feedbackService.FilterAsync(request);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }
}

