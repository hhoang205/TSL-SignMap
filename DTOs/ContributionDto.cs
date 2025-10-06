namespace WebAppTrafficSign.DTOs
{
    public class ContributionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SignId { get; set; }
        public string Action { get; set; }     // e.g., "Add", "Update", "Delete"
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Status { get; set; }     // Pending, Approved, Rejected
        public DateTime CreatedAt { get; set; }
    }
}
