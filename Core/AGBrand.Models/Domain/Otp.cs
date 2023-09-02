using System;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Domain
{
    public class Otp
    {
        public string Code { get; set; }

        public DateTime ExpiresOn { get; set; }

        [Key]
        public Guid Id { get; set; }

        public string SignInId { get; set; }
    }
}
