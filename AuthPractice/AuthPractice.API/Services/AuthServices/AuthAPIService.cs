using AuthPractice.Business.Services.AuthService;
using AuthPractice.Core.DTOs;

namespace AuthPractice.API.Services.AuthServices
{
    public class AuthAPIService: IAuthAPIService
    {
        private readonly IAuthBusinessService _authService;

        public AuthAPIService(IAuthBusinessService authService)
        {
            _authService = authService;
        }

        public async Task<LoginResponse?> AuthenticateAsync(LoginRequest request)
        {
            return await _authService.AuthenticateAsync(request);
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            return await _authService.RegisterAsync(request);
        }
    }
}
