using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Api.Auth
{
    public class PostSignUpResponse
    {
        [Required]
        [JsonProperty("otpTokenSigned")]
        public OtpTokenSigned OtpTokenSigned { get; set; }
    }
}
