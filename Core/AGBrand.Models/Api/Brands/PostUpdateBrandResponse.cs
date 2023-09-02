using Newtonsoft.Json;

namespace AGBrand.Models.Api.Brands
{
    public class PostUpdateBrandResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
    }
}