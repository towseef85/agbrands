using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace AGBrand.Packages.Models.Configs.Services
{
    public class AuthPolicy
    {
        public AuthorizationPolicy AuthorizationPolicy { get; set; }
        public string Claim { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Values { get; set; }
    }
}
