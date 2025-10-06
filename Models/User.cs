

namespace WebAppTrafficSign.Models
{
    public class User 
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public int RoleId { get; set; }
        public string Email { get; set; }
        public int PhoneNumber { get; set; }
        public float Reputation { get; set; }
        public CoinWallet Wallet { get; set; } = new CoinWallet();
        public List<Notification> Notification { get; set; } = new List<Notification>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
    }
}
