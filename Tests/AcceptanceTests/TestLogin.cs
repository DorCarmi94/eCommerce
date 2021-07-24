using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using eCommerce.Auth;
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
    /// Login
    /// </UC>
    /// <Req>
    /// 2.4
    /// </Req>
    /// </summary>
    
    [TestFixture]
    [Order(11)]
    public class TestLogin
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

            MemberInfo yossi = new MemberInfo("Yossi11","yossi@gmail.com", "Yossi Park", DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("singerMermaid","shiran@gmail.com", "Shiran Moris", DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            
            MemberInfo lior = new MemberInfo("Liorwork","lior@gmail.com", "Lior Lee", DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
            string token = _auth.Connect();
            await _auth.Register(token, yossi, "qwerty123");
            await _auth.Register(token, shiran, "130452abc");
            await _auth.Register(token, lior, "987654321");
            _auth.Disconnect(token);
        }
        [TearDownAttribute]
        public void Teardown()
        {
            _auth = null;
        }

        [TestCase("Yossi11", "qwerty123")]
        [TestCase("singerMermaid", "130452abc")]
        [Test]
        public async Task TestLoginSuccess(string username, string password)
        {
            string token = _auth.Connect();
            Result<string> result = await _auth.Login(token, username, password, ServiceUserRole.Member);
            Assert.True(result.IsSuccess, result.Error);
            _auth.Disconnect(token);
        }
        
        [TestCase("Yossi11", "qwerty")]
        [TestCase("_singerMermaid", "130452abc")]
        [TestCase("Tamir123", "130452abc")]
        [Test]
        public async Task TestLoginFailure(string username, string password)
        {
            string token = _auth.Connect();
            Result<string> result = await _auth.Login(token, username, password, ServiceUserRole.Member);
            Assert.True(result.IsFailure, "username: " + username + " | password: " + password + "| was suppose to fail!");
            _auth.Disconnect(token);
        }
    }
}