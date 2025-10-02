using System.Drawing;

namespace WebAppTrafficSign.Models
{
    public class TrafficSign
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public Point Location { get; set; }
        public string status { get; set; } // e.g., "Active", "Inactive", "Under Maintenance"
        public string ImageUrl { get; set; }
        public DateTime validFrom { get; set; }
        public DateTime validTo { get; set; }
    }
}
