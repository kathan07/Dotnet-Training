using Final_POC.Core.DTOs;

namespace Final_POC.Web.Services.ApiServices
{
    public interface IApiServices
    {
        Task<bool> LoginAsync(LoginUserDto user);
        Task<T> GetAsync<T>(string endpoint, object? queryParams);
        Task<T> PostAsync<T>(string endpoint, object data);
        Task<T> PutAsync<T>(string endpoint, object data);
        Task<T> PatchAsync<T>(string endpoint, object data);
        Task<T> DeleteAsync<T>(string endpoint);
    }
}
