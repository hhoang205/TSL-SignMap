using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Models;
using NetTopologySuite.Geometries;

namespace WebAppTrafficSign.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TrafficSign> TrafficSigns { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // User table
            builder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired();
                entity.Property(u => u.PasswordHash).IsRequired();
            });

            // TrafficSign table
            builder.Entity<TrafficSign>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Location).HasColumnType("geography");
            });
        }
    }
}
