using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGBrand.Models.Domain
{
    public class VariantValue
    {
        public VariantValue()
        {
            ProductVariantValues = new List<ProductVariantValue>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [MaxLength(50)]
        public string Value { get; set; }

        public virtual Variant Variant { get; set; }

        public virtual List<ProductVariantValue> ProductVariantValues { get; set; }
    }
}