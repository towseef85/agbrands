using Newtonsoft.Json;
using System.Collections.Generic;

namespace AGBrand.Packages.Models
{
    public sealed class GridPager
    {
        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("cndPage")]
        public int EndPage { get; set; }

        [JsonProperty("firstLinkCssClass")]
        public string FirstLinkCssClass { get; set; } = "First";

        [JsonProperty("firstLinkText")]
        public string FirstLinkText { get; set; } = "<<";

        [JsonProperty("lastLinkCssClass")]
        public string LastLinkCssClass { get; set; } = "Last";

        [JsonProperty("lastLinkText")]
        public string LastLinkText { get; set; } = ">>";

        [JsonProperty("linkSelectedCssClass")]
        public string LinkSelectedCssClass { get; set; } = "Selected";

        [JsonProperty("nextLinkCssClass")]
        public string NextLinkCssClass { get; set; } = "Next";

        [JsonProperty("nextLinkText")]
        public string NextLinkText { get; set; } = ">";

        [JsonProperty("pages")]
        public List<GridPage> Pages { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("previousLinkCssClass")]
        public string PreviousLinkCssClass { get; set; } = "Previous";

        [JsonProperty("previousLinkText")]
        public string PreviousLinkText { get; set; } = "<";

        [JsonProperty("startPage")]
        public int StartPage { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }
    }
}
