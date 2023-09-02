using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using AGBrand.Packages.Providers;

namespace AGBrand.Packages.Services
{
    public static class ApiVersioningService
    {
        public static void AddApiVersioning(this IServiceCollection services, IConfiguration configuration, Action<ApiVersioningOptions> versioningAction = null)
        {
            services.AddApiVersioning(options =>
            {
                var isSecureEnvironment = bool.Parse(configuration["Settings:IsSecureEnvironment"]);

                options.ReportApiVersions = !isSecureEnvironment;
                options.AssumeDefaultVersionWhenUnspecified = !isSecureEnvironment;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");

                if (isSecureEnvironment)
                {
                    options.ErrorResponses = new ErrorReponsesSuppressor();
                }

                versioningAction?.Invoke(options);
            });
        }
    }
}
