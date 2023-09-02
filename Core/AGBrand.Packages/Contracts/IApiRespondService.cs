using Microsoft.AspNetCore.Http;

namespace AGBrand.Packages.Contracts
{
    public interface IApiRespondService
    {
        bool IsSecureEnvironment { get; }

        IContextLogger ContextLogger { get; }

        IHttpContextAccessor HttpContextAccessor { get; }
    }
}
