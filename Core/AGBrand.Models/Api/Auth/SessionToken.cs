using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Api.Auth
{
    public class SessionToken
    {
        [Required]
        [JsonProperty("expiresOn")]
        public DateTime ExpiresOn { get; set; }

        [Required]
        [JsonProperty("scheme")]
        public string Scheme { get; set; }

        [Required]
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
