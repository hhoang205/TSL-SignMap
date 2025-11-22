namespace UserService.Models
{
    /// Model lưu lịch sử giao dịch wallet (credit, debit, adjustment)
    /// Các giao dịch từ PaymentService, ContributionService, VotingService
    /// sẽ được aggregate qua endpoint riêng
    public class WalletTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; } // Positive for credit, negative for debit
        public string Type { get; set; } = string.Empty; // credit, debit, adjustment
        public string Status { get; set; } = "completed"; // pending, completed, failed, cancelled
        public string Description { get; set; } = string.Empty;
        public string? RelatedId { get; set; } // ID của related entity (payment, contribution, etc.)
        public string? RelatedType { get; set; } // payment, contribution, voting, etc.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

