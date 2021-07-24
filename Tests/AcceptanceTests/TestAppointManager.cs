using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Appoint manager
    /// </UC>
    /// <Req>
    /// 4.5
    /// </Req>
    /// </summary>
    [TestFixture]
    [Order(4)]
    public class TestAppointManager
    {
        private IAuthService _auth;
        private INStoreService _inStore;
        private IUserService _user;
        private string store = "Yossi's Store";


        [SetUpAttribute]
        public async Task SetUp()
        {
            ISystemService systemService = new SystemService();
            Result<Services> initRes = systemService.GetInstanceForTests(Path.GetFullPath("..\\..\\..\\testsConfig.json"));
            Assert.True(initRes.IsSuccess, "Error at test config file");
            Services services = initRes.Value;

            _auth = services.AuthService;
            _inStore = services.InStoreService;
            _user = services.UserService;
            
            MemberInfo yossi = new MemberInfo("Yossi11", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("singerMermaid", "shiran@gmail.com", "Shiran Moris",
                DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            MemberInfo lior = new MemberInfo("Liorwork","lior@gmail.com", "Lior Lee", 
                DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
            string token = _auth.Connect();
            await _auth.Register(token, yossi, "qwerty123");
            await _auth.Register(token, shiran, "130452abc");
            await _auth.Register(token, lior, "987654321");
            Result<string> yossiLogInResult = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            _inStore.OpenStore(yossiLogInResult.Value, store);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        [TearDownAttribute]
        public void Teardown()
        {
            _auth = null;
            _inStore = null;
            _user = null;
        }
        
        
        [TestCase("Yossi's Store", "singerMermaid")]
        [TestCase("Yossi's Store", "Liorwork")]
        [Order(1)]
        [Test]
        public async Task TestAppointManagerSuccess(string storeName, string username)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result result = _user.AppointManager(yossiLogin.Value, storeName, username);
            Assert.True(result.IsSuccess, "failed to appoint " + username + ": " + result.Error);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("Yossi's Store", "singerMermaid")]
        [TestCase("Yossi's Store", "Liorwork")]
        [Test]
        public async Task TestAppointManagerFailureDouble(string storeName, string username)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            _user.AppointManager(yossiLogin.Value, storeName, username);
            Result result = _user.AppointManager(yossiLogin.Value, storeName, username);
            Assert.True(result.IsFailure, "Appointing manager was suppose to fail, manager already appointed");
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }

        [TestCase("Yossi11", "qwerty123", "The Polite Frog", "singerMermaid")]
        [TestCase("Yossi11",  "qwerty123", "Yossi's Store", "Yossi11")] 
        [TestCase("Yossi11",   "qwerty123", "Yossi's Store", "Tamir123")]
        [TestCase("singerMermaid", "130452abc", "Yossi's Store", "Liorwork")]
        [Test]
        public async Task TestAppointManagerFailureInvalid(string appointer, string appointerPassword,  string storeName, string username)
        {
            string token = _auth.Connect();
            Result<string>login = await _auth.Login(token, appointer, appointerPassword, ServiceUserRole.Member);
            Result result = _user.AppointManager(login.Value, storeName, username);
            Assert.True(result.IsFailure, "Appointing " + username + " was expected to fail!");
            token = _auth.Logout(login.Value).Value;
            _auth.Disconnect(token);
        }
        
        
         [TestCase("Yossi's Store", "singerMermaid")]
         [TestCase("Yossi's Store", "Liorwork")]
         [Test]
         public void TestAppointManagerFailureLogic(string storeName, string username)
         {
             string token = _auth.Connect();
             Result result = _user.AppointManager(token, storeName, username);
             Assert.True(result.IsFailure, "Appointing " + username + " was expected to fail since the user wasn't logged in!");
             _auth.Disconnect(token);
         }


         [Test]
         public void TestAppointManagerConcurrent()
         {
             MemberInfo menash = new MemberInfo("Menash","lior@gmail.com", "Lior Lee", 
                 DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
             
             MemberInfo shimon = new MemberInfo("shimon","shimon@gmail.com", "shimon Leshimone", 
                 DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
             
             MemberInfo yosef = new MemberInfo("yosef","yosef@gmail.com", "yosef yosef", 
                 DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");

             
             
             var tokenMenash = _auth.Connect();
             var tokenShimon = _auth.Connect();
             var tokenyosef = _auth.Connect();
             
             var registerMenash=_auth.Register(tokenMenash, menash, "qwerty123");
             var registerShimon= _auth.Register(tokenShimon, shimon, "qwerty123");
             var registerYosef= _auth.Register(tokenyosef, yosef, "qwerty123");
             
             Assert.True(registerMenash.Result.IsSuccess,registerMenash.Result.Error);
             Assert.True(registerShimon.Result.IsSuccess,registerShimon.Result.Error);
             Assert.True(registerYosef.Result.IsSuccess,registerYosef.Result.Error);

             var toeknYossi = _auth.Connect();
             Result<string> yossiLogin = _auth.Login(toeknYossi, "Yossi11", "qwerty123", ServiceUserRole.Member).Result;
             Result<string> menashLogin =
                 _auth.Login(tokenMenash, menash.Username, "qwerty123", ServiceUserRole.Member).Result;
             Result<string> shimonLogin =
                 _auth.Login(tokenShimon, shimon.Username, "qwerty123", ServiceUserRole.Member).Result;
             Result<string> yosefLogin =
                 _auth.Login(tokenyosef, yosef.Username, "qwerty123", ServiceUserRole.Member).Result;
             
             Assert.True(yossiLogin.IsSuccess,yossiLogin.Error);
             Assert.True(menashLogin.IsSuccess,menashLogin.Error);
             Assert.True(shimonLogin.IsSuccess,shimonLogin.Error);
             Assert.True(yosefLogin.IsSuccess,yosefLogin.Error);


             var appointMenash=_user.AppointCoOwner(yossiLogin.Value, store, menash.Username);
             var appointShimon=_user.AppointCoOwner(yossiLogin.Value, store, shimon.Username);
             
             Assert.True(appointMenash.IsSuccess,appointMenash.Error);
             Assert.True(appointShimon.IsSuccess,appointShimon.Error);
             
             Task<Result> task1 = new Task<Result>(() => { return _user.AppointManager(menashLogin.Value,store,yosef.Username); });
             Task<Result> task2 = new Task<Result>(() => { return _user.AppointManager(shimonLogin.Value,store,yosef.Username); });
             task1.Start();
             task2.Start();

             var task1res = task1.Result;
             var task2res = task2.Result;

             var both = task1res.IsSuccess && task2res.IsSuccess;

             var oneOfThem = task1res.IsSuccess || task2res.IsSuccess;
             
             Assert.True(!both && oneOfThem);


         }
    }
}