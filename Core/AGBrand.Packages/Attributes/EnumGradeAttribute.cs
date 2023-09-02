using System;

namespace AGBrand.Packages.Attributes
{
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
    public class EnumGradeAttribute : Attribute
    {
        public EnumGradeAttribute(int grade)
        {
            this.Grade = grade;
        }

        public int Grade { get; set; }
    }
}
