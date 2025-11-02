using Microsoft.AspNetCore.Mvc;
using WebAppTrafficSign.DTOs;
using WebAppTrafficSign.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace WebAppTrafficSign.Controller
{
    /// API controller quản lý Payments theo requirement
    /// - Users can top up coins with money (e.g., $1 for 10 Coins) via in-app payment
    /// - Admins can review and manage payments
    /// - Payment status: Pending, Completed, Failed
    [Route("api/payments")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// Lấy payment theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var payment = await _paymentService.GetByIdAsync(id);
                return Ok(payment);
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

        /// Lấy tất cả payments của một user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId([FromRoute] int userId)
        {
            try
            {
                var payments = await _paymentService.GetByUserIdAsync(userId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Lấy tất cả payments theo status
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus([FromRoute] string status)
        {
            try
            {
                var payments = await _paymentService.GetByStatusAsync(status);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Lấy tổng hợp payments
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            try
            {
                var summary = await _paymentService.GetSummaryAsync();
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Tạo payment mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var payment = await _paymentService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
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
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Cập nhật payment (amount, payment method, status, payment date)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PaymentUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var payment = await _paymentService.UpdateAsync(id, request);
                return Ok(payment);
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

        /// Cập nhật status của payment
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] PaymentStatusUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var payment = await _paymentService.UpdateStatusAsync(id, request);
                return Ok(payment);
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

        /// Xóa payment
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var result = await _paymentService.DeleteAsync(id);
                if (result)
                    return Ok(new { message = "Payment deleted successfully" });
                return NotFound(new { message = "Payment not found" });
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

        /// Filter payments với các điều kiện
        [HttpPost("filter")]
        public async Task<IActionResult> Filter([FromBody] PaymentFilterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var payments = await _paymentService.FilterAsync(request);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }
}