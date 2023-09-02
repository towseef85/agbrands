using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Api.Auth
{
    public class OtpTokenSigned
    {
        [Required]
        [JsonProperty("otpToken")]
        public OtpToken OtpToken { get; set; }

        [Required]
        [JsonProperty("otpTokenHash")]
        public string OtpTokenHash { get; set; }
    }
}
