using AGBrand.Packages.Attributes;
using Newtonsoft.Json;

namespace AGBrand.Packages.Models
{
    public sealed class PagerArgs
    {
        [JsonProperty("currentPage")]
        public int? CurrentPage { get; set; }

        [JsonProperty("pageSize")]
        public int? PageSize { get; set; }

        [Encode]
        [JsonProperty("searchTerm")]
        public string SearchTerm { get; set; }

        [JsonProperty("sortDirection")]
        public SortDirection SortDirection { get; set; }

        [Encode]
        [JsonProperty("sortKey")]
        public string SortKey { get; set; }
    }
}
