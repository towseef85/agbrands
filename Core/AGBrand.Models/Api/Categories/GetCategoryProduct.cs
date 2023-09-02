using Newtonsoft.Json;
using System.Collections.Generic;

namespace AGBrand.Models.Api.Categories
{
    public class GetCategoryProduct
    {
        [JsonProperty("productId")]
        public int ProductId { get; set; }

        [JsonProperty("categoryId")]
        public int CategoryId { get; set; }

        [JsonProperty("brandId")]
        public int BrandId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("shortDescription")]
        public string ShortDescription { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price")]
        public float Price { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("itemCode")]
        public string ItemCode { get; set; }

        [JsonProperty("images")]
        public List<GetCategoryProductImage> Images { get; set; }
    }
}