using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using AGBrand.Models.Api.Auth;
using AGBrand.Packages.Contracts;
using AGBrand.Packages.Models;
using AGBrand.Packages.Util;
using AGBrand.Repository;
using static AGBrand.Packages.Util.Hash;
using AGBrand.Models;

namespace AGBrand.Implementations.Api.Base
{
    public class BaseBL
    {
        private readonly IConfiguration _configuration;
        private readonly IContextLogger _contextLogger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SqlContext _repository;

        public BaseBL(SqlContext repository,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IContextLogger contextLogger)
        {
            _repository = repository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _contextLogger = contextLogger;
        }

        protected async Task<(ServiceStatus<T> serviceStatus, bool actionTokenInvalid)> VerifyActionToken<T>(ActionTokenSigned actionTokenSigned)
        {
            (ServiceStatus<T> serviceStatus, bool actionTokenInvalid) = (null, true);

            var actionTokenHash = new Hash(HashServiceProvider.Sha512)
            {
                Salt = _configuration["Settings:PasswordSalt"]
            }.Encrypt(JsonConvert.SerializeObject(actionTokenSigned.ActionToken, Formatting.None));

            if (actionTokenHash != actionTokenSigned.ActionTokenHash)
            {
                serviceStatus = new ServiceStatus<T>
                {
                    Code = HttpStatusCode.Unauthorized,
                    Message = "Action Token Signature Invalid"
                };

                actionTokenInvalid = true;
                return (serviceStatus, actionTokenInvalid);
            }

            var action = await _repository.Actions.FirstOrDefaultAsync(c => c.Id == actionTokenSigned.ActionToken.ActionId &&
                                                                 c.SignInId == actionTokenSigned.ActionToken.SignInId);

            if (action == null)
            {
                serviceStatus = new ServiceStatus<T>
                {
                    Code = HttpStatusCode.Unauthorized,
                    Message = "Invalid Action"
                };

                actionTokenInvalid = true;
                return (serviceStatus, actionTokenInvalid);
            }

            if (action.ExpiresOn < DateTime.UtcNow)
            {
                serviceStatus = new ServiceStatus<T>
                {
                    Code = HttpStatusCode.Unauthorized,
                    Message = "Action Expired"
                };

                actionTokenInvalid = true;
                return (serviceStatus, actionTokenInvalid);
            }

            _repository.Actions.Remove(action);

            actionTokenInvalid = false;

            return (serviceStatus, actionTokenInvalid);
        }

        protected SignInIdType GetSignInIdType(string signInId)
        {
            if (RegexUtilities.IsValidEmail(signInId))
            {
                return SignInIdType.Email;
            }
            else if (RegexUtilities.IsMobileNumber(signInId))
            {
                return SignInIdType.Mobile;
            }

            return SignInIdType.Invalid;
        }
    }
}
