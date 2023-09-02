using AGBrand.Packages.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Api.Products
{
    public class PostAddProductRequest
    {
        [Required]
        [Encode]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Required]
        [Encode]
        [JsonProperty("shortDescription")]
        public string ShortDescription { get; set; }

        [Encode]
        [JsonProperty("description")]
        public string Description { get; set; }

        [Required]
        [Range(0, float.MaxValue)]
        [JsonProperty("price")]
        public float Price { get; set; }
    }
}
