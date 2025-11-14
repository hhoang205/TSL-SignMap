namespace ContributionService.DTOs
{
    public class ContributionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? SignId { get; set; } // Nullable for "Add" action
        public string Action { get; set; }     // e.g., "Add", "Update", "Delete"
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Status { get; set; }     // Pending, Approved, Rejected
        public DateTime CreatedAt { get; set; }
        public string Type { get; set; } // Type of traffic sign (for "Add" action)
        public double? Latitude { get; set; } // Location latitude (for "Add" action)
        public double? Longitude { get; set; } // Location longitude (for "Add" action)
    }

    // Request để submit contribution mới (tốn 5 coins)
    public class ContributionCreateRequest
    {
        public int UserId { get; set; }
        public string Action { get; set; } // "Add", "Update", "Delete"
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        
        // Required for "Add" action
        public string? Type { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        
        // Required for "Update" or "Delete" action
        public int? SignId { get; set; } // Reference to existing TrafficSign
    }

    // Request để admin approve/reject contribution
    public class ContributionReviewRequest
    {
        public string Status { get; set; } // "Approved" or "Rejected"
        public string? AdminNote { get; set; } // Optional note from admin
        public decimal? RewardAmount { get; set; } // Optional reward amount (default 10 coins for approved)
    }

    // Request để filter contributions by status
    public class ContributionFilterRequest
    {
        public string? Status { get; set; } // "Pending", "Approved", "Rejected"
        public string? Action { get; set; } // "Add", "Update", "Delete"
        public int? UserId { get; set; } // Filter by user
    }
}

