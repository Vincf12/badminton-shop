using Microsoft.EntityFrameworkCore;
using MyAPI.Models;

namespace MyAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Brand> Brands { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductImage> ProductImages { get; set; } = null!;
        public DbSet<ProductVariant> ProductVariants { get; set; } = null!;
        public DbSet<ProductSpec> ProductSpecs { get; set; } = null!;
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Coupon> Coupons { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<Shipment> Shipments { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Wishlist> Wishlists { get; set; } = null!;
        public DbSet<WishlistItem> WishlistItems { get; set; } = null!;
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");
                entity.HasKey(e => e.CategoryId);

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CategoryName)
                    .HasColumnName("category_name")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(e => e.CategoryName).IsUnique();
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("brands");
                entity.HasKey(e => e.BrandId);

                entity.Property(e => e.BrandId).HasColumnName("brand_id");

                entity.Property(e => e.BrandName)
                    .HasColumnName("brand_name")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(e => e.BrandName).IsUnique();
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");
                entity.HasKey(e => e.ProductId);

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .IsRequired();

                entity.Property(e => e.BrandId)
                    .HasColumnName("brand_id")
                    .IsRequired();

                entity.Property(e => e.ProductName)
                    .HasColumnName("product_name")
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.Slug)
                    .HasColumnName("slug")
                    .HasMaxLength(255);

                entity.Property(e => e.ShortDescription)
                    .HasColumnName("short_description");

                entity.Property(e => e.Description)
                    .HasColumnName("description");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasMaxLength(20)
                    .HasDefaultValue("active");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime")
                    .IsRequired(false);

                entity.HasOne(e => e.Category)
                    .WithMany()
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Brand)
                    .WithMany()
                    .HasForeignKey(e => e.BrandId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("product_images");
                entity.HasKey(e => e.ImageId);

                entity.Property(e => e.ImageId).HasColumnName("image_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.ImageUrl)
                    .HasColumnName("image_url")
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(e => e.IsMain)
                    .HasColumnName("is_main")
                    .HasDefaultValue(false);

                entity.Property(e => e.SortOrder)
                    .HasColumnName("sort_order")
                    .HasDefaultValue(0);

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("addresses");

                entity.HasKey(e => e.AddressId);

                entity.Property(e => e.AddressId)
                    .HasColumnName("address_id");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                entity.Property(e => e.RecipientName)
                    .HasColumnName("recipient_name")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.Province)
                    .HasColumnName("province")
                    .HasMaxLength(100);

                entity.Property(e => e.Ward)
                    .HasColumnName("ward")
                    .HasMaxLength(100);

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(100);

                entity.Property(e => e.AddressDetail)
                    .HasColumnName("address_detail");

                entity.Property(e => e.IsDefault)
                    .HasColumnName("is_default")
                    .HasDefaultValue(false);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("carts");

                entity.HasKey(e => e.CartId);

                entity.Property(e => e.CartId)
                    .HasColumnName("cart_id");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.ToTable("cart_items");

                entity.HasKey(e => e.CartItemId);

                entity.Property(e => e.CartItemId)
                    .HasColumnName("cart_item_id");

                entity.Property(e => e.CartId)
                    .HasColumnName("cart_id")
                    .IsRequired();

                entity.Property(e => e.VariantId)
                    .HasColumnName("variant_id")
                    .IsRequired();

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasDefaultValue(1);

                entity.HasOne(e => e.Cart)
                    .WithMany()
                    .HasForeignKey(e => e.CartId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ProductVariant)
                    .WithMany()
                    .HasForeignKey(e => e.VariantId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.ToTable("product_variants");
                entity.HasKey(e => e.VariantId);

                entity.Property(e => e.VariantId).HasColumnName("variant_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Sku)
                    .HasColumnName("sku")
                    .HasMaxLength(100);

                entity.Property(e => e.Weight)
                    .HasColumnName("weight")
                    .HasMaxLength(20);

                entity.Property(e => e.GripSize)
                    .HasColumnName("grip_size")
                    .HasMaxLength(20);

                entity.Property(e => e.Color)
                    .HasColumnName("color")
                    .HasMaxLength(50);

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(12,2)")
                    .IsRequired();

                entity.Property(e => e.StockQuantity)
                    .HasColumnName("stock_quantity")
                    .HasDefaultValue(0);

                entity.Property(e => e.ImageUrl)
                    .HasColumnName("image_url")
                    .HasMaxLength(255);

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.ToTable("shipments");
                entity.HasKey(e => e.ShipmentId);
                entity.HasIndex(e => e.OrderId).IsUnique();
                entity.HasIndex(e => e.TrackingNumber).IsUnique();

                entity.HasOne(e => e.Order)
                    .WithOne()
                    .HasForeignKey<Shipment>(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("reviews");
                entity.HasKey(e => e.ReviewId);
                entity.HasIndex(e => new { e.UserId, e.ProductId }).IsUnique();

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.ToTable("wishlists");
                entity.HasKey(e => e.WishlistId);
                entity.HasIndex(e => e.UserId).IsUnique();

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<WishlistItem>(entity =>
            {
                entity.ToTable("wishlist_items");
                entity.HasKey(e => e.WishlistItemId);
                entity.HasIndex(e => new { e.WishlistId, e.ProductId }).IsUnique();

                entity.HasOne(e => e.Wishlist)
                    .WithMany()
                    .HasForeignKey(e => e.WishlistId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
