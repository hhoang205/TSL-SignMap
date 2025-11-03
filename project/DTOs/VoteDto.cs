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
}
