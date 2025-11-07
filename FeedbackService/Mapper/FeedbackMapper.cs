using FeedbackService.Models;
using FeedbackService.DTOs;

namespace FeedbackService.Mapper
{
    public static class FeedbackMapper
    {
        public static FeedbackDto ToDto(this Feedback feedback, string? username = null)
        {
            return new FeedbackDto()
            {
                Id = feedback.Id,
                UserId = feedback.UserId,
                Content = feedback.Content,
                Status = feedback.Status,
                CreatedAt = feedback.CreatedAt,
                ResolvedAt = feedback.ResolvedAt,
                Username = username
            };
        }
    }
}

