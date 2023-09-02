using AGBrand.Models.Api.Products;
using AGBrand.Packages.Models;
using System.Threading.Tasks;

namespace AGBrand.Contracts.Api.Products
{
    public interface IProductsBL
    {
        Task<ServiceStatus<GetProductsResponse>> GetAsync(PagerArgs pagerArgs);

        Task<ServiceStatus<PostAddProductResponse>> AddAsync(PostAddProductRequest postAddProductRequest);
    }
}
