using Newtonsoft.Json;

namespace AGBrand.Models.Api.Categories
{
    public class PostUpdateCategoryResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

    }
}