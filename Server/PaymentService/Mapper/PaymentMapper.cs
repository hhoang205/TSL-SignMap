using PaymentService.Models;
using PaymentService.DTOs;

namespace PaymentService.Mapper
{
    public static class PaymentMapper
    {
        public static PaymentDto ToDto(this Payment payment, string username = "")
        {
            return new PaymentDto
            {
                Id = payment.Id,
                UserId = payment.UserId,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                Username = username
            };
        }
    }
}

