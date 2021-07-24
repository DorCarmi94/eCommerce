using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using eCommerce.Auth;
using NUnit.Framework;

namespace Tests.AuthTests
{
    [TestFixture]
    public class JwtTests
    {
        private JWTAuth _jwt;
        
        public JwtTests()
        {
            _jwt = new JWTAuth("easyKeyeasyKeyeasyKeyeasyKey");
        }

        [Test]
        public void TokenGenerationValidClaimsTest()
        {
            Claim[] claims = new Claim[]
            {
                new Claim("Username", "user1"),
                new Claim("Role", "Member")
            };

            string token = _jwt.GenerateToken(claims);
            
            Assert.True(_jwt.ValidateToken(token));
        }
        
        [Test]
        public void TokenGenerationInvalidClaimsTest()
        {
            Assert.AreEqual(null, 
                        _jwt.GenerateToken(null),
                        "JWT Auth accepts null Claim array");
            
            Assert.AreEqual(null, 
                _jwt.GenerateToken(new Claim[0]),
                "JWT Auth accepts null Claim array");
        }
        
        [Test]
        public void GetClaimsFromTokenValidTokensTest()
        {
            Claim[] claims = new Claim[]
            {
                new Claim("Username", "user1"),
                new Claim("Role", "Member")
            };
            
            string token = _jwt.GenerateToken(claims);
            Claim[] tokenClaims = _jwt.GetClaimsFromToken(token).ToArray();

            bool equalClaims = ContainsClaims(claims, tokenClaims);
            Assert.True(equalClaims,
                        "The extracted claims from the token is not the same from what it was built from");
        }

        [Test]
        public void GetClaimsFromTokenInvalidTokensTest()
        {
            Assert.AreEqual(null,
                _jwt.GetClaimsFromToken(null), 
                "Receive null token");
            
            Assert.AreEqual(null,
                _jwt.GetClaimsFromToken("Invalid token"), 
                "Receive invalid token");
        }
        
        private bool ContainsClaims(Claim[] soruce, Claim[] dest)
        {
            bool equalClaims = true;
            foreach (var claim in soruce)
            {
                bool foundEqual = false;
                foreach (var tokenClaim in dest)
                {
                    if (tokenClaim.ValueType.Equals(claim.ValueType) &&
                        tokenClaim.Value.Equals(claim.Value))
                    {
                        foundEqual = true;
                        break;
                    }
                }

                if (!foundEqual)
                {
                    equalClaims = false;
                    break;
                }
            }

            return equalClaims;
        }
    }
}