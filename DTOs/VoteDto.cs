namespace WebAppTrafficSign.DTOs
{
    public class VoteDto
    {
        public int Id { get; set; }
        public int ContributionId { get; set; }
        public int UserId { get; set; }
        public bool Value { get; set; }    // true: upvote, false: downvote
        public float Weight { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class VoteCreateRequest
    {
        public int ContributionId { get; set; }
        public int UserId { get; set; }
        public bool Value { get; set; }    // true: upvote, false: downvote
        public float Weight { get; set; } = 1.0f;
    }

    public class VoteUpdateRequest
    {
        public bool? Value { get; set; }    // true: upvote, false: downvote
        public float? Weight { get; set; }
    }

    public class VoteFilterRequest
    {
        public int? ContributionId { get; set; }
        public int? UserId { get; set; }
        public bool? Value { get; set; }    // Filter by upvote/downvote
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class VoteSummaryResponse
    {
        public int ContributionId { get; set; }
        public int TotalVotes { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public double AverageWeight { get; set; }
        public double TotalScore { get; set; }  // Sum of (Value * Weight)
    }
}
