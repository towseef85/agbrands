using Newtonsoft.Json;
using System.Collections.Generic;

namespace AGBrand.Packages.Models
{
    public sealed class GridModel<T>
    {
        /// <summary>
        /// Total records In The Database
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// The actual list of records returned to the view for rendering
        /// </summary>
        [JsonProperty("data")]
        public List<T> Data { get; set; }

        /// <summary>
        /// The pager data
        /// </summary>
        [JsonProperty("pager")]
        public GridPager Pager { get; set; }

        /// <summary>
        /// The ASC or DESC sort directions
        /// </summary>
        [JsonProperty("sortDirection")]
        public SortDirection SortDirection { get; set; }

        /// <summary>
        /// The column to sort
        /// </summary>
        [JsonProperty("sortKey")]
        public string SortKey { get; set; }
    }
}
