using Newtonsoft.Json;

namespace AGBrand.Models.Api.Brands
{
    public class PostDeleteBrandResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}