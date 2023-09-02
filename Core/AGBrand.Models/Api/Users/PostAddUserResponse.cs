using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Api.Users
{
    public class PostAddUserResponse
    {
        [JsonProperty("id")]
        [Required]
        public Guid Id { get; set; }
    }
}
