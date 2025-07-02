using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Final_POC.Web.Middleware
{
    public class AuthRedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LinkGenerator _linkGenerator;

        public AuthRedirectMiddleware(RequestDelegate next, LinkGenerator linkGenerator)
        {
            _next = next;
            _linkGenerator = linkGenerator;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLower();
            var isAuthenticated = context.User.Identity?.IsAuthenticated == true;

            // Authenticated users should not access login page
            if (isAuthenticated && path.Contains("/auth/login"))
            {
                var redirectUrl = _linkGenerator.GetPathByAction("Privacy", "Home");
                context.Response.Redirect(redirectUrl ?? "/");
                return;
            }

            await _next(context);
        }

    }
}
