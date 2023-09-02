using Newtonsoft.Json;

namespace AGBrand.Models.Api.Categories
{
    public class PostAddCategoryResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}