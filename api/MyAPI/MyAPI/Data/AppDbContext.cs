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
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(e => e.CategoryName).IsUnique();
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("decimal(12,2)");
                entity.Property(e => e.ProductName).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Brand).HasMaxLength(100);
                entity.Property(e => e.ImageUrl).HasMaxLength(255);
                entity.Property(e => e.Stock).HasDefaultValue(0);

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
