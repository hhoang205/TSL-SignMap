using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PaymentService.DTOs;
using PaymentService.Services;
using System.Security.Cryptography;
using System.Text;

namespace PaymentService.Controllers
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
        private readonly IConfiguration _configuration;

        public PaymentController(IPaymentService paymentService, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
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

        /// Tạo URL thanh toán VNPAY cho giao dịch nạp coin
        [HttpPost("vnpay/create")]
        public async Task<IActionResult> CreateVnPayPayment([FromBody] VnPayPaymentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.Amount <= 0)
                return BadRequest(new { message = "Số tiền phải lớn hơn 0" });

            try
            {
                // Tạo payment pending trong hệ thống
                var payment = await _paymentService.CreateAsync(new PaymentCreateRequest
                {
                    UserId = request.UserId,
                    Amount = request.Amount,
                    PaymentMethod = "VNPAY",
                    Status = "Pending",
                    PaymentDate = DateTime.UtcNow
                });

                var vnPaySection = _configuration.GetSection("VnPay");
                var tmnCode = vnPaySection["TmnCode"];
                var hashSecret = vnPaySection["HashSecret"];
                var baseUrl = vnPaySection["BaseUrl"];
                var returnUrl = vnPaySection["ReturnUrl"];

                if (string.IsNullOrWhiteSpace(tmnCode) ||
                    string.IsNullOrWhiteSpace(hashSecret) ||
                    string.IsNullOrWhiteSpace(baseUrl) ||
                    string.IsNullOrWhiteSpace(returnUrl))
                {
                    return StatusCode(500, new { message = "VNPAY configuration is missing" });
                }

                var vnpParams = new SortedDictionary<string, string>
                {
                    ["vnp_Version"] = "2.1.0",
                    ["vnp_Command"] = "pay",
                    ["vnp_TmnCode"] = tmnCode,
                    ["vnp_Amount"] = ((long)(request.Amount * 100)).ToString(),
                    ["vnp_CurrCode"] = "VND",
                    ["vnp_TxnRef"] = payment.Id.ToString(),
                    ["vnp_OrderInfo"] = $"Top-up for user {request.UserId}, payment #{payment.Id}",
                    ["vnp_OrderType"] = "topup",
                    ["vnp_ReturnUrl"] = returnUrl,
                    ["vnp_Locale"] = "vn",
                    ["vnp_CreateDate"] = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                    ["vnp_IpAddr"] = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"
                };

                if (!string.IsNullOrWhiteSpace(request.BankCode))
                {
                    vnpParams["vnp_BankCode"] = request.BankCode;
                }

                var query = new StringBuilder();
                var dataToSign = new StringBuilder();
                foreach (var kv in vnpParams)
                {
                    if (query.Length > 0)
                    {
                        query.Append('&');
                        dataToSign.Append('&');
                    }

                    query.Append(Uri.EscapeDataString(kv.Key));
                    query.Append('=');
                    query.Append(Uri.EscapeDataString(kv.Value));

                    dataToSign.Append(kv.Key);
                    dataToSign.Append('=');
                    dataToSign.Append(kv.Value);
                }

                var secureHash = HmacSHA512(hashSecret, dataToSign.ToString());
                query.Append("&vnp_SecureHash=");
                query.Append(secureHash);

                var paymentUrl = $"{baseUrl}?{query}";

                return Ok(new VnPayPaymentUrlResponse
                {
                    PaymentUrl = paymentUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        private static string HmacSHA512(string key, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);
            var sb = new StringBuilder(hashBytes.Length * 2);
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}

