using Final_POC.API.Services.AuthServices;
using Final_POC.API.Services.TaskServices;
using Final_POC.API.Services.UserServices;
using Final_POC.Business.Services.AuthService;
using Final_POC.Business.Services.TaskService;
using Final_POC.Business.Services.UserService;
using Final_POC.Data.Repositories.TaskRepository;
using Final_POC.Data.Repositories.UserRepository;

namespace Final_POC.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IAuthBusinessService, AuthBusinessService>();
            services.AddScoped<IUserBusinessService, UserBusinessService>();
            services.AddScoped<ITaskBusinessService, TaskBusinessService>();
            services.AddScoped<IUserApiService, UserApiService>();
            services.AddScoped<IAuthApiService, AuthApiService>();
            services.AddScoped<ITaskApiService, TaskApiService>();
            return services;
        }
    }
}
