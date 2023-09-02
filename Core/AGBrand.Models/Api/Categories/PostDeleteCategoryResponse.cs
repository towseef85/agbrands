using Newtonsoft.Json;

namespace AGBrand.Models.Api.Categories
{
    public class PostDeleteCategoryResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}