using System;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Domain
{
    public class Action
    {
        public DateTime ExpiresOn { get; set; }

        [Key]
        public Guid Id { get; set; }

        public string SignInId { get; set; }
    }
}
