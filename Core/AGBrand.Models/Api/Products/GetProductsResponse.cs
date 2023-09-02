using AGBrand.Packages.Models;
using Newtonsoft.Json;

namespace AGBrand.Models.Api.Products
{
    public class GetProductsResponse
    {
        [JsonProperty("products")]
        public GridModel<GetProduct> Products { get; set; }
    }
}
