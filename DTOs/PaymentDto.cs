namespace WebAppTrafficSign.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } // Navigation property từ User
    }

    public class PaymentCreateRequest
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
    }

    public class PaymentUpdateRequest
    {
        public decimal? Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
    }

    public class PaymentStatusUpdateRequest
    {
        public string Status { get; set; }
    }

    public class PaymentFilterRequest
    {
        public int? UserId { get; set; }
        public string? Status { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class PaymentSummaryResponse
    {
        public int TotalPayments { get; set; }
        public int CompletedPayments { get; set; }
        public int PendingPayments { get; set; }
        public int FailedPayments { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AverageAmount { get; set; }
        public decimal TotalCompletedAmount { get; set; }
    }
}