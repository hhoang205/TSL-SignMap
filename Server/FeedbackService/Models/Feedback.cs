namespace FeedbackService.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public string Status { get; set; } // e.g., "Pending", "Reviewed", "Resolved"
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}

