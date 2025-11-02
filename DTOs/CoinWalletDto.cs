using WebAppTrafficSign.Models;

namespace WebAppTrafficSign.DTOs
{
    /// DTO cho CoinWallet với User navigation
    public class CoinWalletDto
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Username { get; set; } = string.Empty;
    }

    /// Request để tạo wallet mới
    public class CoinWalletCreateRequest
    {
        public int UserId { get; set; }
        public decimal InitialBalance { get; set; } = 20m; // Default 20 coins theo requirement
    }

    /// Request để cập nhật wallet (chủ yếu cho admin)
    public class CoinWalletUpdateRequest
    {
        public decimal? Balance { get; set; }
    }

    /// Request để admin điều chỉnh coin (reward/penalty)
    public class CoinWalletAdjustRequest
    {
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty; // e.g., "Reward for contribution", "Penalty for violation"
        public string AdjustmentType { get; set; } = "Credit"; // "Credit" or "Debit"
    }

    /// Request để filter wallets
    public class CoinWalletFilterRequest
    {
        public int? UserId { get; set; }
        public decimal? MinBalance { get; set; }
        public decimal? MaxBalance { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    /// Response tổng hợp wallets
    public class CoinWalletSummaryResponse
    {
        public int TotalWallets { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal AverageBalance { get; set; }
        public decimal MinBalance { get; set; }
        public decimal MaxBalance { get; set; }
        public int WalletsWithZeroBalance { get; set; }
        public int WalletsWithLowBalance { get; set; } // Balance < 10
        public int WalletsWithHighBalance { get; set; } // Balance >= 100
    }
}
