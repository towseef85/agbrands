using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGBrand.Models.Domain
{
    public class Product
    {
        public Product()
        {
            ProductSpecs = new List<ProductSpec>();
            ProductHighlights = new List<ProductHighlight>();
            Tags = new List<Tag>();
            ProductVariantValues = new List<ProductVariantValue>();
            AssociatedCategoriesCache = new List<CategoryProductCache>();
            ProductImages = new List<ProductImage>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CategoryId { get; set; }
        public int BrandId { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string ShortDescription { get; set; }

        public string Description { get; set; }

        [Range(0, float.MaxValue)]
        public float Price { get; set; }

        public DiscountType DiscountType { get; set; }

        [Range(0, float.MaxValue)]
        public float DiscountAmount { get; set; }

        [Range(0, 100)]
        public float DiscountPercentage { get; set; }

        [MaxLength(20)]
        public string Sku { get; set; }

        [MaxLength(20)]
        public string ItemCode { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual Category Category { get; set; }
        public virtual List<ProductSpec> ProductSpecs { get; set; }
        public virtual List<ProductHighlight> ProductHighlights { get; set; }
        public virtual List<Tag> Tags { get; set; }
        public virtual List<ProductVariantValue> ProductVariantValues { get; set; }
        public virtual List<ProductImage> ProductImages { get; set; }
        public virtual List<CategoryProductCache> AssociatedCategoriesCache { get; set; }
    }
}
