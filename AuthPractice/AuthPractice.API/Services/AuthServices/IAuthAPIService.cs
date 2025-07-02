using AuthPractice.Core.DTOs;

namespace AuthPractice.API.Services.AuthServices
{
    public interface IAuthAPIService
    {
        Task<LoginResponse?> AuthenticateAsync(LoginRequest request);
        Task<bool> RegisterAsync(RegisterRequest request);
    }
}
