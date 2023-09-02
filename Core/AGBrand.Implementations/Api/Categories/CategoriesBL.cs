using System;
using AGBrand.Contracts.Api.Categories;
using AGBrand.Implementations.Api.Base;
using AGBrand.Models.Api.Categories;
using AGBrand.Packages.Contracts;
using AGBrand.Packages.Models;
using AGBrand.Packages.Util;
using AGBrand.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AGBrand.Models.Domain;
using Microsoft.AspNetCore.Hosting;

namespace AGBrand.Implementations.Api.Categories
{
    public class CategoriesBL : BaseBL, ICategoriesBL
    {
        private readonly IConfiguration _configuration;
        private readonly IContextLogger _contextLogger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SqlContext _repository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public CategoriesBL(SqlContext repository,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IContextLogger contextLogger, IWebHostEnvironment hostingEnvironment) : base(repository, configuration, httpContextAccessor, contextLogger)
        {
            _repository = repository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _contextLogger = contextLogger;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<ServiceStatus<GetCategoryResponse>> GetByIdAsync(int id)
        {
            var category = await _repository.Categories
                .Where(c => c.Id == id)
                .Select(c => new GetCategoryResponse
                {
                    Id = c.Id,
                    Name = c.Title,
                    ParentId = c.ParentCategoryId,
                    ImageUrl = c.ImageUrl
                }).FirstOrDefaultAsync();

            category.ImageUrl = GetAbsoluteUrl(category.ImageUrl);

            return new ServiceStatus<GetCategoryResponse>
            {
                Code = HttpStatusCode.OK,
                Message = $"Get Category By Id [{id}] Successful",
                Object = category
            };
        }

        private string GetAbsoluteUrl(string relativeUrl)
        {
            return $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}:{_httpContextAccessor.HttpContext.Request.Host.Port}/{relativeUrl}";
        }

        public async Task<ServiceStatus<GetCategoriesResponse>> GetAsync()
        {
            var data = await _repository.Categories
                .Select(c => new GetCategories
                {
                    Id = c.Id,
                    Name = c.Title,
                    ParentId = c.ParentCategoryId,
                    ImageUrl = c.ImageUrl
                }).ToListAsync();

            data = data.Select(c =>
            {
                c.ImageUrl = GetAbsoluteUrl(c.ImageUrl);

                return c;
            }).ToList();

            return new ServiceStatus<GetCategoriesResponse>
            {
                Code = HttpStatusCode.OK,
                Message = "Successfully Fetched All Categories",
                Object = new GetCategoriesResponse
                {
                    GetCategories = data
                }
            };
        }

        ////public async Task<ServiceStatus<GetCategoriesResponse>> GetAsync(PagerArgs pagerArgs)
        ////{
        ////    var gm = pagerArgs.GetGridModel<GetCategory>(nameof(GetCategory.Id));

        ////    gm.Data = await _repository.Categories
        ////        .Select(c => new GetCategory
        ////        {

        ////            Id = c.Id,
        ////            Name = c.Title,
        ////            ImageUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}/{ c.ImageUrl}",
        ////        })
        ////        .FilterBySearchTerm(c => c.Name.Contains(pagerArgs.SearchTerm), pagerArgs.SearchTerm)
        ////        .ApplyPagingFilter(gm)
        ////        .ToListAsync();

        ////    return new ServiceStatus<GetCategoriesResponse>
        ////    {
        ////        Code = HttpStatusCode.OK,
        ////        Message = "Successfully Fetched All Categories",
        ////        Object = new GetCategoriesResponse
        ////        {
        ////            Category = gm
        ////        }
        ////    };
        ////}

