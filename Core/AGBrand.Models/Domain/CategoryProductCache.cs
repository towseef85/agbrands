namespace AGBrand.Models.Domain
{
    public class CategoryProductCache
    {
        public int CategoryId { get; set; }
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }
        public virtual Category Category { get; set; }
    }
}
