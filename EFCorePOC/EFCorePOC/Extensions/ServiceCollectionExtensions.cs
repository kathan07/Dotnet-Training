using EFCore.Data.Repositories.CategoryRepository;
using EFCore.Data.Repositories.OrderRepository;
using EFCore.Data.Repositories.ProductRepository;
using EFCore.Data.Repositories.UserRepository;
using EFCore.Service.CategoryService;
using EFCore.Service.OrderService;
using EFCore.Service.ProductService;
using EFCore.Service.UserService;

namespace EFCorePOC.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderService, OrderService>();
            return services;
        }
    }
}
