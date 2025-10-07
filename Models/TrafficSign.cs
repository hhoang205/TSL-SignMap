using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebAppTrafficSign.Models
{
    public class TrafficSign
    {
        public int Id { get; set; }
        public string Type { get; set; }

        [Column(TypeName = "geometry")]
        public Point Location { get; set; }
        public string Status { get; set; } // e.g., "Active", "Inactive", "Under Maintenance"
        public string ImageUrl { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public List<string> Traffic { get; set; } = new List<string>();
    }
}
