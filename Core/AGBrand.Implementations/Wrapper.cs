using AGBrand.Contracts;
using AGBrand.Contracts.Api.Auth;
using AGBrand.Contracts.Api.Brands;
using AGBrand.Contracts.Api.Categories;
using AGBrand.Contracts.Api.Products;
using AGBrand.Implementations.Api.Auth;
using AGBrand.Implementations.Api.Brands;
using AGBrand.Implementations.Api.Categories;
using AGBrand.Implementations.Api.Products;
using AGBrand.Implementations.Api.Users;
using AGBrand.Packages.Contracts;
using AGBrand.Packages.Helpers.JWT;
using AGBrand.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AGBrand.Implementations
{
    public class Wrapper : IWrapper, IApiRespondService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly SqlContext _sqlContext;
        private readonly JwtTokenBuilder _jwtTokenBuilder;
        private readonly IConfiguration _configuration;

        public Wrapper(SqlContext sqlContext,
           JwtTokenBuilder jwtTokenBuilder,
           IContextLogger contextLogger,
           IConfiguration configuration,
           IHttpContextAccessor httpContextAccessor,
           IWebHostEnvironment hostingEnvironment)
        {
            _sqlContext = sqlContext;
            _jwtTokenBuilder = jwtTokenBuilder;
            ContextLogger = contextLogger;
            _configuration = configuration;
            HttpContextAccessor = httpContextAccessor;
            IsSecureEnvironment = bool.Parse(configuration["Settings:IsSecureEnvironment"]);
            _hostingEnvironment = hostingEnvironment;
        }
        public Wrapper(SqlContext sqlContext,
            JwtTokenBuilder jwtTokenBuilder,
            IContextLogger contextLogger,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _sqlContext = sqlContext;
            _jwtTokenBuilder = jwtTokenBuilder;
            _configuration = configuration;

            HttpContextAccessor = httpContextAccessor;
            ContextLogger = contextLogger;
            IsSecureEnvironment = bool.Parse(configuration["Settings:IsSecureEnvironment"]);
        }

        public bool IsSecureEnvironment { get; }
        public IContextLogger ContextLogger { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }

        public IAuthBL AuthBL => new AuthBL(_sqlContext, _configuration, _jwtTokenBuilder, HttpContextAccessor, ContextLogger);
        public IUsersBL UsersBL => new UsersBL(_sqlContext, _configuration, HttpContextAccessor, ContextLogger);
        public IProductsBL ProductsBL => new ProductsBL(_sqlContext, _configuration, HttpContextAccessor, ContextLogger);
        public ICategoriesBL CategoriesBL => new CategoriesBL(_sqlContext, _configuration, HttpContextAccessor, ContextLogger, _hostingEnvironment);
        public IBrandsBL BrandsBL => new BrandsBL(_sqlContext, _configuration, HttpContextAccessor, ContextLogger, _hostingEnvironment);
    }
}
