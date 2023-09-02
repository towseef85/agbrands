using AGBrand.Contracts.Api.Products;
using AGBrand.Implementations.Api.Base;
using AGBrand.Models.Api.Products;
using AGBrand.Models.Domain;
using AGBrand.Packages.Contracts;
using AGBrand.Packages.Models;
using AGBrand.Packages.Util;
using AGBrand.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AGBrand.Implementations.Api.Products
{
    public class ProductsBL : BaseBL, IProductsBL
    {
        private readonly IConfiguration _configuration;
        private readonly IContextLogger _contextLogger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SqlContext _repository;

        public ProductsBL(SqlContext repository,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IContextLogger contextLogger) : base(repository, configuration, httpContextAccessor, contextLogger)
        {
            _repository = repository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _contextLogger = contextLogger;
        }

        public async Task<ServiceStatus<GetProductsResponse>> GetAsync(PagerArgs pagerArgs)
        {
            var gm = Utilities.GetGridModel<GetProduct>(pagerArgs, nameof(GetProduct.Id));

            gm.Data = await _repository.Products
                .Select(c => new GetProduct
                {
                    Description = c.Description,
                    Id = c.Id,
                    Name = c.Name,
                    Price = c.Price,
                    ShortDescription = c.ShortDescription
                })
                .FilterBySearchTerm(c => c.Name.Contains(pagerArgs.SearchTerm) ||
                                         c.Description.Contains(pagerArgs.SearchTerm) ||
                                         c.ShortDescription.Contains(pagerArgs.SearchTerm), pagerArgs.SearchTerm)
                .ApplyPagingFilter(gm)
                .ToListAsync();

            return new ServiceStatus<GetProductsResponse>
            {
                Code = System.Net.HttpStatusCode.OK,
                Message = "Successfully Fetched All Products",
                Object = new GetProductsResponse
                {
                    Products = gm
                }
            };
        }

        public async Task<ServiceStatus<PostAddProductResponse>> AddAsync(PostAddProductRequest postAddProductRequest)
        {
            var productByName = await _repository.Products.FirstOrDefaultAsync(c => c.Name == postAddProductRequest.Name);

            if (productByName != null)
            {
                return new ServiceStatus<PostAddProductResponse>
                {
                    Code = System.Net.HttpStatusCode.Conflict,
                    Message = $"Product by name: [{postAddProductRequest.Name}] already exists by Id: [{productByName.Id}]",
                    Object = null
                };
            }

            var product = new Product
            {
                Description = postAddProductRequest.Description,
                Name = postAddProductRequest.Name,
                Price = postAddProductRequest.Price,
                ShortDescription = postAddProductRequest.ShortDescription
            };

            await _repository.Products.AddAsync(product);

            await _repository.SaveChangesAsync();

            _contextLogger.Log($"Product with name: [{postAddProductRequest.Name}] successfully created with Id: [{product.Id}] by User: [Anonymous] at DateTime: [{DateTime.UtcNow}]");

            return new ServiceStatus<PostAddProductResponse>
            {
                Code = System.Net.HttpStatusCode.Created,
                Message = $"Product with name: [{postAddProductRequest.Name}] successfully created with Id: [{product.Id}]",
                Object = new PostAddProductResponse
                {
                    Id = product.Id
                }
            };
        }
    }
}
