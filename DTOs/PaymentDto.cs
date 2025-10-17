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
}