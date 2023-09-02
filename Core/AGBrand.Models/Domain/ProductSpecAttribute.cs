using System;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Domain
{
    public class ProductSpecAttribute
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ProductSpecId { get; set; }

        [MaxLength(100)]
        public string Key { get; set; }

        [MaxLength(1000)]
        public string Value { get; set; }

        public virtual ProductSpec ProductSpec { get; set; }
    }
}
