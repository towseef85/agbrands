using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using AGBrand.Packages.Attributes;

namespace AGBrand.Models.Api.Auth
{
    public class PostSignUpRequest
    {
        [JsonProperty("password")]
        [Required]
        public string Password { get; set; }

        [JsonProperty("signInId")]
        [Encode]
        [Required]
        public string SignInId { get; set; }
    }
}
