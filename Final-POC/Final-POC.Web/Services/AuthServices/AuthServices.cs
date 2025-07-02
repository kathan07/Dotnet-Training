using Final_POC.Core.DTOs;
using Final_POC.Web.Services.ApiServices;

namespace Final_POC.Web.Services.AuthServices
{
    public class AuthServices: IAuthServices
    {
        private readonly IApiServices _apiServices;
        private readonly ILogger<AuthServices> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _accessToken;

        public AuthServices(IApiServices apiServices, ILogger<AuthServices> logger, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _apiServices = apiServices;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _accessToken = configuration["ApiSettings:AccessToken"]!;
        }

        public async Task<bool> LoginAsync(LoginUserDto loginRequest)
        {
            try
            {
                _logger.LogInformation("Attempting login for user: {Email}", loginRequest.Email);
                return await _apiServices.LoginAsync(loginRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login attempt for {Email}", loginRequest.Email);
                return false;
            }
        }

        public void Logout()
        {
            try
            {
                _logger.LogInformation("Logging out user");
                var context = _httpContextAccessor.HttpContext;

                if (context != null)
                {
                    context.Response.Cookies.Delete(_accessToken);
                    _logger.LogInformation("User logged out successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
            }
        }

    }
}
