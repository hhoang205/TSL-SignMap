using FeedbackService.DTOs;

namespace FeedbackService.Services
{
    public interface IFeedbackService
    {
        Task<FeedbackDto> GetByIdAsync(int id);
        Task<IEnumerable<FeedbackDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<FeedbackDto>> GetByStatusAsync(string status);
        Task<FeedbackDto> CreateAsync(FeedbackCreateRequest request);
        Task<FeedbackDto> UpdateAsync(int id, FeedbackUpdateRequest request);
        Task<FeedbackDto> UpdateStatusAsync(int id, FeedbackStatusUpdateRequest request);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<FeedbackDto>> FilterAsync(FeedbackFilterRequest request);
        Task<FeedbackSummaryResponse> GetSummaryAsync();
    }
}

