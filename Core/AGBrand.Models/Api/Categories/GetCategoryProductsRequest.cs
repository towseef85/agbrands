using AGBrand.Packages.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AGBrand.Models.Api.Categories
{
    public class GetCategoryProductsRequest
    {
        [FromQuery]
        [JsonProperty("id")]
        public int Id { get; set; }

        [FromQuery]
        [JsonProperty("pagerArgs")]
        public PagerArgs PagerArgs { get; set; }
    }
}
