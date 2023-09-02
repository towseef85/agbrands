namespace AGBrand.Packages.Helpers.JWT
{
    using System;
    using System.IdentityModel.Tokens.Jwt;

    public sealed class JwtToken
    {
        private readonly JwtSecurityToken _token;

        internal JwtToken(JwtSecurityToken token)
        {
            _token = token;
        }

        public DateTime ValidTo => _token.ValidTo;

        public string Value => new JwtSecurityTokenHandler().WriteToken(_token);
    }
}
