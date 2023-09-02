namespace AGBrand.Packages.Models.Configs.Services
{
    public class ExceptionHandlerAppBuilderConfig
    {
        public string ContentType { get; set; } = "application/json";
        public bool IsSecureEnvironment { get; set; }
        public string LoggerName { get; set; } = "GlobalException";
        public int StatusCode { get; set; } = 500;
    }
}
