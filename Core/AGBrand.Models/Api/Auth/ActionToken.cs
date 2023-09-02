using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using AGBrand.Packages.Attributes;

namespace AGBrand.Models.Api.Auth
{
    public class ActionToken
    {
        [Required]
        [JsonProperty("actionId")]
        public Guid ActionId { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [Required]
        [JsonProperty("expiresOn")]
        public DateTime ExpiresOn { get; set; }

        [Required]
        [JsonProperty("signInId")]
        [Encode]
        public string SignInId { get; set; }
    }
}
