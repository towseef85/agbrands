using System;

namespace AGBrand.Models.Domain
{
    public class BaseEntity
    {
        public DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
