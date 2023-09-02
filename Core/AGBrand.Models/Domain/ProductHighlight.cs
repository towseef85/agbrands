using System;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Domain
{
    public class ProductHighlight
    {
        [Key]
        public Guid Id { get; set; }

        public int ProductId { get; set; }

        [MaxLength(50)]
        public string Key { get; set; }

        [MaxLength(200)]
        public string Value { get; set; }

        public virtual Product Product { get; set; }
    }
}
