using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGBrand.Models.Domain
{
    public class Category : BaseEntity
    {
        public Category()
        {
            ChildCategories = new List<Category>();
            Products = new List<Product>();
            AssociatedProductsCache = new List<CategoryProductCache>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public int? ParentCategoryId { get; set; }

        public virtual Category ParentCategory { get; set; }

        public virtual List<Category> ChildCategories { get; set; }

        public virtual List<CategoryProductCache> AssociatedProductsCache { get; set; }
        public virtual List<Product> Products { get; set; }
    }
}
