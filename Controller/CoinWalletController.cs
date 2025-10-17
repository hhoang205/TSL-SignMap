using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;

namespace WebAppTrafficSign.Controller
{
    /// API controller cung cấp các thao tác CRUD cho ví xu (CoinWallet).
    /// Người dùng có thể xem danh sách ví, xem chi tiết, tạo mới, cập nhật hoặc xoá ví.  
    /// Tất cả thao tác đều sử dụng ApplicationDbContext để thao tác với cơ sở dữ liệu.
    [Route("api/wallets")]
    [ApiController]
    public class CoinWalletController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CoinWalletController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// Trả về danh sách tất cả ví xu trong hệ thống.
        [HttpGet]
        public IActionResult GetAll()
        {
            var wallets = _context.CoinWallets.ToList();
            return Ok(wallets);
        }

        /// Lấy thông tin ví theo id.
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var wallet = _context.CoinWallets.Find(id);
            if (wallet == null)
                return NotFound();
            return Ok(wallet);
        }

        /// Tạo ví xu mới.  
        /// Việc tạo ví thủ công chủ yếu dành cho mục đích quản trị hoặc test; trong thực tế ví sẽ được
        /// tạo tự động khi người dùng đăng ký.
        [HttpPost]
        public IActionResult Create([FromBody] CoinWallet wallet)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.CoinWallets.Add(wallet);
            _context.SaveChanges();
            return Ok(wallet);
        }

        /// Cập nhật thông tin ví xu.  
        /// Cho phép sửa số dư hoặc các thuộc tính khác (nếu có).
        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] CoinWallet updated)
        {
            var wallet = _context.CoinWallets.Find(id);
            if (wallet == null)
                return NotFound();

            // Cập nhật các trường cần thiết.  
            wallet.Balance = updated.Balance;
            wallet.UserId  = updated.UserId;
            // Có thể cập nhật CreatedAt/UpdatedAt tuỳ nhu cầu

            _context.SaveChanges();
            return Ok(wallet);
        }

        /// Xoá ví xu khỏi hệ thống.  
        /// Cân nhắc trước khi xoá vì quan hệ 1:1 với người dùng.
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var wallet = _context.CoinWallets.Find(id);
            if (wallet == null)
                return NotFound();

            _context.CoinWallets.Remove(wallet);
            _context.SaveChanges();
            return Ok(wallet);
        }
    }
}