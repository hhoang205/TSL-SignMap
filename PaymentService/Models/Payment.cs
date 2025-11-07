namespace PaymentService.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } // e.g., Credit Card, PayPal
        public string Status { get; set; } // e.g., Completed, Pending, Failed
    }
}

