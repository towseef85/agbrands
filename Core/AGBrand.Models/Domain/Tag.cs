namespace AGBrand.Models.Domain
{
    public class Tag
    {
        public string Id { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
