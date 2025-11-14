using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;
using UserService.DTOs;
using UserService.Mapper;

namespace UserService.Services
{
    /// Service quản lý CoinWallet theo requirement
    public interface ICoinWalletService
    {
        Task<CoinWalletDto> GetByIdAsync(Guid id);
        Task<CoinWalletDto> GetByUserIdAsync(int userId);
        Task<decimal> GetBalanceAsync(int userId);
        Task<CoinWalletDto> CreateAsync(CoinWalletCreateRequest request);
        Task<CoinWalletDto> UpdateAsync(Guid id, CoinWalletUpdateRequest request);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<CoinWalletDto>> FilterAsync(CoinWalletFilterRequest request);
        Task<CoinWalletSummaryResponse> GetSummaryAsync();
        Task<bool> CreditAsync(int userId, decimal amount);
        Task<bool> DebitAsync(int userId, decimal amount);
        Task<bool> HasEnoughBalanceAsync(int userId, decimal amount);
        Task<CoinWalletDto> AdjustAsync(int userId, CoinWalletAdjustRequest request);
    }

    public class CoinWalletService : ICoinWalletService
    {
        private readonly UserDbContext _context;

        public CoinWalletService(UserDbContext context)
        {
            _context = context;
        }

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

        public async Task<decimal> GetBalanceAsync(int userId)
        {
            var wallet = await _context.CoinWallets
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.UserId == userId);
            return wallet?.Balance ?? 0m;
        }

        public async Task<CoinWalletDto> CreateAsync(CoinWalletCreateRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                throw new InvalidOperationException("User not found");

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

            await _context.Entry(wallet).Reference(w => w.User).LoadAsync();

            return wallet.toDto();
        }

        public async Task<CoinWalletDto> UpdateAsync(Guid id, CoinWalletUpdateRequest request)
        {
            var wallet = await _context.CoinWallets.FindAsync(id);
            if (wallet == null)
                throw new InvalidOperationException("Wallet not found");

            if (request.Balance.HasValue)
            {
                if (request.Balance.Value < 0)
                    throw new ArgumentException("Balance cannot be negative");
                wallet.Balance = request.Balance.Value;
                wallet.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            await _context.Entry(wallet).Reference(w => w.User).LoadAsync();

            return wallet.toDto();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var wallet = await _context.CoinWallets.FindAsync(id);
            if (wallet == null)
                throw new InvalidOperationException("Wallet not found");

            _context.CoinWallets.Remove(wallet);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CoinWalletDto>> FilterAsync(CoinWalletFilterRequest request)
        {
            var query = _context.CoinWallets
                .Include(w => w.User)
                .AsQueryable();

            if (request.UserId.HasValue)
                query = query.Where(w => w.UserId == request.UserId.Value);

            if (request.MinBalance.HasValue)
                query = query.Where(w => w.Balance >= request.MinBalance.Value);

            if (request.MaxBalance.HasValue)
                query = query.Where(w => w.Balance <= request.MaxBalance.Value);

            if (request.StartDate.HasValue)
                query = query.Where(w => w.CreatedAt >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                query = query.Where(w => w.CreatedAt <= request.EndDate.Value);

            query = query.OrderByDescending(w => w.CreatedAt);

            var skip = (request.PageNumber - 1) * request.PageSize;
            query = query.Skip(skip).Take(request.PageSize);

            var wallets = await query.AsNoTracking().ToListAsync();
            return wallets.Select(w => w.toDto());
        }

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

            return new CoinWalletSummaryResponse
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
        }

        public async Task<bool> CreditAsync(int userId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Số xu phải lớn hơn 0");

            var wallet = await _context.CoinWallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null)
            {
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

        public async Task<bool> HasEnoughBalanceAsync(int userId, decimal amount)
        {
            var balance = await GetBalanceAsync(userId);
            return balance >= amount;
        }

        public async Task<CoinWalletDto> AdjustAsync(int userId, CoinWalletAdjustRequest request)
        {
            if (request.Amount <= 0)
                throw new ArgumentException("Amount must be greater than 0");

            var wallet = await _context.CoinWallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
            {
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
            await _context.Entry(wallet).Reference(w => w.User).LoadAsync();

            return wallet.toDto();
        }
    }
}

