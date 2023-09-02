using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using AGBrand.Packages.Contracts.JWT;

namespace AGBrand.Packages.Helpers.JWT
{
    public class JwtCertificateSecurityKey : IJwtSecurityKey
    {
        public JwtCertificateSecurityKey(X509Certificate2 x509Certificate2, string keyId)
        {
            Create(x509Certificate2, keyId);
        }

        public SecurityKey PrivateKey { get; private set; }

        public SecurityKey PublicKey { get; private set; }

        public RSAParameters RsaParameters { get; set; }
        public SigningCredentials SigningCredential { get; private set; }
        public string X5C { get; set; }
        public string X5T { get; set; }

        private void Create(X509Certificate2 x509Certificate2, string keyId)
        {
            var key = new X509SecurityKey(x509Certificate2);

            var rsaProvider = (RSA)key.PrivateKey;

            var publicKey = new RsaSecurityKey(rsaProvider.ExportParameters(false));

            PublicKey = publicKey;

            var privateKey = new RsaSecurityKey(rsaProvider)
            {
                KeyId = keyId
            };

            PrivateKey = privateKey;

            SigningCredential = new SigningCredentials(PrivateKey, SecurityAlgorithms.RsaSha256, SecurityAlgorithms.Sha256);

            var chain = new X509Chain();
            chain.Build(x509Certificate2);

            X5C = Convert.ToBase64String(chain.ChainElements[0].Certificate.RawData, 0, chain.ChainElements[0].Certificate.RawData.Length);
            X5T = Base64UrlEncoder.Encode(chain.ChainElements[0].Certificate.Thumbprint);

            RsaParameters = publicKey.Parameters;
        }
    }
}
