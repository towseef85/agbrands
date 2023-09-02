namespace AGBrand.Packages.Models.Configs.Services
{
    public class AddRepositoryConfig
    {
        public int CommandTimeout { get; set; }
        public string ConnectionString { get; set; }
        public string MigrationAssembly { get; set; }
        public int RetryOnFailureCount { get; set; }
    }
}
