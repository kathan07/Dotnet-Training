using Microsoft.EntityFrameworkCore;
using EFCore.Data.Contexts;
using EFCore.Data.Repositories.UserRepository;
using EFCore.Service.UserService;
using EFCore.Service.ProductService;
using EFCore.Data.Repositories.ProductRepository;
using EFCore.Service.CategoryService;
using EFCore.Data.Repositories.CategoryRepository;
using EFCore.Data.Repositories.OrderRepository;
using EFCore.Service.OrderService;
using AutoMapper;
using EFCore.Service.AutoMapper;
using EFCorePOC.Web.Middleware;
using EFCorePOC.Web.Extensions;


namespace EFCorePOC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddHttpContextAccessor();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddAppDependencies();


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("EFCore.Data") // Specify the assembly where migrations are stored
            ));

            builder.Services.AddAutoMapper(typeof(MappingProfile));


            //builder.Services.AddScoped<IUserRepository, UserRepository>();
            //builder.Services.AddScoped<IUserService, UserService>();
            //builder.Services.AddScoped<IProductRepository, ProductRepository>();
            //builder.Services.AddScoped<IProductService, ProductService>();
            //builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            //builder.Services.AddScoped<ICategoryService, CategoryService>();
            //builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            //builder.Services.AddScoped<IOrderService, OrderService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();


            app.UseRouting();
            app.UseSession();
            app.UseAuthMiddleware();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
