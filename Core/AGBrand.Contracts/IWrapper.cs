using AGBrand.Contracts.Api.Auth;
using AGBrand.Contracts.Api.Brands;
using AGBrand.Contracts.Api.Categories;
using AGBrand.Contracts.Api.Products;

namespace AGBrand.Contracts
{
    public interface IWrapper
    {
        IProductsBL ProductsBL { get; }
        ICategoriesBL CategoriesBL { get; }
        IAuthBL AuthBL { get; }
        IUsersBL UsersBL { get; }
        IBrandsBL BrandsBL { get; }
    }
}
