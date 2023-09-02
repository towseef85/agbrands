using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;
using AGBrand.Packages.Contracts;

namespace AGBrand.Packages.Providers
{
    public class ContextLogger : IContextLogger
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private StringBuilder log;

        public ContextLogger(ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor)
        {
            _logger = loggerFactory.CreateLogger("ContextLogger");
            _httpContextAccessor = httpContextAccessor;
        }

        public void Commit(EventLogEntryType eventLogEntryType, string id)
        {
            if (log != null)
            {
                _httpContextAccessor.HttpContext.Response.Headers.Add("correlation-id", id);

                if (log.ToString().IndexOf(id) < 0)
                {
                    log.Insert(0, $"Correlation Id: {id} \n");
                }

                switch (eventLogEntryType)
                {
                    case EventLogEntryType.Error:
                        _logger.LogError(log.ToString());
                        break;

                    case EventLogEntryType.Warning:
                        _logger.LogWarning(log.ToString());
                        break;

                    default:
                        _logger.LogInformation(log.ToString());
                        break;
                }
            }
        }

        public void Log(string message)
        {
            if (log == null)
            {
                log = new StringBuilder();
            }

            log.Append(message);
            log.Append("\n");
        }
    }
}
