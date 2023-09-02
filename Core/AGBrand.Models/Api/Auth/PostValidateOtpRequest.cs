using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using AGBrand.Packages.Attributes;

namespace AGBrand.Models.Api.Auth
{
    public class PostValidateOtpRequest
    {
        [Required]
        [Encode]
        [JsonProperty("otp")]
        public string Otp { get; set; }

        [Required]
        [JsonProperty("otpTokenSigned")]
        public OtpTokenSigned OtpTokenSigned { get; set; }
    }
}
