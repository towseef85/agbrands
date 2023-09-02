using AGBrand.Packages.Models;
using Newtonsoft.Json;

namespace AGBrand.Models.Api.Categories
{
    public class GetCategoryProductsResponse
    {
        [JsonProperty("products")]
        public GridModel<GetCategoryProduct> Products { get; set; }
    }
}
