using AGBrand.Models.Api.Brands;
using AGBrand.Packages.Models;
using System.Threading.Tasks;

namespace AGBrand.Contracts.Api.Brands
{
    public interface IBrandsBL
    {
        Task<ServiceStatus<GetBrandResponse>> GetByIdAsync(int id);
        Task<ServiceStatus<GetBrandsResponse>> GetAsync(PagerArgs pagerArgs);
        Task<ServiceStatus<PostAddBrandResponse>> AddAsync(PostAddBrandRequest postAddBrandRequest);
        Task<ServiceStatus<PostDeleteBrandResponse>> DeleteAsync(int id);
        Task<ServiceStatus<PostUpdateBrandResponse>> EditAsync(PostUpdateBrandRequest postUpdateBrandRequest);
    }
}
