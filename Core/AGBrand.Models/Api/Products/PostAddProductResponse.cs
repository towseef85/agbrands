using Newtonsoft.Json;

namespace AGBrand.Models.Api.Products
{
    public class PostAddProductResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
