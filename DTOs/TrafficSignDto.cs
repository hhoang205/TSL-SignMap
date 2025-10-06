namespace WebAppTrafficSign.DTOs
{
    public class TrafficSignDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string ImageUrl { get; set; }
    }
}
