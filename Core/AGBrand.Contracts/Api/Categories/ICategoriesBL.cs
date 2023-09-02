using AGBrand.Models.Api.Categories;
using AGBrand.Packages.Models;
using System.Threading.Tasks;

namespace AGBrand.Contracts.Api.Categories
{
    public interface ICategoriesBL
    {
        Task<ServiceStatus<GetCategoriesResponse>> GetAsync();
        Task<ServiceStatus<GetCategoryResponse>> GetByIdAsync(int id);
        Task<ServiceStatus<PostAddCategoryResponse>> AddAsync(PostAddCategoryRequest postAddCategoryRequest);
        Task<ServiceStatus<PostDeleteCategoryResponse>> DeleteAsync(int id);
        Task<ServiceStatus<PostUpdateCategoryResponse>> EditAsync(PostUpdateCategoryRequest postUpdateCategoryRequest);
        Task<ServiceStatus<GetCategoryProductsResponse>> GetCategoryProducts(GetCategoryProductsRequest getCategoryProductsRequest);
    }
}
