using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;

namespace WebAppTrafficSign.Mapper
{
    public static class VoteMapper
    {
        public static VoteDto toDto(this Vote vote)
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

