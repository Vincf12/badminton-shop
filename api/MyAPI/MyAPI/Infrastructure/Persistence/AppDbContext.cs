using Microsoft.EntityFrameworkCore;
using MyAPI.Domain.Entities;

namespace MyAPI.Infrastructure.Persistence
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
        public DbSet<Banner> Banners { get; set; } = null!;

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

            modelBuilder.Entity<Banner>(entity =>
            {
                entity.ToTable("banners");
                entity.HasKey(e => e.BannerId);

                entity.Property(e => e.BannerId).HasColumnName("banner_id");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.ImageUrl)
                    .HasColumnName("image_url")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.TargetType)
                    .HasColumnName("target_type")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(e => e.TargetId)
                    .HasColumnName("target_id")
                    .HasMaxLength(36);

                entity.Property(e => e.CustomUrl)
                    .HasColumnName("custom_url")
                    .HasMaxLength(500);

                entity.Property(e => e.Position)
                    .HasColumnName("position")
                    .HasMaxLength(50)
                    .HasDefaultValue("HOME_TOP")
                    .IsRequired();

                entity.Property(e => e.DisplayOrder)
                    .HasColumnName("display_order")
                    .HasDefaultValue(0);

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasDefaultValue(true);

                entity.Property(e => e.StartDate)
                    .HasColumnName("start_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.EndDate)
                    .HasColumnName("end_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValue(1);

                entity.HasIndex(e => new { e.Position, e.DisplayOrder });
                entity.HasIndex(e => e.IsActive);
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

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100).IsRequired();
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
                entity.Property(e => e.FullName).HasColumnName("full_name").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(20);
                entity.Property(e => e.Gender).HasColumnName("gender");
                entity.Property(e => e.Birthdate).HasColumnName("birthdate");
                entity.Property(e => e.Role).HasColumnName("role");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.EmailVerified).HasColumnName("email_verified");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.ToTable("coupons");
                entity.HasKey(e => e.CouponId);

                entity.Property(e => e.CouponId).HasColumnName("coupon_id");
                entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
                entity.Property(e => e.CouponName).HasColumnName("coupon_name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.DiscountType).HasColumnName("discount_type").HasMaxLength(20);
                entity.Property(e => e.DiscountValue).HasColumnName("discount_value");
                entity.Property(e => e.MinimumOrderAmount).HasColumnName("minimum_order_amount");
                entity.Property(e => e.MaximumDiscountAmount).HasColumnName("maximum_discount_amount");
                entity.Property(e => e.UsageLimit).HasColumnName("usage_limit");
                entity.Property(e => e.UsedCount).HasColumnName("used_count");
                entity.Property(e => e.StartDate).HasColumnName("start_date");
                entity.Property(e => e.EndDate).HasColumnName("end_date");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });

            modelBuilder.Entity<ProductSpec>(entity =>
            {
                entity.ToTable("product_specs");
                entity.HasKey(e => e.SpecId);

                entity.Property(e => e.SpecId).HasColumnName("spec_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.SpecName).HasColumnName("spec_name").HasMaxLength(100);
                entity.Property(e => e.SpecValue).HasColumnName("spec_value").HasMaxLength(255);

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");
                entity.HasKey(e => e.OrderId);

                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.CouponId).HasColumnName("coupon_id");
                entity.Property(e => e.OrderCode).HasColumnName("order_code").HasMaxLength(100);
                entity.Property(e => e.ShippingRecipientName).HasColumnName("shipping_recipient_name").HasMaxLength(100);
                entity.Property(e => e.ShippingPhone).HasColumnName("shipping_phone").HasMaxLength(20);
                entity.Property(e => e.ShippingProvince).HasColumnName("shipping_province").HasMaxLength(100);
                entity.Property(e => e.ShippingDistrict).HasColumnName("shipping_district").HasMaxLength(100);
                entity.Property(e => e.ShippingWard).HasColumnName("shipping_ward").HasMaxLength(100);
                entity.Property(e => e.ShippingAddressDetail).HasColumnName("shipping_address_detail");
                entity.Property(e => e.TotalAmount).HasColumnName("total_amount").HasColumnType("decimal(12,2)");
                entity.Property(e => e.ShippingFee).HasColumnName("shipping_fee").HasColumnType("decimal(12,2)");
                entity.Property(e => e.DiscountAmount).HasColumnName("discount_amount").HasColumnType("decimal(12,2)");
                entity.Property(e => e.FinalAmount).HasColumnName("final_amount").HasColumnType("decimal(12,2)");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50);
                entity.Property(e => e.TrackingCode).HasColumnName("tracking_code").HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Coupon)
                    .WithMany()
                    .HasForeignKey(e => e.CouponId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("order_details");
                entity.HasKey(e => e.OrderDetailId);

                entity.Property(e => e.OrderDetailId).HasColumnName("order_detail_id");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.VariantId).HasColumnName("variant_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.UnitPrice).HasColumnName("unit_price").HasColumnType("decimal(12,2)");
                entity.Property(e => e.SubTotal).HasColumnName("subtotal").HasColumnType("decimal(12,2)");

                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Variant)
                    .WithMany()
                    .HasForeignKey(e => e.VariantId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OrderStatusHistory>(entity =>
            {
                entity.ToTable("order_status_history");
                entity.HasKey(e => e.HistoryId);

                entity.Property(e => e.HistoryId).HasColumnName("history_id");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50);
                entity.Property(e => e.Note).HasColumnName("note");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("payments");
                entity.HasKey(e => e.PaymentId);

                entity.Property(e => e.PaymentId).HasColumnName("payment_id");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.PaymentMethod).HasColumnName("payment_method").HasMaxLength(50);
                entity.Property(e => e.PaymentStatus).HasColumnName("payment_status").HasMaxLength(50);
                entity.Property(e => e.Amount).HasColumnName("amount").HasColumnType("decimal(12,2)");
                entity.Property(e => e.TransactionCode).HasColumnName("transaction_code").HasMaxLength(100);
                entity.Property(e => e.PaidAt).HasColumnName("paid_at");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.ToTable("shipments");
                entity.HasKey(e => e.ShipmentId);
                entity.HasIndex(e => e.OrderId).IsUnique();
                entity.HasIndex(e => e.TrackingNumber).IsUnique();

                entity.Property(e => e.ShipmentId).HasColumnName("shipment_id");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.TrackingNumber).HasColumnName("tracking_number").HasMaxLength(100);
                entity.Property(e => e.Courier).HasColumnName("courier").HasMaxLength(100);
                entity.Property(e => e.ShippedDate).HasColumnName("shipped_date");
                entity.Property(e => e.DeliveredDate).HasColumnName("delivered_date");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50);

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

                entity.Property(e => e.ReviewId).HasColumnName("review_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.Rating).HasColumnName("rating");
                entity.Property(e => e.Comment).HasColumnName("comment");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

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

                entity.Property(e => e.WishlistId).HasColumnName("wishlist_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");

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

                entity.Property(e => e.WishlistItemId).HasColumnName("wishlist_item_id");
                entity.Property(e => e.WishlistId).HasColumnName("wishlist_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");

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
