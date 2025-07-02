using AuthPractice.Core.DTOs;

namespace AuthPractice.Web.Services.WebAuthService
{
    public interface IWebAuthService
    {
        Task<bool> LoginAsync(LoginRequest request, HttpContext httpContext);
        Task LogoutAsync(HttpContext httpContext);
        Task<bool> RegisterAsync(RegisterRequest request);
        string GetToken(HttpContext httpContext);
    }
}
