using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using AGBrand.Packages.Models.Configs.Services;

namespace AGBrand.Packages.Services
{
    public static class DbRepositoryService
    {
        public static void AddRepository<TContext>(this IServiceCollection services, AddRepositoryConfig config) where TContext : DbContext
        {
            services.AddDbContextPool<TContext>(o =>
                o.UseSqlServer(config.ConnectionString,
                    b =>
                    {
                        b.MigrationsAssembly(config.MigrationAssembly);
                        b.CommandTimeout(config.CommandTimeout);
                        b.EnableRetryOnFailure(config.RetryOnFailureCount);
                    })
                .ConfigureWarnings(warnings =>
                {
                })).AddScoped<TContext>();
        }

        public static void InitMigration<TContext>(this IApplicationBuilder app,
            IConfiguration configuration,
            Action<TContext, IConfiguration> seedMethod) where TContext : DbContext
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();

            serviceScope.ServiceProvider.GetService<TContext>().Database.Migrate();
            serviceScope.ServiceProvider.GetService<TContext>().EnsureSeeded(configuration, seedMethod);
        }

        private static void EnsureSeeded<TContext>(this TContext context, IConfiguration configuration, Action<TContext, IConfiguration> seedMethod)
        {
            seedMethod(context, configuration);
        }
    }
}
