using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using AGBrand.Models.Api.Auth;

namespace AGBrand.Models.Api.Users
{
    public class PostAddUserRequest
    {
        [JsonProperty("actionTokenSigned")]
        [Required]
        public ActionTokenSigned ActionTokenSigned { get; set; }
    }
}
