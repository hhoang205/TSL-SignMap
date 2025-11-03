using System.Threading.Tasks;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;
using WebAppTrafficSign.Services.Interfaces;

namespace WebAppTrafficSign.Services
{
    public class CoinWalletService : ICoinWalletService
    {
        private readonly ApplicationDBContext _context;

        public CoinWalletService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task CreateWalletAsync(int userId, decimal initialBalance)
        {
            var wallet = new CoinWallet
            {
                UserId = userId,
                Balance = initialBalance,
                CreatedAt = DateTime.UtcNow
            };

            await _context.CoinWallets.AddAsync(wallet);
            await _context.SaveChangesAsync();
        }
    }
}