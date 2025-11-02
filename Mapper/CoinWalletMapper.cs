using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;

namespace WebAppTrafficSign.Mapper
{
    public static class CoinWalletExtensions
    {
        public static CoinWalletDto toDto(this CoinWallet wallet)
        {
            return new CoinWalletDto
            {
                Id = wallet.Id,
                UserId = wallet.UserId,
                Balance = wallet.Balance,
                CreatedAt = wallet.CreatedAt,
                UpdatedAt = wallet.UpdatedAt,
                Username = wallet.User?.Username ?? string.Empty
            };
        }
    }
}

