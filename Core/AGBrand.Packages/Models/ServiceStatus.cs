using System.Net;

namespace AGBrand.Packages.Models
{
    public class ServiceStatus<TResponse>
    {
        public HttpStatusCode Code { get; set; }
        public string Message { get; set; }
        public TResponse Object { get; set; }
    }
}
