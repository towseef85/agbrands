using System.ComponentModel;

namespace AGBrand.Packages.Models
{
    public sealed class OListItem
    {
        public AttributeCollection Attributes { get; set; }
        public string Description { get; set; }
        public bool Disabled { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
        public string Text { get; set; }
        public object Value { get; set; }
    }
}
