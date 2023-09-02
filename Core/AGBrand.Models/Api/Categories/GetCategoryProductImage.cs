using Newtonsoft.Json;

namespace AGBrand.Models.Api.Categories
{
    public class GetCategoryProductImage
    {
        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("isMainImage")]
        public bool IsMainImage { get; set; }
    }
}