namespace VotingService.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ContributionId { get; set; }
        public int Value { get; set; } // e.g., 1 for upvote, -1 for downvote
        public bool IsUpvote { get; set; } // true for upvote, false for downvote
        public float Weight { get; set; } // Weight of the vote, e.g., 1.0 for full vote, 0.5 for half vote
        public DateTime CreatedAt { get; set; }
    }
}

