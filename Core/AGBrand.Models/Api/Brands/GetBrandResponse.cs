using Newtonsoft.Json;

namespace AGBrand.Models.Api.Brands
{
    public class GetBrandResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }
    }
}
