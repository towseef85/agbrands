using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Api.Auth
{
    public class PostResendOtpResponse
    {
        [JsonProperty("otpTokenSigned")]
        [Required]
        public OtpTokenSigned OtpTokenSigned { get; set; }
    }
}
