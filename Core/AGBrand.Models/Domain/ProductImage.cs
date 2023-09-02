using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AGBrand.Models.Domain
{
    public class ProductImage
    {
        [Key]
        public Guid Id { get; set; }

        public int ProductId { get; set; }

        public string ImageUrl { get; set; }

        public bool IsMainImage { get; set; }

        public virtual Product Product { get; set; }
    }
}
