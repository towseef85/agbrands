using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Api.Auth
{
    public class CreateUserActionData
    {
        [Required]
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
