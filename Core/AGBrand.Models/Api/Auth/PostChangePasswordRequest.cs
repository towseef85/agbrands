using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Api.Auth
{
    public class PostChangePasswordRequest
    {
        [JsonProperty("actionTokenSigned")]
        [Required]
        public ActionTokenSigned ActionTokenSigned { get; set; }

        [JsonProperty("password")]
        [Required]
        public string Password { get; set; }
    }
}
