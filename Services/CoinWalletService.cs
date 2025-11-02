using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;
using WebAppTrafficSign.Mapper;
using WebAppTrafficSign.Services.Interfaces;

namespace WebAppTrafficSign.Services
{
    /// Service quản lý CoinWallet theo requirement
    /// - Users receive 20 initial TSL Coins when registering
    /// - Users manage profiles (personal info, settings, coin balance)
    /// - Admins manage user accounts, permissions, and coin adjustments via the web panel
    public class CoinWalletService : ICoinWalletService
    {
        private readonly ApplicationDbContext _context;

        public CoinWalletService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// Lấy wallet theo ID
        public async Task<CoinWalletDto> GetByIdAsync(Guid id)
        {
            var wallet = await _context.CoinWallets
                .Include(w => w.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id);

            if (wallet == null)
                throw new InvalidOperationException("Wallet not found");

            return wallet.toDto();
        }

        /// Lấy wallet theo UserId
        public async Task<CoinWalletDto> GetByUserIdAsync(int userId)
        {
            var wallet = await _context.CoinWallets
                .Include(w => w.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
                throw new InvalidOperationException($"Wallet for user {userId} not found");

            return wallet.toDto();
        }

        /// Lấy số dư hiện tại của ví xu
        public async Task<decimal> GetBalanceAsync(int userId)
        {
            var wallet = await _context.CoinWallets
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.UserId == userId);
            return wallet?.Balance ?? 0m;
        }

        /// Tạo ví xu mới cho người dùng với số dư ban đầu
        public async Task<CoinWalletDto> CreateAsync(CoinWalletCreateRequest request)
        {
            // Kiểm tra user tồn tại
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Kiểm tra ví đã tồn tại chưa
            var existingWallet = await _context.CoinWallets.FirstOrDefaultAsync(w => w.UserId == request.UserId);
            if (existingWallet != null)
                throw new InvalidOperationException($"Wallet for user {request.UserId} already exists");

            var wallet = new CoinWallet
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Balance = request.InitialBalance,
                CreatedAt = DateTime.UtcNow
            };

            await _context.CoinWallets.AddAsync(wallet);
            await _context.SaveChangesAsync();

            // Load User để map Username
            await _context.Entry(wallet).Reference(w => w.User).LoadAsync();

            return wallet.toDto();
        }

        /// Cập nhật wallet (chủ yếu cho admin)
        public async Task<CoinWalletDto> UpdateAsync(Guid id, CoinWalletUpdateRequest request)
        {
            var wallet = await _context.CoinWallets.FindAsync(id);
            if (wallet == null)
                throw new InvalidOperationException("Wallet not found");

            // Cập nhật balance nếu có
            if (request.Balance.HasValue)
            {
                if (request.Balance.Value < 0)
                    throw new ArgumentException("Balance cannot be negative");
                wallet.Balance = request.Balance.Value;
                wallet.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            // Load User để map Username
            await _context.Entry(wallet).Reference(w => w.User).LoadAsync();

            return wallet.toDto();
        }

        /// Xóa wallet
        public async Task<bool> DeleteAsync(Guid id)
        {
            var wallet = await _context.CoinWallets.FindAsync(id);
            if (wallet == null)
                throw new InvalidOperationException("Wallet not found");

            _context.CoinWallets.Remove(wallet);
            await _context.SaveChangesAsync();
            return true;
        }

        /// Filter wallets với các điều kiện
        public async Task<IEnumerable<CoinWalletDto>> FilterAsync(CoinWalletFilterRequest request)
        {
            var query = _context.CoinWallets
                .Include(w => w.User)
                .AsQueryable();

            // Filter theo UserId
            if (request.UserId.HasValue)
            {
                query = query.Where(w => w.UserId == request.UserId.Value);
            }

            // Filter theo balance range
            if (request.MinBalance.HasValue)
            {
                query = query.Where(w => w.Balance >= request.MinBalance.Value);
            }

            if (request.MaxBalance.HasValue)
            {
                query = query.Where(w => w.Balance <= request.MaxBalance.Value);
            }

            // Filter theo date range
            if (request.StartDate.HasValue)
            {
                query = query.Where(w => w.CreatedAt >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(w => w.CreatedAt <= request.EndDate.Value);
            }

            // Sắp xếp theo thời gian mới nhất
            query = query.OrderByDescending(w => w.CreatedAt);

            // Pagination
            var skip = (request.PageNumber - 1) * request.PageSize;
            query = query.Skip(skip).Take(request.PageSize);

            var wallets = await query.AsNoTracking().ToListAsync();
            return wallets.Select(w => w.toDto());
        }

        /// Lấy tổng hợp wallets
        public async Task<CoinWalletSummaryResponse> GetSummaryAsync()
        {
            var wallets = await _context.CoinWallets
                .AsNoTracking()
                .ToListAsync();

            if (wallets.Count == 0)
            {
                return new CoinWalletSummaryResponse
                {
                    TotalWallets = 0,
                    TotalBalance = 0,
                    AverageBalance = 0,
                    MinBalance = 0,
                    MaxBalance = 0,
                    WalletsWithZeroBalance = 0,
                    WalletsWithLowBalance = 0,
                    WalletsWithHighBalance = 0
                };
            }

            var summary = new CoinWalletSummaryResponse
            {
                TotalWallets = wallets.Count,
                TotalBalance = wallets.Sum(w => w.Balance),
                AverageBalance = wallets.Average(w => w.Balance),
                MinBalance = wallets.Min(w => w.Balance),
                MaxBalance = wallets.Max(w => w.Balance),
                WalletsWithZeroBalance = wallets.Count(w => w.Balance == 0),
                WalletsWithLowBalance = wallets.Count(w => w.Balance < 10),
                WalletsWithHighBalance = wallets.Count(w => w.Balance >= 100)
            };

            return summary;
        }

        /// Cộng thêm xu vào ví (credit)
        public async Task<bool> CreditAsync(int userId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Số xu phải lớn hơn 0");

            var wallet = await _context.CoinWallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null)
            {
                // Tự động tạo ví nếu chưa có
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
                throw new InvalidOperationException($"Wallet for user {userId} not found");

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

        /// Admin điều chỉnh coin (reward/penalty)
        public async Task<CoinWalletDto> AdjustAsync(int userId, CoinWalletAdjustRequest request)
        {
            if (request.Amount <= 0)
                throw new ArgumentException("Amount must be greater than 0");

            var wallet = await _context.CoinWallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
            {
                // Tự động tạo ví nếu chưa có
                wallet = new CoinWallet
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Balance = request.AdjustmentType == "Credit" ? request.Amount : 0m,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.CoinWallets.AddAsync(wallet);
            }
            else
            {
                if (request.AdjustmentType == "Credit")
                {
                    wallet.Balance += request.Amount;
                }
                else if (request.AdjustmentType == "Debit")
                {
                    if (wallet.Balance < request.Amount)
                        throw new InvalidOperationException("Insufficient balance for debit");
                    wallet.Balance -= request.Amount;
                }
                else
                {
                    throw new ArgumentException("AdjustmentType must be 'Credit' or 'Debit'");
                }
                wallet.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            // Load User để map Username
            await _context.Entry(wallet).Reference(w => w.User).LoadAsync();

            return wallet.toDto();
        }
    }
}

