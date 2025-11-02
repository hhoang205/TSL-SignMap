using Microsoft.AspNetCore.Mvc;
using WebAppTrafficSign.DTOs;
using WebAppTrafficSign.Services.Interfaces;

namespace WebAppTrafficSign.Controller
{
    /// API controller quản lý Votes theo requirement
    /// - Users can vote on contributions (upvote/downvote)
    /// - Votes có weight và mỗi user chỉ có thể vote 1 lần cho mỗi contribution
    [Route("api/votes")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        private readonly IVoteService _voteService;

        public VoteController(IVoteService voteService)
        {
            _voteService = voteService;
        }

        /// Lấy vote theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var vote = await _voteService.GetByIdAsync(id);
                return Ok(vote);
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

        /// Lấy tất cả votes của một contribution
        [HttpGet("contribution/{contributionId}")]
        public async Task<IActionResult> GetByContributionId([FromRoute] int contributionId)
        {
            try
            {
                var votes = await _voteService.GetByContributionIdAsync(contributionId);
                return Ok(votes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Lấy tất cả votes của một user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId([FromRoute] int userId)
        {
            try
            {
                var votes = await _voteService.GetByUserIdAsync(userId);
                return Ok(votes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Lấy tổng hợp votes của một contribution
        [HttpGet("contribution/{contributionId}/summary")]
        public async Task<IActionResult> GetVoteSummary([FromRoute] int contributionId)
        {
            try
            {
                var summary = await _voteService.GetVoteSummaryAsync(contributionId);
                return Ok(summary);
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

        /// Tạo vote mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VoteCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var vote = await _voteService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = vote.Id }, vote);
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

        /// Cập nhật vote
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] VoteUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var vote = await _voteService.UpdateAsync(id, request);
                return Ok(vote);
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

        /// Xóa vote
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var result = await _voteService.DeleteAsync(id);
                if (result)
                    return Ok(new { message = "Vote deleted successfully" });
                return NotFound(new { message = "Vote not found" });
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

        /// Filter votes với các điều kiện
        [HttpPost("filter")]
        public async Task<IActionResult> Filter([FromBody] VoteFilterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var votes = await _voteService.FilterAsync(request);
                return Ok(votes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }
}