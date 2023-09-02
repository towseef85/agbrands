using System.ComponentModel.DataAnnotations;
using AGBrand.Packages.Attributes;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AGBrand.Models.Api.Categories
{
    public class PostAddCategoryRequest
    {
        [Required]
        [Encode]
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("parentId")]
        public int? ParentId { get; set; }

        [Required]
        [JsonProperty("file")]

        public IFormFile File { get; set; }
    }
}