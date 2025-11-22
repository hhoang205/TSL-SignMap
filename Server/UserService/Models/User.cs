namespace UserService.Models
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
        public string PhoneNumber { get; set; }
        public float Reputation { get; set; }
        public string? FcmToken { get; set; } // Firebase Cloud Messaging token for push notifications
        public CoinWallet Wallet { get; set; }
        // Note: Navigation properties to other services (Contributions, Votes, etc.) 
        // are removed as they belong to other microservices
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
    }
}

