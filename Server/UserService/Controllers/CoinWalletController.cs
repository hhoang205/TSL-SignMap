using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserService.DTOs;
using UserService.Services;

namespace UserService.Controllers
{
    /// API controller quản lý CoinWallet
    [Route("api/wallets")]
    [ApiController]
    [Authorize]
    public class CoinWalletController : ControllerBase
    {
        private readonly ICoinWalletService _coinWalletService;

        public CoinWalletController(ICoinWalletService coinWalletService)
        {
            _coinWalletService = coinWalletService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            try
            {
                var wallet = await _coinWalletService.GetByIdAsync(id);
                return Ok(wallet);
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

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId([FromRoute] int userId)
        {
            try
            {
                var wallet = await _coinWalletService.GetByUserIdAsync(userId);
                return Ok(wallet);
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

        [HttpGet("user/{userId}/balance")]
        public async Task<IActionResult> GetBalance([FromRoute] int userId)
        {
            try
            {
                var balance = await _coinWalletService.GetBalanceAsync(userId);
                return Ok(new { UserId = userId, Balance = balance });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            try
            {
                var summary = await _coinWalletService.GetSummaryAsync();
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CoinWalletCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var wallet = await _coinWalletService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = wallet.Id }, wallet);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CoinWalletUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var wallet = await _coinWalletService.UpdateAsync(id, request);
                return Ok(wallet);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpPost("user/{userId}/adjust")]
        public async Task<IActionResult> Adjust([FromRoute] int userId, [FromBody] CoinWalletAdjustRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var wallet = await _coinWalletService.AdjustAsync(userId, request);
                return Ok(wallet);
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

        [HttpPost("user/{userId}/credit")]
        [AllowAnonymous] // Allow service-to-service calls
        public async Task<IActionResult> Credit([FromRoute] int userId, [FromBody] CreditRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _coinWalletService.CreditAsync(userId, request.Amount);
                if (result)
                {
                    var wallet = await _coinWalletService.GetByUserIdAsync(userId);
                    return Ok(wallet);
                }
                return BadRequest(new { message = "Failed to credit" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpPost("user/{userId}/debit")]
        [AllowAnonymous] // Allow service-to-service calls
        public async Task<IActionResult> Debit([FromRoute] int userId, [FromBody] DebitRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _coinWalletService.DebitAsync(userId, request.Amount);
                if (result)
                {
                    var wallet = await _coinWalletService.GetByUserIdAsync(userId);
                    return Ok(wallet);
                }
                return BadRequest(new { message = "Failed to debit" });
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

        [HttpPost("user/{userId}/check-balance")]
        [AllowAnonymous] // Allow service-to-service calls
        public async Task<IActionResult> CheckBalance([FromRoute] int userId, [FromBody] CheckBalanceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var hasEnough = await _coinWalletService.HasEnoughBalanceAsync(userId, request.Amount);
                return Ok(new { UserId = userId, HasEnoughBalance = hasEnough });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var result = await _coinWalletService.DeleteAsync(id);
                if (result)
                    return Ok(new { message = "Wallet deleted successfully" });
                return NotFound(new { message = "Wallet not found" });
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

        [HttpPost("filter")]
        public async Task<IActionResult> Filter([FromBody] CoinWalletFilterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var wallets = await _coinWalletService.FilterAsync(request);
                return Ok(wallets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Lấy lịch sử giao dịch của user (aggregate từ wallet transactions và payments)
        [HttpGet("user/{userId}/transactions")]
        public async Task<IActionResult> GetTransactions(
            [FromRoute] int userId,
            [FromQuery] int? page = null,
            [FromQuery] int? pageSize = null,
            [FromQuery] string? type = null,
            [FromQuery] string? status = null)
        {
            try
            {
                var transactions = await _coinWalletService.GetTransactionsAsync(userId, page, pageSize, type, status);
                
                // Convert to list and return in format expected by mobile app
                var transactionList = transactions.ToList();
                return Ok(new { transactions = transactionList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }

    // Helper DTOs
    public class CreditRequest
    {
        public decimal Amount { get; set; }
    }

    public class DebitRequest
    {
        public decimal Amount { get; set; }
    }

    public class CheckBalanceRequest
    {
        public decimal Amount { get; set; }
    }
}

