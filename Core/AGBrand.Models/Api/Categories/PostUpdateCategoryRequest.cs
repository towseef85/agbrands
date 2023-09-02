using System.ComponentModel.DataAnnotations;
using AGBrand.Packages.Attributes;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AGBrand.Models.Api.Categories
{
    public class PostUpdateCategoryRequest
    {
        [Required]
        [JsonProperty("id")]

        public int Id { get; set; }

        [Required]
        [Encode]
        [JsonProperty("name")]

        public string Name { get; set; }

        [JsonProperty("parentId")]
        public int? ParentId { get; set; }

        [JsonProperty("file")]
        public IFormFile File { get; set; }
    }
}