using System.Collections.Generic;

namespace AGBrand.Packages.Models
{
    public sealed class NotificationSms
    {
        public string Content { get; set; }

        public List<string> Tos { get; set; }
    }
}
