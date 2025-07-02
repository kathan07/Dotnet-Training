using Microsoft.AspNetCore.Authentication;

namespace MVC_POC.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/Auth"))
            {
                await _next(context);
                return;
            }
            var userJson = context.Session.GetString("User");
            if (string.IsNullOrEmpty(userJson))
            {
                context.Response.Redirect("/Auth/Signin");
                return;
            }

            await _next(context);
        }
    }
    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
