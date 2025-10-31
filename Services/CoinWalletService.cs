using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;

namespace WebAppTrafficSign.Services
{

    /// Service quản lý ví xu (Coin Wallet) của người dùng
    public class CoinWalletService : Interfaces.ICoinWalletService
    {
        private readonly ApplicationDbContext _context;

        public CoinWalletService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// Tạo ví xu mới cho người dùng với số dư ban đầu
        public async Task CreateWalletAsync(int userId, decimal initialBalance)
        {
            // Kiểm tra ví đã tồn tại chưa
            var existingWallet = await _context.CoinWallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (existingWallet != null)
                throw new InvalidOperationException($"Ví xu cho user {userId} đã tồn tại.");

            var wallet = new CoinWallet
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Balance = initialBalance,
                CreatedAt = DateTime.UtcNow
            };

            await _context.CoinWallets.AddAsync(wallet);
            await _context.SaveChangesAsync();
        }

        /// Lấy số dư hiện tại của ví xu
        public async Task<decimal> GetBalanceAsync(int userId)
        {
            var wallet = await _context.CoinWallets.FirstOrDefaultAsync(w => w.UserId == userId);
            return wallet?.Balance ?? 0m;
        }

        /// Cộng thêm xu vào ví (credit)
        public async Task<bool> CreditAsync(int userId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Số xu phải lớn hơn 0");

            var wallet = await _context.CoinWallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null)
            {
                // Tự động tạo ví nếu chưa có - tạo trực tiếp trong context để tránh nested SaveChanges
                wallet = new CoinWallet
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Balance = amount,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.CoinWallets.AddAsync(wallet);
            }
            else
            {
                wallet.Balance += amount;
                wallet.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        /// Trừ xu từ ví (debit)
        public async Task<bool> DebitAsync(int userId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Số xu phải lớn hơn 0");

            var wallet = await _context.CoinWallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null)
                throw new InvalidOperationException($"Ví xu của user {userId} không tồn tại.");

            if (wallet.Balance < amount)
                throw new InvalidOperationException("Số dư không đủ.");

            wallet.Balance -= amount;
            wallet.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        /// Kiểm tra số dư có đủ để thực hiện giao dịch không
        public async Task<bool> HasEnoughBalanceAsync(int userId, decimal amount)
        {
            var balance = await GetBalanceAsync(userId);
            return balance >= amount;
        }
    }
}

