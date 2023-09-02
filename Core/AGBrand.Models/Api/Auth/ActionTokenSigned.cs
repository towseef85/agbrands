using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Api.Auth
{
    public class ActionTokenSigned
    {
        [Required]
        [JsonProperty("actionToken")]
        public ActionToken ActionToken { get; set; }

        [Required]
        [JsonProperty("actionTokenHash")]
        public string ActionTokenHash { get; set; }
    }
}
