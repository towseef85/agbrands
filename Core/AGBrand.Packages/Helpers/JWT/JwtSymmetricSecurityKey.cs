namespace AGBrand.Packages.Helpers.JWT
{
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using AGBrand.Packages.Contracts.JWT;

    public class JwtSymmetricSecurityKey : IJwtSecurityKey
    {
        public JwtSymmetricSecurityKey(string secret, string keyId)
        {
            var privateKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret))
            {
                KeyId = keyId
            };

            PrivateKey = privateKey;
            PublicKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));

            SigningCredential = new SigningCredentials(PrivateKey, SecurityAlgorithms.HmacSha256);
        }

        public SecurityKey PrivateKey { get; }

        public SecurityKey PublicKey { get; }
        public RSAParameters RsaParameters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public SigningCredentials SigningCredential { get; }
        public string X5C { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string X5T { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
