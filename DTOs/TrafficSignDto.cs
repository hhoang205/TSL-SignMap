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

    /// Request để tạo traffic sign mới (admin only hoặc từ approved contribution)
    public class TrafficSignCreateRequest
    {
        public string Type { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Status { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    /// Request để cập nhật traffic sign
    public class TrafficSignUpdateRequest
    {
        public string? Type { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Status { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    /// Request để search traffic signs (có thể filter by type hoặc location)
    /// Tốn 1 coin nếu dùng advanced filters (type hoặc proximity)
    public class TrafficSignSearchRequest
    {
        public string? Type { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? RadiusKm { get; set; } // Radius in kilometers
        public int? UserId { get; set; } // For coin charging
    }

    /// Request để filter traffic signs theo proximity (bán kính)
    /// Tốn 1 coin
    public class TrafficSignProximityFilterRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double RadiusKm { get; set; }
        public int? UserId { get; set; } // For coin charging
    }
}
