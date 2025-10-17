using Microsoft.AspNetCore.Mvc;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;
using WebAppTrafficSign.Services;
using WebAppTrafficSign.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace WebAppTrafficSign.Controller
{

    /// API controller quản lý giao dịch nạp tiền/xu (Payment).
    /// Mỗi bản ghi Payment đại diện cho một lần thanh toán nạp xu của người dùng.
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

        /// Lấy danh sách tất cả các giao dịch thanh toán.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _paymentService.GetAllAsync();
            return Ok(payments);
        }

        /// Lấy giao dịch theo ID.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null) return NotFound();
            return Ok(payment);
        }


        /// Tạo giao dịch thanh toán mới.  
        /// Khi tạo, nên gán PaymentDate = DateTime.UtcNow và Status = "Pending" hoặc "Completed" tuỳ logic thanh toán.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentCreateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var payment = await _paymentService.CreateAsync(request);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// Cập nhật giao dịch thanh toán.  
        /// Cho phép thay đổi Amount, PaymentMethod, Status và PaymentDate (nếu cần).
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PaymentUpdateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var payment = await _paymentService.UpdateAsync(id, request);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// Xoá giao dịch thanh toán.  
        /// Cân nhắc cẩn thận trước khi xoá vì có thể ảnh hưởng tới lịch sử nạp xu.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _paymentService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}