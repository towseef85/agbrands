using AGBrand.Contracts.Api.Brands;
using AGBrand.Implementations.Api.Base;
using AGBrand.Models.Api.Brands;
using AGBrand.Models.Domain;
using AGBrand.Packages.Contracts;
using AGBrand.Packages.Models;
using AGBrand.Packages.Util;
using AGBrand.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AGBrand.Implementations.Api.Brands
{

    public class BrandsBL : BaseBL, IBrandsBL
    {
        private readonly IConfiguration _configuration;
        private readonly IContextLogger _contextLogger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SqlContext _repository;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public BrandsBL(SqlContext repository,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IContextLogger contextLogger,
            IWebHostEnvironment hostingEnvironment
            ) : base(repository, configuration, httpContextAccessor, contextLogger)
        {
            _repository = repository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _contextLogger = contextLogger;
            _hostingEnvironment = hostingEnvironment;
        }


        public async Task<ServiceStatus<GetBrandResponse>> GetByIdAsync(int id)
        {
            var brand = await _repository.Brands
                .Where(c => c.Id == id)
                .Select(c => new GetBrandResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImageUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}/{ c.ImageUrl}",
                }).FirstOrDefaultAsync();

            return new ServiceStatus<GetBrandResponse>
            {
                Code = HttpStatusCode.OK,
                Message = $"Get Brand By Id [{id}] Successful",
                Object = brand
            };
        }

        public async Task<ServiceStatus<GetBrandsResponse>> GetAsync(PagerArgs pagerArgs)
        {
            var gm = pagerArgs.GetGridModel<GetBrandResponse>(nameof(GetBrandResponse.Id));

            gm.Data = await _repository.Brands
                .Select(c => new GetBrandResponse
                {

                    Id = c.Id,
                    Name = c.Name,
                    ImageUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}/{ c.ImageUrl}",
                })
                .FilterBySearchTerm(c => c.Name.Contains(pagerArgs.SearchTerm), pagerArgs.SearchTerm)
                .ApplyPagingFilter(gm)
                .ToListAsync();

            return new ServiceStatus<GetBrandsResponse>
            {
                Code = HttpStatusCode.OK,
                Message = "Successfully Fetched All Brands",
                Object = new GetBrandsResponse
                {
                    Brands = gm
                }
            };
        }

        public async Task<ServiceStatus<PostAddBrandResponse>> AddAsync(PostAddBrandRequest postAddBrandRequest)
        {
            var brandByName = await _repository.Brands.FirstOrDefaultAsync(c => c.Name == postAddBrandRequest.Name);

            if (brandByName != null)
            {
                return new ServiceStatus<PostAddBrandResponse>
                {
                    Code = HttpStatusCode.Conflict,
                    Message = $"Brand by name: [{postAddBrandRequest.Name}] already exists by Id: [{brandByName.Id}]",
                    Object = null
                };
            }
            var fr = Utilities.UploadFile(_hostingEnvironment, postAddBrandRequest.File, new FileUploadSettings
            {
                FileType = FileType.Image,
                MaxSize = 10,
                StoragePath = LocalStorages.BrandsImageStoragePath
            });

            if (fr.IsSuccess)
            {
                var brand = new Brand
                {

                    Name = postAddBrandRequest.Name,
                    ImageUrl = fr.Result.ToString(),
                    CreatedOn = DateTime.UtcNow
                };

                await _repository.Brands.AddAsync(brand);

                await _repository.SaveChangesAsync();

                _contextLogger.Log($"Brand with name: [{postAddBrandRequest.Name}] successfully created with Id: [{brand.Id}] by User: [Anonymous] at DateTime: [{DateTime.UtcNow}]");

                return new ServiceStatus<PostAddBrandResponse>
                {
                    Code = HttpStatusCode.Created,
                    Message = $"Brand with name: [{postAddBrandRequest.Name}] successfully created with Id: [{brand.Id}]",
                    Object = new PostAddBrandResponse
                    {
                        Id = brand.Id
                    }
                };

            }
            return new ServiceStatus<PostAddBrandResponse>
            {
                Code = HttpStatusCode.BadRequest,
                Message = fr.Message,
                Object = null
            };
        }

        public async Task<ServiceStatus<PostDeleteBrandResponse>> DeleteAsync(int id)
        {
            var brandByName = await _repository.Brands.FirstOrDefaultAsync(c => c.Id == id);

            if (brandByName == null)
            {
                return new ServiceStatus<PostDeleteBrandResponse>
                {
                    Code = HttpStatusCode.Conflict,
                    Message = $"Brand by Id: [{id}] doesn't exists.",
                    Object = null
                };
            }


            brandByName.IsDeleted = true;
            brandByName.UpdatedOn = DateTime.UtcNow;
            _repository.Brands.Update(brandByName);

            await _repository.SaveChangesAsync();

            _contextLogger.Log($"Brand with name: [{brandByName.Name}] successfully created with Id: [{brandByName.Id}] by User: [Anonymous] at DateTime: [{DateTime.UtcNow}]");

            return new ServiceStatus<PostDeleteBrandResponse>
            {
                Code = HttpStatusCode.OK,
                Message = $"Brand with name: [{brandByName.Name}] successfully created with Id: [{brandByName.Id}]",
                Object = null
            };
        }

        public async Task<ServiceStatus<PostUpdateBrandResponse>> EditAsync(PostUpdateBrandRequest postUpdateBrandRequest)
        {
            var brandByName = await _repository.Brands.FirstOrDefaultAsync(c => c.Id == postUpdateBrandRequest.Id);

            if (brandByName == null || _repository.Brands.Any(c => c.Id != postUpdateBrandRequest.Id && c.Name == postUpdateBrandRequest.Name))
            {
                return new ServiceStatus<PostUpdateBrandResponse>
                {
                    Code = HttpStatusCode.Conflict,
                    Message = brandByName == null ? $"Brand by Name: [{postUpdateBrandRequest.Name}] doesn't exists." : $"Brand Name: [{postUpdateBrandRequest.Name}] already exists.",
                    Object = null
                };
            }
            brandByName.Name = postUpdateBrandRequest.Name;
            var fr = Utilities.UploadFile(_hostingEnvironment, postUpdateBrandRequest.File, new FileUploadSettings
            {
                FileType = FileType.Image,
                MaxSize = 10,
                StoragePath = LocalStorages.BrandsImageStoragePath
            });

            if (fr.IsSuccess)
            {
                brandByName.ImageUrl = fr.Result.ToString();
                brandByName.UpdatedOn = DateTime.UtcNow;
                _repository.Brands.Update(brandByName);
                await _repository.SaveChangesAsync();
                _contextLogger.Log($"Brand with name: [{brandByName.Name}] successfully created with Id: [{brandByName.Id}] by User: [Anonymous] at DateTime: [{DateTime.UtcNow}]");

                return new ServiceStatus<PostUpdateBrandResponse>
                {
                    Code = HttpStatusCode.OK,
                    Message = $"Brand with name: [{brandByName.Name}] successfully Updated with Id: [{brandByName.Id}]",
                    Object = new PostUpdateBrandResponse
                    {
                        Id = brandByName.Id
                    }
                };
            }
            return new ServiceStatus<PostUpdateBrandResponse>
            {
                Code = HttpStatusCode.BadRequest,
                Message = fr.Message,
                Object = null
            };


        }

    }
}
