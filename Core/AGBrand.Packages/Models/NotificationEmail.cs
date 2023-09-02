using System.Collections.Generic;

namespace AGBrand.Packages.Models
{
    public sealed class NotificationEmail
    {
        public List<string> Bccs { get; set; }
        public string Content { get; set; }

        public string From { get; set; }

        public string Name { get; set; }

        public string Subject { get; set; }
        public List<string> Tos { get; set; }
    }
}