        public async Task<ServiceStatus<PostAddCategoryResponse>> AddAsync(PostAddCategoryRequest postAddCategoryRequest)
        {
            var categoryByName = await _repository.Categories.FirstOrDefaultAsync(c => c.Title == postAddCategoryRequest.Name);

            if (categoryByName != null)
            {
                return new ServiceStatus<PostAddCategoryResponse>
                {
                    Code = HttpStatusCode.Conflict,
                    Message = $"Category by name: [{postAddCategoryRequest.Name}] already exists by Id: [{categoryByName.Id}]",
                    Object = null
                };
            }

            var fr = Utilities.UploadFile(_hostingEnvironment, postAddCategoryRequest.File, new FileUploadSettings
            {
                FileType = FileType.Image,
                MaxSize = 10,
                StoragePath = LocalStorages.CategoriesImageStoragePath
            });

            if (fr.IsSuccess)
            {
                var category = new Category
                {
                    Title = postAddCategoryRequest.Name,
                    ParentCategoryId = postAddCategoryRequest.ParentId,
                    ImageUrl = fr.Result.ToString(),
                    CreatedOn = DateTime.UtcNow
                };

                await _repository.Categories.AddAsync(category);

                await _repository.SaveChangesAsync();

                _contextLogger.Log($"Category with name: [{postAddCategoryRequest.Name}] successfully created with Id: [{category.Id}] by User: [Anonymous] at DateTime: [{DateTime.UtcNow}]");

                return new ServiceStatus<PostAddCategoryResponse>
                {
                    Code = HttpStatusCode.Created,
                    Message = $"Category with name: [{postAddCategoryRequest.Name}] successfully created with Id: [{category.Id}]",
                    Object = new PostAddCategoryResponse
                    {
                        Id = category.Id
                    }
                };
            }

            return new ServiceStatus<PostAddCategoryResponse>
            {
                Code = HttpStatusCode.BadRequest,
                Message = fr.Message,
                Object = null
            };
        }

        public async Task<ServiceStatus<PostDeleteCategoryResponse>> DeleteAsync(int id)
        {
            var categoryByName = await _repository.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (categoryByName == null)
            {
                return new ServiceStatus<PostDeleteCategoryResponse>
                {
                    Code = HttpStatusCode.Conflict,
                    Message = $"Category by Id: [{id}] doesn't exists.",
                    Object = null
                };
            }

            categoryByName.IsDeleted = true;
            categoryByName.UpdatedOn = DateTime.UtcNow;

            _repository.Categories.Update(categoryByName);

            await _repository.SaveChangesAsync();

            _contextLogger.Log($"Category with name: [{categoryByName.Title}] successfully deleted with Id: [{categoryByName.Id}] by User: [Anonymous] at DateTime: [{DateTime.UtcNow}]");

            return new ServiceStatus<PostDeleteCategoryResponse>
            {
                Code = HttpStatusCode.OK,
                Message = $"Category with name: [{categoryByName.Title}] successfully deleted with Id: [{categoryByName.Id}]",
                Object = null
            };
        }

