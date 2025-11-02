

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
        public string PhoneNumber { get; set; }
        public float Reputation { get; set; }
        public CoinWallet Wallet { get; set; }
        public List<Notification> Notifications { get; set; } = new();
        public ICollection<Contribution> Contributions { get; set; }
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public ICollection<Feedback> Feedbacks { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
    }
}
