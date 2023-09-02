using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGBrand.Models.Domain
{
    public class Variant
    {
        public Variant()
        {
            VariantValues = new List<VariantValue>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        public virtual List<VariantValue> VariantValues { get; set; }
    }
}