        public async Task<ServiceStatus<PostUpdateCategoryResponse>> EditAsync(PostUpdateCategoryRequest postUpdateCategoryRequest)
        {
            var categoryByName = await _repository.Categories.FirstOrDefaultAsync(c => c.Id == postUpdateCategoryRequest.Id);

            if (categoryByName == null || _repository.Categories.Any(c => c.Id != postUpdateCategoryRequest.Id && c.Title == postUpdateCategoryRequest.Name))
            {
                return new ServiceStatus<PostUpdateCategoryResponse>
                {
                    Code = HttpStatusCode.Conflict,
                    Message = categoryByName == null ? $"Category by Name: [{postUpdateCategoryRequest.Name}] doesn't exists." : $"Category Name: [{postUpdateCategoryRequest.Name}] already exists.",
                    Object = null
                };
            }

            if (!string.IsNullOrEmpty(postUpdateCategoryRequest.Name))
            {
                categoryByName.Title = postUpdateCategoryRequest.Name;
            }

            if (postUpdateCategoryRequest.File != null)
            {
                var fr = Utilities.UploadFile(_hostingEnvironment, postUpdateCategoryRequest.File, new FileUploadSettings
                {
                    FileType = FileType.Image,
                    MaxSize = 10,
                    StoragePath = LocalStorages.CategoriesImageStoragePath
                });

                if (fr.IsSuccess)
                {
                    categoryByName.ImageUrl = fr.Result.ToString();
                }
                else
                {
                    return new ServiceStatus<PostUpdateCategoryResponse>
                    {
                        Code = HttpStatusCode.BadRequest,
                        Message = fr.Message,
                        Object = null
                    };
                }
            }

            categoryByName.ParentCategoryId = postUpdateCategoryRequest.ParentId;
            categoryByName.UpdatedOn = DateTime.UtcNow;

            _repository.Categories.Update(categoryByName);

            await _repository.SaveChangesAsync();

            _contextLogger.Log($"Category with name: [{categoryByName.Title}] successfully created with Id: [{categoryByName.Id}] by User: [Anonymous] at DateTime: [{DateTime.UtcNow}]");

            return new ServiceStatus<PostUpdateCategoryResponse>
            {
                Code = HttpStatusCode.OK,
                Message = $"Category with name: [{categoryByName.Title}] successfully Updated with Id: [{categoryByName.Id}]",
                Object = new PostUpdateCategoryResponse
                {
                    Id = categoryByName.Id
                }
            };
        }

        public async Task<ServiceStatus<GetCategoryProductsResponse>> GetCategoryProducts(GetCategoryProductsRequest getCategoryProductsRequest)
        {
            var searchTerm = getCategoryProductsRequest.PagerArgs.SearchTerm;

            var gm = Utilities.GetGridModel<GetCategoryProduct>(getCategoryProductsRequest.PagerArgs, nameof(GetCategoryProduct.ProductId));

            var query = _repository.CategoryProductsCache
                        .Include(c => c.Product)
                        .ThenInclude(c => c.ProductImages)
                        .Where(c => c.CategoryId == getCategoryProductsRequest.Id)
                        .Select(c => new GetCategoryProduct
                        {
                            BrandId = c.Product.BrandId,
                            CategoryId = c.CategoryId,
                            Sku = c.Product.Sku,
                            ShortDescription = c.Product.ShortDescription,
                            Description = c.Product.Description,
                            ItemCode = c.Product.ItemCode,
                            Name = c.Product.Name,
                            Price = c.Product.Price,
                            ProductId = c.ProductId,
                            Images = c.Product.ProductImages.Select(i => new GetCategoryProductImage
                            {
                                ImageUrl = i.ImageUrl,
                                IsMainImage = i.IsMainImage
                            }).ToList()
                        })
                        .FilterBySearchTerm(c => c.Name.Contains(searchTerm) ||
                                                c.ItemCode.Contains(searchTerm) ||
                                                c.Description.Contains(searchTerm) ||
                                                c.ShortDescription.Contains(searchTerm) ||
                                                c.Sku.Contains(searchTerm), searchTerm)
                        .ApplyPagingFilter(gm);

            gm.Data = await query.ToListAsync();

            return new ServiceStatus<GetCategoryProductsResponse>
            {
                Code = HttpStatusCode.OK,
                Message = $"Get Products By Category Id [{getCategoryProductsRequest.Id}] Successful",
                Object = new GetCategoryProductsResponse
                {
                    Products = gm
                }
            };

            ////.ThenInclude(c => c.ProductVariantValues)
            ////.ThenInclude(c => c.VariantValue)
            ////.ThenInclude(c => c.Variant)

            ////.SelectMany(c => c.Product.ProductVariantValues.Select(d => d.VariantValue.Variant))
            ////.Distinct()
            ////.Select(c => new
            ////{
            ////    Variant = c.Title,
            ////    Values = c.VariantValues.Select(c => c.Value)
            ////})
            ////.ToList();
        }
    }
}
