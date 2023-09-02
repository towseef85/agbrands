using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using AGBrand.Contracts.Api.Auth;
using AGBrand.Models;
using AGBrand.Models.Api.Auth;
using AGBrand.Models.Api.Users;
using AGBrand.Packages.Contracts;
using AGBrand.Packages.Models;
using AGBrand.Packages.Util;
using AGBrand.Repository;
using static AGBrand.Packages.Util.Hash;
using AGBrand.Implementations.Api.Base;
using AGBrand.Models.Domain;

namespace AGBrand.Implementations.Api.Users
{
    public class UsersBL : BaseBL, IUsersBL
    {
        private readonly IConfiguration _configuration;
        private readonly IContextLogger _contextLogger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SqlContext _repository;

        public UsersBL(SqlContext repository,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IContextLogger contextLogger) : base(repository, configuration, httpContextAccessor, contextLogger)
        {
            _repository = repository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _contextLogger = contextLogger;
        }

        public async Task<ServiceStatus<PostAddUserResponse>> AddAsync(PostAddUserRequest postAddUserRequest)
        {
            var dataDesrialized = JsonConvert.DeserializeObject(postAddUserRequest.ActionTokenSigned.ActionToken.Data, typeof(CreateUserActionData));

            if (!(dataDesrialized is CreateUserActionData))
            {
                return new ServiceStatus<PostAddUserResponse>
                {
                    Code = HttpStatusCode.BadRequest,
                    Message = "Invalid Data Object"
                };
            }

            var (serviceStatus, actionTokenInvalid) = await VerifyActionToken<PostAddUserResponse>(postAddUserRequest.ActionTokenSigned);

            if (actionTokenInvalid)
            {
                return serviceStatus;
            }

            var signInIdType = GetSignInIdType(postAddUserRequest.ActionTokenSigned.ActionToken.SignInId);

            var user = new User
            {
                Email = signInIdType == SignInIdType.Email ? postAddUserRequest.ActionTokenSigned.ActionToken.SignInId : null,
                Mobile = signInIdType == SignInIdType.Mobile ? postAddUserRequest.ActionTokenSigned.ActionToken.SignInId : null,
                Id = Guid.NewGuid(),
                IsDeleted = false,
                IsEmailVerified = signInIdType == SignInIdType.Email,
                IsMobileVerified = signInIdType == SignInIdType.Mobile,
                PasswordHash = new Hash(HashServiceProvider.Sha512)
                {
                    Salt = _configuration["Settings:PasswordSalt"]
                }.Encrypt(((CreateUserActionData)dataDesrialized).Password)
            };

            await _repository.Users.AddAsync(user);

            await _repository.SaveChangesAsync();

            return new ServiceStatus<PostAddUserResponse>
            {
                Code = HttpStatusCode.Created,
                Message = "User Created Successfully",
                Object = new PostAddUserResponse
                {
                    Id = user.Id
                }
            };
        }






        #region PrivateMethods




        #endregion PrivateMethods
    }
}
