using Newtonsoft.Json;

namespace AGBrand.Models.Api.Products
{
    public class GetProduct
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("shortDescription")]
        public string ShortDescription { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price")]
        public float Price { get; set; }
    }
}
