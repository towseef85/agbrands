using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Api.Auth
{
    public class PostRefreshTokenRequest
    {
        [Required]
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }

        [Required]
        [JsonProperty("userId")]
        public Guid UserId { get; set; }
    }
}
