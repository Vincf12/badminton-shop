
namespace MyAPI.Extensions
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAccountService, AccountService>();

            services.AddScoped<IUserAdminService, UserAdminService>();
            services.AddScoped<IDashboardService, DashboardService>();

            services.AddScoped<IBannerService, BannerService>();

            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            services.AddScoped<IProductSpecService, ProductSpecService>();
            services.AddScoped<IProductVariantService, ProductVariantService>();
            services.AddScoped<IReviewService, ReviewService>();

            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IShipmentService, ShipmentService>();
            services.AddScoped<IWishlistService, WishlistService>();

            return services;
        }
    }
}

