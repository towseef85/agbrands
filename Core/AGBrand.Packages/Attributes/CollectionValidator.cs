namespace AGBrand.Packages.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using AGBrand.Packages.Util;

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class CollectionValidatorAttribute : ValidationAttribute
    {
        private readonly string _collection;
        private readonly Type _type;

        public CollectionValidatorAttribute(string collection, Type type)
        {
            _collection = collection;
            _type = type;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var items = _collection.GetIds(_type);

            return items.Contains(value) ? ValidationResult.Success : new ValidationResult($"Value {value} doesn't exist in the collection {_collection}");
        }
    }
}
