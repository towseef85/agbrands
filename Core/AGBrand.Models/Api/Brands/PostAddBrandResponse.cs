using Newtonsoft.Json;

namespace AGBrand.Models.Api.Brands
{
    public class PostAddBrandResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}