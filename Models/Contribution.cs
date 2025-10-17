namespace WebAppTrafficSign.Models
{
    public class Contribution
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SignId { get; set; }
        public int TrafficSignId { get; set; }
        public string Action { get; set; } // e.g., "Add", "Update", "Delete"
        public string Description { get; set; }
        public string Status { get; set; } // e.g., "Pending", "Approved", "Rejected"
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public User User { get; set; }
        public TrafficSign TrafficSign { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
