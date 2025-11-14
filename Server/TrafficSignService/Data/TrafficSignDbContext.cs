using Microsoft.EntityFrameworkCore;
using TrafficSignService.Models;
using NetTopologySuite.Geometries;

namespace TrafficSignService.Data
{
    public class TrafficSignDbContext : DbContext
    {
        public TrafficSignDbContext(DbContextOptions<TrafficSignDbContext> options)
            : base(options)
        {
        }

        public DbSet<TrafficSign> TrafficSigns { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TrafficSign>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Type).IsRequired();
                entity.Property(t => t.Location)
                      .HasColumnType("geography")
                      .IsRequired();
                entity.Property(t => t.Status).HasMaxLength(50);
                entity.Property(t => t.ValidFrom).IsRequired();
            });
        }
    }
}

