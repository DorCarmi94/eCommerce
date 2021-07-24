using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace eCommerce.Auth
{
    public class JWTAuth
    {
        private readonly SecurityKey _key;

        private readonly string _hashAlgo;
        private readonly int _expressionInH;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly JwtSecurityTokenHandler _securityTokenHandler;

        public JWTAuth(string key)
        {
            _key = new SymmetricSecurityKey(Encoding.Default.GetBytes(key));
            _expressionInH = 240;
            _hashAlgo = SecurityAlgorithms.HmacSha256Signature;

            _tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = _key

            };

            _securityTokenHandler = new JwtSecurityTokenHandler();
        }

        public string GenerateToken(Claim[] claims)
        {
            if (claims == null || claims.Length == 0)
            {
                return null;
            }

            SecurityTokenDescriptor std = new SecurityTokenDescriptor()
            {
                Expires = DateTime.UtcNow.Add(TimeSpan.FromHours(_expressionInH)),
                SigningCredentials = new SigningCredentials(_key, _hashAlgo),
                Subject = new ClaimsIdentity(claims)
            };

            SecurityToken securityToken = _securityTokenHandler.CreateToken(std);
            return _securityTokenHandler.WriteToken(securityToken);
        }

        public IEnumerable<Claim> GetClaimsFromToken(string token)
        {
            SecurityToken securityTokentoken = null;
            try
            {
                var claimsPrincipal =
                    _securityTokenHandler.ValidateToken(token, _tokenValidationParameters, out securityTokentoken);
                return claimsPrincipal.Claims;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool ValidateToken(string token)
        {
            return GetClaimsFromToken(token) != null;
        }
    }
}