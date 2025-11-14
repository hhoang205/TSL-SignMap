using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<CoinWallet> CoinWallets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // =========================
            // User
            // =========================
            builder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Password).IsRequired();
                entity.Property(u => u.RoleId).IsRequired();
                entity.Property(u => u.CreatedAt).IsRequired();

                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();

                // 1–1: User ↔ CoinWallet (Wallet là dependent, FK ở CoinWallet.UserId)
                entity.HasOne(u => u.Wallet)
                      .WithOne(w => w.User)
                      .HasForeignKey<CoinWallet>(w => w.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================
            // CoinWallet
            // =========================
            builder.Entity<CoinWallet>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Balance)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(c => c.CreatedAt).IsRequired();

                // Đảm bảo quan hệ 1–1
                entity.HasIndex(c => c.UserId).IsUnique();
            });
        }
    }
}

