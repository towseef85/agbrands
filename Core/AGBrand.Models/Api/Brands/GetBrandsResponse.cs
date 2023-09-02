using AGBrand.Packages.Models;
using Newtonsoft.Json;

namespace AGBrand.Models.Api.Brands
{
    public class GetBrandsResponse
    {
        [JsonProperty("brands")]
        public GridModel<GetBrandResponse> Brands { get; set; }
    }
}
