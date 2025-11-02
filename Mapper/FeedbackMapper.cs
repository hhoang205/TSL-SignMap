using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;

namespace WebAppTrafficSign.Mapper
{
    public static class FeedbackMapper
    {
        public static FeedbackDto toDto(this Feedback feedback)
        {
            return new FeedbackDto()
            {
                Id = feedback.Id,
                UserId = feedback.UserId,
                Content = feedback.Content,
                Status = feedback.Status,
                CreatedAt = feedback.CreatedAt,
                ResolvedAt = feedback.ResolvedAt,
                Username = feedback.User?.Username
            };
        }
    }
}

