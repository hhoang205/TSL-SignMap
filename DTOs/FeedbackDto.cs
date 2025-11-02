namespace WebAppTrafficSign.DTOs
{
    public class FeedbackDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public string Status { get; set; } // "Pending", "Reviewed", "Resolved"
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string? Username { get; set; } // Optional: from User navigation
    }

    public class FeedbackCreateRequest
    {
        public int UserId { get; set; }
        public string Content { get; set; }
    }

    public class FeedbackUpdateRequest
    {
        public string? Content { get; set; }
        public string? Status { get; set; } // "Pending", "Reviewed", "Resolved"
    }

    public class FeedbackStatusUpdateRequest
    {
        public string Status { get; set; } // "Pending", "Reviewed", "Resolved"
        public bool AutoResolve { get; set; } = false; // If true, set ResolvedAt to current time when status = "Resolved"
    }

    public class FeedbackFilterRequest
    {
        public int? UserId { get; set; }
        public string? Status { get; set; } // "Pending", "Reviewed", "Resolved"
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Search { get; set; } // Search in Content
        public bool? IsResolved { get; set; } // null: all, true: resolved (ResolvedAt != null), false: not resolved
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class FeedbackSummaryResponse
    {
        public int TotalFeedbacks { get; set; }
        public int PendingFeedbacks { get; set; }
        public int ReviewedFeedbacks { get; set; }
        public int ResolvedFeedbacks { get; set; }
        public double AverageResolutionTime { get; set; } // Average time from CreatedAt to ResolvedAt (in days)
    }
}

