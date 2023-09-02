using Newtonsoft.Json;

namespace AGBrand.Packages.Models.JWT
{
    /// <summary>
    /// RSA Configuration Class Model
    /// </summary>
    public class RsaConfig
    {
        [JsonProperty("D")]
        public string D { get; set; }

        [JsonProperty("DP")]
        public string DP { get; set; }

        [JsonProperty("DQ")]
        public string DQ { get; set; }

        [JsonProperty("Exponent")]
        public string Exponent { get; set; }

        [JsonProperty("InverseQ")]
        public string InverseQ { get; set; }

        [JsonProperty("Modulus")]
        public string Modulus { get; set; }

        [JsonProperty("P")]
        public string P { get; set; }

        [JsonProperty("Q")]
        public string Q { get; set; }
    }
}
