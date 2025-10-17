using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.DTOs;
using WebAppTrafficSign.Models;
using WebAppTrafficSign.Mapper;

namespace WebAppTrafficSign.Services
{
    /// Định nghĩa các chức năng của PaymentService.
    /// Cung cấp các phương thức để tạo, cập nhật, đọc và xóa giao dịch nạp tiền,
    /// đồng thời cập nhật số dư trong ví xu của người dùng khi thanh toán hoàn tất.
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetAllAsync();
        Task<PaymentDto> GetByIdAsync(int id);
        Task<PaymentDto> CreateAsync(PaymentCreateRequest request);
        Task<PaymentDto> UpdateAsync(int id, PaymentUpdateRequest request);
        Task<bool> DeleteAsync(int id);
    }

    /// Triển khai PaymentService sử dụng Entity Framework Core để thao tác dữ liệu.
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;

        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// Lấy danh sách tất cả giao dịch thanh toán.
        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            // Sử dụng Set<Payment>() thay vì _context.Payments để tránh lỗi khi DbContext chưa khai báo DbSet<Payment>
            var payments = await _context.Set<Payment>()
                .AsNoTracking()
                .Select(p => p.toDto())
                .ToListAsync();
            return payments;
        }

        /// Lấy thông tin giao dịch theo Id.
        public async Task<PaymentDto> GetByIdAsync(int id)
        {
            // Lấy payment qua Set<Payment>() để hỗ trợ các DbContext chưa định nghĩa DbSet<Payment>
            var payment = await _context.Set<Payment>().AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            return payment?.toDto();
        }

        /// Tạo giao dịch thanh toán mới. Khi thanh toán thành công, số xu được cộng vào ví của người dùng.
        public async Task<PaymentDto> CreateAsync(PaymentCreateRequest request)
        {
            if (request.Amount <= 0m) throw new ArgumentException("Số tiền phải lớn hơn 0");

            // Kiểm tra người dùng tồn tại
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null) throw new InvalidOperationException("User not found");

            var payment = new Payment
            {
                UserId = request.UserId,
                Amount = request.Amount,
                PaymentDate = request.PaymentDate ?? DateTime.UtcNow,
                PaymentMethod = request.PaymentMethod,
                Status = string.IsNullOrWhiteSpace(request.Status) ? "Completed" : request.Status
            };

            await _context.Set<Payment>().AddAsync(payment);

            // Cập nhật ví xu của người dùng nếu trạng thái là Completed
            if (payment.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            {
                await CreditWalletAsync(payment.UserId, payment.Amount);
            }

            await _context.SaveChangesAsync();
            return payment.toDto();
        }

        /// Cập nhật giao dịch thanh toán. Nếu thay đổi số tiền hoặc trạng thái từ Pending sang Completed, 
        /// số xu trong ví sẽ được điều chỉnh tương ứng.
        public async Task<PaymentDto> UpdateAsync(int id, PaymentUpdateRequest request)
        {
            var payment = await _context.Set<Payment>().FindAsync(id);
            if (payment == null) throw new InvalidOperationException("Payment not found");

            // Lưu số tiền và trạng thái cũ để so sánh
            var oldAmount = payment.Amount;
            var oldStatus = payment.Status;

            // Cập nhật các trường
            payment.PaymentMethod = request.PaymentMethod ?? payment.PaymentMethod;
            payment.Status = request.Status ?? payment.Status;
            payment.PaymentDate = request.PaymentDate ?? payment.PaymentDate;

            // Nếu số tiền được cập nhật và trạng thái Completed -> điều chỉnh ví
            if (request.Amount.HasValue && request.Amount.Value > 0m)
            {
                payment.Amount = request.Amount.Value;
            }

            // Lưu thay đổi và cập nhật ví
            await _context.SaveChangesAsync();

            // Nếu trạng thái mới là Completed nhưng trước đó không phải Completed, cộng xu
            if (!oldStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) &&
                payment.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            {
                await CreditWalletAsync(payment.UserId, payment.Amount);
            }
            // Nếu thay đổi số tiền trong trạng thái Completed, điều chỉnh chênh lệch
            else if (oldStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) &&
                     payment.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase) &&
                     payment.Amount != oldAmount)
            {
                var difference = payment.Amount - oldAmount;
                await CreditWalletAsync(payment.UserId, difference);
            }

            return payment.toDto();
        }

        /// Xóa giao dịch thanh toán. Lưu ý: không hoàn xu khi xóa, bạn có thể điều chỉnh theo nghiệp vụ.
        public async Task<bool> DeleteAsync(int id)
        {
            var payment = await _context.Set<Payment>().FindAsync(id);
            if (payment == null) return false;

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return true;
        }

        /// Cộng thêm số xu cho ví của người dùng. Nếu ví chưa tồn tại, tạo ví mới.
        private async Task CreditWalletAsync(int userId, decimal amount)
        {
            if (amount == 0m) return;
            var wallet = await _context.CoinWallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null)
            {
                wallet = new CoinWallet
                {
                    UserId = userId,
                    Balance = amount,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.CoinWallets.AddAsync(wallet);
            }
            else
            {
                wallet.Balance += amount;
            }
        }
    }
}