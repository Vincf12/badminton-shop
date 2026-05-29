using Microsoft.EntityFrameworkCore;
using MyAPI.Models;

namespace MyAPI.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Tables
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("decimal(12,2)");
                entity.Property(e => e.CostPrice).HasColumnType("decimal(12,2)");
                entity.Property(e => e.DiscountPrice).HasColumnType("decimal(12,2)");
                entity.Property(e => e.Weight).HasColumnType("decimal(8,2)");
                entity.Property(e => e.AvgRating).HasColumnType("decimal(3,2)");

                entity.Property(e => e.DiscountPercent).HasDefaultValue(0);
                entity.Property(e => e.TotalStock).HasDefaultValue(0);
                entity.Property(e => e.LowStockThreshold).HasDefaultValue(5);
                entity.Property(e => e.AllowBackorder).HasDefaultValue(false);
                entity.Property(e => e.IsFeatured).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsNew).HasDefaultValue(false);
                entity.Property(e => e.AvgRating).HasDefaultValue(0m);
                entity.Property(e => e.TotalReviews).HasDefaultValue(0);
                entity.Property(e => e.TotalSold).HasDefaultValue(0);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("NULL")
                    .ValueGeneratedOnAddOrUpdate();
            });
        }
    }
}
