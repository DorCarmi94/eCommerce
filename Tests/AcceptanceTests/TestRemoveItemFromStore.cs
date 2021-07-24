using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Remove product from store
    /// </UC>
    /// <Req>
    /// 4.1
    /// </Req>
    /// </summary>
    [TestFixture]
    [Order(14)]
    public class TestRemoveItemFromStore
    {
        private IAuthService _auth;
        private INStoreService _inStore;
        private string storeName = "Target";

        public TestRemoveItemFromStore()
        {
            
        }
        
        [SetUpAttribute]
        public async Task SetUp()
        {
            ISystemService systemService = new SystemService();
            Result<Services> initRes = systemService.GetInstanceForTests(Path.GetFullPath("..\\..\\..\\testsConfig.json"));
            Assert.True(initRes.IsSuccess, "Error at test config file");
            Services services = initRes.Value;

            _auth = services.AuthService;
            _inStore = services.InStoreService;
            
            MemberInfo yossi = new MemberInfo("Mechanism1000", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("PhrogLiv", "shiran@gmail.com", "Shiran Moris",
                DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            string token = _auth.Connect();
            var resReg=_auth.Register(token, yossi, "qwerty123");
            var regiRes=resReg.Result;
            var regRes2=_auth.Register(token, shiran, "130452abc");
            var regiRes2 = regRes2.Result;
            var yossiLogInTask = _auth.Login(token, "Mechanism1000", "qwerty123", ServiceUserRole.Member);
            var yossiLoginRes = yossiLogInTask.Result;
            IItem product = new SItem("Tara milk", storeName, 10, "dairy",
                new List<string>{"dairy", "milk", "Tara"}, (double)5.4);
            _inStore.OpenStore(yossiLoginRes.Value, storeName);
            _inStore.AddNewItemToStore(yossiLoginRes.Value, product);
            _inStore.AddNewItemToStore(yossiLoginRes.Value, new SItem("iPhone X", storeName, 35, "smartphones", 
                new List<string>{"smartphone", "iPhone", "Apple", "Iphone X"}, (double) 5000.99));
            token = _auth.Logout(yossiLoginRes.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TearDownAttribute]
        public void Teardown()
        {
            _auth = null;
            _inStore = null;
        }
        
        [TestCase("Tara milk")]
        [TestCase("iPhone X")]
        [Order(0)]
        [Test]
        public async Task TestRemoveItemFromStoreSuccess(string productName)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = await _auth.Login(token, "Mechanism1000", "qwerty123", ServiceUserRole.Member);
            Result removeItemResult = _inStore.RemoveItemFromStore(yossiLogin.Value, storeName, productName);
            Assert.True(removeItemResult.IsSuccess, "failed to remove item " + productName + ": " + removeItemResult.Error);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token); 
        }
        
        [TestCase("Target", "Gans 356 air", "Mechanism1000", "qwerty123")]
        [TestCase("dancing doors", "Tara milk", "Mechanism1000", "qwerty123")] // non existing store
        [TestCase("Target", "Gans 356 air", "PhrogLiv", "130452abc")]
        [Test]      
        public async Task TestRemoveItemFromStoreFailureInvalid(string store, string productName, string member, string password)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = await _auth.Login(token, member, password, ServiceUserRole.Member);
            Result removeItemResult = _inStore.RemoveItemFromStore(yossiLogin.Value, store, productName);
            Assert.True(removeItemResult.IsFailure);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token); 
        }

       
        
        [TestCase("Tara milk")]
        [TestCase("iPhone X")]
        [Test]
        public void TestRemoveItemFromStoreFailureLogic(string productName)
        {
            string token = _auth.Connect();
            Result removeItemResult = _inStore.RemoveItemFromStore(token, storeName, productName);
            Assert.True(removeItemResult.IsFailure, "Suppose to fail, user not logged in");
            _auth.Disconnect(token); 
        }
    }
}