using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using AGBrand.Packages.Attributes;

namespace AGBrand.Models.Api.Auth
{
    public class PostGetOtpRequest
    {
        [Required]
        [Encode]
        [JsonProperty("signInId")]
        public string SignInId { get; set; }
    }
}
