using System;
using System.Collections.Generic;
using AGBrand.Packages.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGBrand.Models.Api.Categories
{
    public class GetCategoryResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("parentId")]
        public int? ParentId { get; set; }
    }

    public class GetCategoriesResponse
    {
        public List<GetCategories> GetCategories { get; set; }
    }
    public class GetCategories
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Name { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        public int? ParentId { get; set; }

    }
}