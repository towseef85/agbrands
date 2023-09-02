////using System;

////using Microsoft.AspNetCore.Cors.Infrastructure;
////using Microsoft.AspNetCore.Mvc;
////using Microsoft.AspNetCore.Mvc.Cors.Internal;
////using Microsoft.Extensions.Configuration;
////using Microsoft.Extensions.DependencyInjection;

////namespace AGBrand.Packages.Services
////{
////    public static class CorsService
////    {
////        public static void AddCors(this IServiceCollection services, IConfiguration configuration, Action<CorsOptions> corsAction = null)
////        {
////            services.AddCors(options =>
////            {
////                var corsOrigins = configuration["Settings:CorsOrigins"].Split(";", StringSplitOptions.RemoveEmptyEntries);

////                options.AddPolicy(
////                    "CorsPolicy",
////                    builder => builder.WithOrigins(corsOrigins)
////                        .AllowAnyMethod()
////                        .AllowAnyHeader()
////                        .AllowCredentials());

////                corsAction?.Invoke(options);
////            });

////            services.Configure<MvcOptions>(options =>
////            {
////                options.Filters.Add(new CorsAuthorizationFilterFactory("CorsPolicy"));
////            });
////        }
////    }
////}
