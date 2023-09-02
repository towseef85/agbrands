using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using System.Linq;
using AGBrand.Repository;

namespace AGBrand.Implementations
{
    /// <summary>
    /// Repository Seed Extension
    /// </summary>
    public static class SqlContextExtension
    {
        /// <summary>
        /// Repository Migration
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool AllMigrationsApplied(this DbContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }

        /// <summary>
        /// Repository Seed
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configuration"></param>
        public static void Seed(SqlContext context, IConfiguration configuration)
        {
        }
    }
}
