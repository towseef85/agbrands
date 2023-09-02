using System;

namespace AGBrand.Packages.Attributes
{
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
    public class EnumFilterAttribute : Attribute
    {
        public EnumFilterAttribute(string filter)
        {
            Filter = filter;
        }

        public string Filter { get; set; }
    }
}
