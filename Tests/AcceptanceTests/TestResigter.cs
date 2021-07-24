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


namespace Tests.AcceptanceTests
{
    /// <summary>
    /// <UC>
    /// Register to system
    /// </UC>
    /// <Req>
    /// 2.3
    /// </Req>
    /// </summary>
    [TestFixture]
    [Order(15)]
    public class TestRegister
    {
        private IAuthService _auth;

        [SetUpAttribute]
        public void SetUp()
        {
            ISystemService systemService = new SystemService();
            Result<Services> initRes = systemService.GetInstanceForTests(Path.GetFullPath("..\\..\\..\\testsConfig.json"));
            Assert.True(initRes.IsSuccess, "Error at test config file");
            Services services = initRes.Value;

            _auth = services.AuthService;
        }
        
        [TearDownAttribute]
        public void Teardown()
        {
            _auth = null;
        }
        
        
        [TestCase("Tamir123","tamir@gmail.com", "Tamir", "23/03/1961", "gold st. 14", "tdddev123")]
        [TestCase("Nathan43","nat4343@gmail.com", "Nathan dor", "17/07/1997", "main st. 57", "NathanFTW765")]
        [Order(0)]
        [Test]
        public async Task TestRegisterSuccess(string username, string email, string name, string birthday, string address, string password)
        {
            MemberInfo info = new MemberInfo(username, email, name, DateTime.ParseExact(birthday, "dd/MM/yyyy", CultureInfo.InvariantCulture), address);
            string token = _auth.Connect();
            Result regResult = await _auth.Register(token, info, password);
            Assert.IsTrue(regResult.IsSuccess, regResult.Error);
            _auth.Disconnect(token);
        }
        [TestCase("~~Nathan43~~","nat4343@gmail.com", "Nathan dor", "17/07/1997", "main st. 57", "NathanFTW765")]
        [TestCase("Nathan43","nat4343@gmail", "Nathan dor", "17/07/1997", "main st. 57", "NathanFTW765")]
        //[TestCase("Nathan43","nat4343@gmail.com", "Nathan dor12", "17/07/1997", "main st. 57", "NathanFTW765")]
        //[TestCase("Nathan43","nat4343@gmail.com", "Nathan dor", "17/07/1997", "main st. 57", "Nath")]
        //[TestCase("Nathan43","nat4343@gmail.com", "Nathan dor", "17/07/1997", "main st. 57", "NathanFTW12345678")]
        [Test]
        public async Task TestRegisterFailureInput(string username, string email, string name, string birthday, string address, string password)
        {
            MemberInfo info = new MemberInfo(username, email, name, DateTime.ParseExact(birthday, "dd/MM/yyyy", CultureInfo.InvariantCulture), address);
            string token = _auth.Connect();
            Result regResult = await _auth.Register(token, info, password);
            Assert.IsTrue(regResult.IsFailure, "test case was suppose to fail");
            _auth.Disconnect(token);
        }
        
        [TestCase("Yossi11","yossi@gmail.com", "Yossi Park", "19/04/2005", "hazait 14", "tdddev123")]
        [Test]
        public async Task TestRegisterFailureLogic(string username, string email, string name, string birthday, string address, string password)
        {
            MemberInfo info = new MemberInfo(username, email, name, DateTime.ParseExact(birthday, "dd/MM/yyyy", CultureInfo.InvariantCulture), address);
            string token = _auth.Connect();
            await _auth.Register(token, info, password);   //registers the user once.
            Result regResult = await _auth.Register(token, info, password); //user is already registered 
            Assert.IsTrue(regResult.IsFailure, "test case was suppose to fail. User already registered");
            _auth.Disconnect(token);
        }
    }
}