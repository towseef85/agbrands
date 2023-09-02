using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using AGBrand.Packages.Attributes;

namespace AGBrand.Models.Api.Auth
{
    public class OtpToken
    {
        [JsonProperty("data")]
        public string Data { get; set; }

        [Required]
        [JsonProperty("expiresOn")]
        public DateTime ExpiresOn { get; set; }

        [Required]
        [JsonProperty("otpId")]
        public Guid OtpId { get; set; }

        [Required]
        [Encode]
        [JsonProperty("signInId")]
        public string SignInId { get; set; }
    }
}
