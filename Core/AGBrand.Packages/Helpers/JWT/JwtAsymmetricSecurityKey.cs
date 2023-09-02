namespace AGBrand.Packages.Helpers.JWT
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Security.Cryptography;
    using AGBrand.Packages.Contracts.JWT;
    using AGBrand.Packages.Models.JWT;

    public class JwtAsymmetricSecurityKey : IJwtSecurityKey
    {
        public JwtAsymmetricSecurityKey(IConfiguration configuration)
        {
            var rsaConfig = configuration.GetSection("Token:RSAConfig").Get<RsaConfig>();
            var keyId = configuration["Token:KeyId"];

            RSAParameters rsaParams = GetRSAParamsFromConfig(rsaConfig);

            Create(rsaParams, keyId);
        }

        public SecurityKey PrivateKey { get; private set; }

        public SecurityKey PublicKey { get; private set; }

        public RSAParameters RsaParameters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public SigningCredentials SigningCredential { get; private set; }
        public string X5C { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string X5T { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private void Create(RSAParameters rsaParams, string keyId)
        {
            var rsaProvider = new RSACryptoServiceProvider(2048);

            rsaProvider.ImportParameters(rsaParams);

            PublicKey = new RsaSecurityKey(rsaProvider.ExportParameters(false));

            var privateKey = new RsaSecurityKey(rsaProvider)
            {
                KeyId = keyId
            };

            PrivateKey = privateKey;

            SigningCredential = new SigningCredentials(PrivateKey, SecurityAlgorithms.RsaSha256, SecurityAlgorithms.Sha256);
        }

        private RSAParameters GetRSAParamsFromConfig(RsaConfig rsaConfig)
        {
            return new RSAParameters
            {
                Modulus = rsaConfig.Modulus != null ? Convert.FromBase64String(rsaConfig.Modulus) : null,
                Exponent = rsaConfig.Exponent != null ? Convert.FromBase64String(rsaConfig.Exponent) : null,
                P = rsaConfig.P != null ? Convert.FromBase64String(rsaConfig.P) : null,
                Q = rsaConfig.Q != null ? Convert.FromBase64String(rsaConfig.Q) : null,
                DP = rsaConfig.DP != null ? Convert.FromBase64String(rsaConfig.DP) : null,
                DQ = rsaConfig.DQ != null ? Convert.FromBase64String(rsaConfig.DQ) : null,
                InverseQ = rsaConfig.InverseQ != null ? Convert.FromBase64String(rsaConfig.InverseQ) : null,
                D = rsaConfig.D != null ? Convert.FromBase64String(rsaConfig.D) : null
            };
        }
    }
}
