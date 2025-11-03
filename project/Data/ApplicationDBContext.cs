using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Reflection.Emit;
using WebAppTrafficSign.Models;

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
        public DbSet<CoinWallet> CoinWallets { get; set; }
        public DbSet<Contribution> Contributions { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public DbSet<Feedback> FeedBacks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            // User table
            builder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired();
                entity.Property(u => u.Password).IsRequired();

                entity.HasOne(u => u.Wallet)
                      .WithOne(w => w.User)
                      .HasForeignKey<CoinWallet>(w => w.UserId);
            });
            builder.Entity<User>()
               .Navigation(u => u.Wallet)
               .AutoInclude();
            builder.Entity<User>()
                .Navigation(u => u.Notification)
                .AutoInclude();

            // TrafficSign table
            builder.Entity<TrafficSign>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Location).HasColumnType("geography");
            });

            //Wallet table
            builder.Entity<CoinWallet>(entity =>
            {
                entity.HasKey(w => w.Id);
                entity.Property(w => w.Balance).HasColumnType("decimal(18,2)");
            });
            builder.Entity<Feedback>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.Content).IsRequired();
                entity.Property(f => f.Status).IsRequired();
                entity.Property(f => f.CreatedAt).IsRequired();
            });
        }
    }
}
