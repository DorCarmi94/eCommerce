using System;
using System.Globalization;
using System.IO;
using eCommerce.Auth;
using System.Threading.Tasks;
using eCommerce.Business;
using eCommerce.Business.Repositories;
using eCommerce.Common;
using eCommerce.Service;
using NUnit.Framework;
using Tests.AuthTests;

namespace Tests.AcceptanceTests
{
    /// <summary>
    /// <UC>
    /// Logout
    /// </UC>
    /// <Req>
    /// 3.1
    /// </Req>
    /// </summary>
    
    [TestFixture]
    public class TestLogout
    {
        private IAuthService _auth;

        [SetUpAttribute]
        public async Task SetUp()
        {
            ISystemService systemService = new SystemService();
            Result<Services> initRes = systemService.GetInstanceForTests(Path.GetFullPath("..\\..\\..\\testsConfig.json"));
            Assert.True(initRes.IsSuccess, "Error at test config file");
            Services services = initRes.Value;

            _auth = services.AuthService;
            
            MemberInfo yossi = new MemberInfo("Yossi250","yossi@gmail.com", "Yossi Park", DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("happyFrog","shiran@gmail.com", "Shiran Moris", DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            string token = _auth.Connect();
            await _auth.Register(token, yossi, "qwerty123");
            await _auth.Register(token, shiran, "130452abc");
            _auth.Disconnect(token);
        }

        [TearDownAttribute]
        public void TearDown()
        {
            _auth = null;
        }
        
        [Test]
        [Order(12)]
        public async Task TestLogoutSuccess()
        {
            string token = _auth.Connect();
            Result<string> result = await _auth.Login(token, "Yossi250", "qwerty123", ServiceUserRole.Member);
            result = _auth.Logout(result.Value);
            Assert.True(result.IsSuccess, result.Error);
            token = result.Value;
            result = await _auth.Login(token, "happyFrog", "130452abc", ServiceUserRole.Member);
            result = _auth.Logout(result.Value);
            Assert.True(result.IsSuccess, result.Error);
            _auth.Disconnect(result.Value);
        }
        
        [Test]
        public async Task TestLogoutFailure()
        {
            string token = _auth.Connect();
            Result<string> result = await _auth.Login(token, "Yossi250", "qwerty123", ServiceUserRole.Member);
            Result<string> falseResult = _auth.Logout(token);
            Assert.True(falseResult.IsFailure, "Logout was suppose to fail");
            _auth.Disconnect(result.Value);
        }
    }
}