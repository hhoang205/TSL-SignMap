using PaymentService.DTOs;

namespace PaymentService.Services
{
    public interface IPaymentService
    {
        Task<PaymentDto> GetByIdAsync(int id);
        Task<IEnumerable<PaymentDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<PaymentDto>> GetByStatusAsync(string status);
        Task<PaymentDto> CreateAsync(PaymentCreateRequest request);
        Task<PaymentDto> UpdateAsync(int id, PaymentUpdateRequest request);
        Task<PaymentDto> UpdateStatusAsync(int id, PaymentStatusUpdateRequest request);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<PaymentDto>> FilterAsync(PaymentFilterRequest request);
        Task<PaymentSummaryResponse> GetSummaryAsync();
    }
}

