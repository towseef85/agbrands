using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AGBrand.Packages.Services
{
    public static class SecurityHeadersAppBuilder
    {
        public static void AddSecurityHeaders(this IApplicationBuilder app, Action<HttpContext> contextAction = null)
        {
            app.Use((context, next) =>
            {
                context.Response.Headers.Clear();
                context.Response.Headers.Remove("x-powered-by");
                context.Response.Headers.Remove("X-Powered-By");
                context.Response.Headers.Remove("server");

                context.Response.Headers.Add("X-Frame-Options", "deny");
                context.Response.Headers.Add("X-Xss-Protection", "1");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");

                contextAction?.Invoke(context);

                return next();
            });
        }

        public static void ConfigureHsts(this IServiceCollection services)
        {
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });
        }

        public static void ConfigureHttpsRedirection(this IServiceCollection services)
        {
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                options.HttpsPort = 443;
            });
        }
    }
}
