using AGBrand.Packages.Attributes;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Api.Brands
{
    public class PostAddBrandRequest
    {
        [Required]
        [Encode]
        [JsonProperty("name")]
        public string Name { get; set; }

        
        [Required]
        [JsonProperty("file")]

        public IFormFile File { get; set; }
    }
}
