using System;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Domain
{
    public class ProductVariantValue
    {
        [Key]
        public Guid Id { get; set; }

        public int ProductId { get; set; }

        public long VariantValueId { get; set; }

        public virtual Product Product { get; set; }
        public virtual VariantValue VariantValue { get; set; }
    }
}
