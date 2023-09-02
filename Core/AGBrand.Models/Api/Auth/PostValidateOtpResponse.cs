using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Api.Auth
{
    public class PostValidateOtpResponse
    {
        [Required]
        [JsonProperty("actionTokenSigned")]
        public ActionTokenSigned ActionTokenSigned { get; set; }
    }
}
