using Newtonsoft.Json;

namespace AGBrand.Packages.Models
{
    public sealed class GridPage
    {
        [JsonProperty("cssClass")]
        public string CssClass { get; set; }

        [JsonProperty("pageNumber")]
        public int PageNumber { get; set; }

        [JsonProperty("sortDirection")]
        public SortDirection SortDirection { get; set; }

        [JsonProperty("sortKey")]
        public string SortKey { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
