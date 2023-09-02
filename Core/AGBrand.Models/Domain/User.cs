using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AGBrand.Models.Domain
{
    public class User : BaseEntity
    {
        public User()
        {
            RefreshTokens = new List<RefreshToken>();
        }

        public string Email { get; set; }

        [Key]
        public Guid Id { get; set; }

        public bool IsEmailVerified { get; set; }
        public bool IsMobileVerified { get; set; }
        public string Mobile { get; set; }

        public string PasswordHash { get; set; }
        public virtual List<RefreshToken> RefreshTokens { get; set; }
    }
}
