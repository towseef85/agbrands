using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace AGBrand.Packages.Contracts.JWT
{
    public interface IJwtSecurityKey
    {
        SecurityKey PrivateKey { get; }
        SecurityKey PublicKey { get; }
        RSAParameters RsaParameters { get; set; }
        SigningCredentials SigningCredential { get; }
        string X5C { get; set; }
        string X5T { get; set; }
    }
}
