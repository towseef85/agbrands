using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using AGBrand.Packages.Util;

namespace AGBrand.Packages.Filters
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Models;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class XceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;

        public XceptionFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("GlobalException");
        }

        public void OnException(ExceptionContext context)
        {
            LogAsync(context).GetAwaiter().GetResult();
        }

        private Task LogAsync(ExceptionContext context)
        {
            context.ExceptionHandled = true;

            var (message, _) = context.Exception.GetExceptionMessage();

            _logger.Log(LogLevel.Error, message);

            var raiseError = new RaiseError(((int)HttpStatusCode.InternalServerError).ToString(), message);
            var errorSerialized = JsonConvert.SerializeObject(raiseError);

            var response = context.HttpContext.Response;
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.ContentType = new MediaTypeHeaderValue("application/json").ToString();

            return response.WriteAsync(new
            {
                message = errorSerialized
            }.ToString(), Encoding.UTF8);
        }
    }
}
