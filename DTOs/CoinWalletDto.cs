using WebAppTrafficSign.Models;

namespace WebAppTrafficSign.DTOs
{
    public class CoinWalletDto
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserDto? User { get; set; }
    }
}
