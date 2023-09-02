using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGBrand.Models.Domain
{
    public class Brand : BaseEntity
    {
        public Brand()
        {
            Products = new List<Product>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public string ImageUrl { get; set; }

       //// public byte[] Content { get; set; }
        public virtual List<Product> Products { get; set; }
    }
}
