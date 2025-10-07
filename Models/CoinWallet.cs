namespace WebAppTrafficSign.Models
{
    public class CoinWallet
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public User User { get; set; }
    }

}
