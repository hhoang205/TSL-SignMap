using VotingService.Models;
using VotingService.DTOs;

namespace VotingService.Mapper
{
    public static class VoteMapper
    {
        public static VoteDto ToDto(this Vote vote)
        {
            return new VoteDto()
            {
                Id = vote.Id,
                ContributionId = vote.ContributionId,
                UserId = vote.UserId,
                Value = vote.IsUpvote,
                Weight = vote.Weight,
                CreatedAt = vote.CreatedAt
            };
        }
    }
}

