using System.Security.Claims;
using AuthPractice.Service.Services.AuthService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using AuthPractice.Core.DTOs;

namespace AuthPractice.Web.Services.WebAuthService
{
    public class WebAuthService: IWebAuthService
    {
        private readonly IAuthService _authService;
        private const string TokenKey = "JwtToken";

        public WebAuthService(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> LoginAsync(LoginRequest request, HttpContext httpContext)
        {
            var response = await _authService.LoginAsync(request);
            if (response == null)
                return false;

            // Store the JWT token
            httpContext.Session.SetString(TokenKey, response.Token);

            // Create claims for cookie auth
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, response.User.Id.ToString()),
                new Claim(ClaimTypes.Name, response.User.Username),
                new Claim(ClaimTypes.Email, response.User.Email),
                new Claim(ClaimTypes.Role, response.User.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return true;
        }

        public async Task LogoutAsync(HttpContext httpContext)
        {
            httpContext.Session.Remove(TokenKey);
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            return await _authService.RegisterAsync(request);
        }

        public string GetToken(HttpContext httpContext)
        {
            return httpContext.Session.GetString(TokenKey);
        }
    }
}
