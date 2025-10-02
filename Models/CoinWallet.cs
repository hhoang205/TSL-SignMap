namespace WebAppTrafficSign.Models
{
    public class CoinWallet
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
