using System;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Domain
{
    public class RefreshToken
    {
        public string ClientInfo { get; set; }

        public string ClientIp { get; set; }

        public DateTime ExpiresOn { get; set; }

        [Key]
        public Guid Id { get; set; }

        public string Token { get; set; }
        public virtual User User { get; set; }
        public Guid UserId { get; set; }
    }
}
