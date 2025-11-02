using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;

namespace WebAppTrafficSign.Mapper
{
    public static class PaymentExtensions
    {
        public static PaymentDto toDto(this Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                UserId = payment.UserId,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                Username = payment.User?.Username ?? string.Empty
            };
        }
    }
}