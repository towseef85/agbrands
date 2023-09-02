using AGBrand.Packages.Util;

namespace AGBrand.Packages.Helpers.JWT
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using AGBrand.Packages.Contracts.JWT;

    public sealed class JwtTokenBuilder
    {
        private readonly IJwtSecurityKey _jwtSecurityKey;
        private Dictionary<string, object> _claims = new Dictionary<string, object>();

        private double _expiryInMinutes;

        private string audience;

        private DateTime expiryTime;

        private string issuer;

        private string subject;

        public JwtTokenBuilder(IConfiguration configuration,
            IJwtSecurityKey jwtSecurityKey)
        {
            issuer = configuration["Token:ValidIssuer"];
            audience = configuration["Token:ValidAudience"];
            _expiryInMinutes = int.Parse(configuration["Token:ExpiryInMinutes"], CultureInfo.InvariantCulture);
            _jwtSecurityKey = jwtSecurityKey;
        }

        public JwtTokenBuilder AddAudience(string audience)
        {
            this.audience = audience;
            return this;
        }

        public JwtTokenBuilder AddClaim(string type, object value)
        {
            _claims.Add(type, value);
            return this;
        }

        public JwtTokenBuilder AddClaims(Dictionary<string, object> claims)
        {
            _claims = _claims.MergeLeft(claims);
            return this;
        }

        public JwtTokenBuilder AddExpiryByMinutes(double expiryInMinutes)
        {
            _expiryInMinutes = expiryInMinutes;
            return this;
        }

        public JwtTokenBuilder AddExpiryByTime(DateTime expiryTime)
        {
            this.expiryTime = expiryTime;
            return this;
        }

        public JwtTokenBuilder AddIssuer(string issuer)
        {
            this.issuer = issuer;
            return this;
        }

        public JwtTokenBuilder AddSubject(string subject)
        {
            this.subject = subject;
            return this;
        }

        public JwtToken Build(bool byMinutes = true)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }.Union(_claims.Select(item => new Claim(item.Key, item.Value == null ? string.Empty : item.Value.ToString())));

            var token = new JwtSecurityToken(
                              issuer: issuer,
                              audience: audience,
                              claims: claims,
                              expires: byMinutes ? DateTime.UtcNow.AddMinutes(_expiryInMinutes) : expiryTime,
                              signingCredentials: _jwtSecurityKey.SigningCredential);

            return new JwtToken(token);
        }
    }
}
