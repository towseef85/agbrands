using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Domain
{
    public class ProductSpec
    {
        public ProductSpec()
        {
            ProductSpecAttributes = new List<ProductSpecAttribute>();
        }

        [Key]
        public Guid Id { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        [Range(0, int.MaxValue)]
        public int Order { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }
        public virtual List<ProductSpecAttribute> ProductSpecAttributes { get; set; }
    }
}
