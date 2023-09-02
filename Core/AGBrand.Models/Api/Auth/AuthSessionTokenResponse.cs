using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using AGBrand.Packages.Attributes;

namespace AGBrand.Models.Api.Auth
{
    public class AuthSessionTokenResponse
    {
        [JsonProperty("email")]
        [Encode]
        public string Email { get; set; }

        [JsonProperty("mobile")]
        [Encode]
        public string Mobile { get; set; }

        [Required]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Required]
        [JsonProperty("refreshToken")]
        public SessionToken RefreshToken { get; set; }

        [Required]
        [JsonProperty("sessionToken")]
        public SessionToken SessionToken { get; set; }

        [Required]
        [JsonProperty("userId")]
        public Guid UserId { get; set; }
    }
}
