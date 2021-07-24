using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
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
    /// Add new product to store
    /// </UC>
    /// <Req>
    /// 4.1
    /// </Req>
    /// </summary>
    
    [TestFixture]
    [Order(1)]
    public class TestAddNewItemToStore
    {
        private IAuthService _auth;
        private INStoreService _inStore;
        private string storeName = "Yossi's Store";
        
        [SetUpAttribute]
        public async Task SetUp()
        {

            ISystemService systemService = new SystemService();
            Result<Services> initRes = systemService.GetInstanceForTests(Path.GetFullPath("..\\..\\..\\testsConfig.json"));
            Assert.True(initRes.IsSuccess, "Error at test config file");
            Services services = initRes.Value;

            _auth = services.AuthService;
            _inStore = services.InStoreService;
            
            
            MemberInfo yossi = new MemberInfo("Yossi11", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("singerMermaid", "shiran@gmail.com", "Shiran Moris",
                DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            string token = _auth.Connect();
            await _auth.Register(token, yossi, "qwerty123");
            await _auth.Register(token, shiran, "130452abc");
            Result<string> yossiLogInResult = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            _inStore.OpenStore(yossiLogInResult.Value, storeName);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }

        [TearDownAttribute]
        public void Teardown()
        {
            _auth = null;
            _inStore = null;
        }

        [TestCase("iPhone X", 35, "smartphones",
            new string[] {"smartphone", "iPhone", "Apple", "Iphone X"}, 5000.99)]
        [TestCase("Gans 356 air Rubik's cube", 178, "games",
            new string[] {"games", "Rubik's cube", "Gans","356 air"}, 114.75)]
        [Test]
        [Order(2)]
        public async Task TestAddNewItemToStoreSuccess(string name, int amount, string category, string[] tags,
            double price)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Assert.True(yossiLogin.IsSuccess,yossiLogin.Error);
            Result addItemResult = _inStore.AddNewItemToStore(yossiLogin.Value,
                new SItem(name, storeName, amount, category, tags.ToList(), price));
            Assert.True(addItemResult.IsSuccess, "failed to add item: " + name + "|error: " + addItemResult.Error);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("iPhone X", "Yossi's Store", -23, "smartphones",
            new string[] {"smartphone", "iPhone", "Apple", "Iphone X"}, (double) 5000.99)]
        [TestCase("Gans 356 air Rubik's cube", "Yossi's Store", 178, "games",
            new string[] {"games", "Rubik's cube", "Gans","356 air"}, (double) -75.9)]
        [TestCase("Cube Alarm", "the dancing pirate", 5986, "electronics",
            new string[] {"alarm", "electronics", "cube","decorations"}, (double) 65.5)]
        [Test]      
        public async Task TestAddNewItemToStoreFailureInput(string name, string store, int amount, string category, string[] tags,
            double price)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result addItemResult = _inStore.AddNewItemToStore(yossiLogin.Value,
                new SItem(name, store, amount, category, tags.ToList(), price));
            Assert.True(addItemResult.IsFailure, "item addition was suppose to fail for " + name);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("iPhone X", 35, "smartphones",
            new string[] {"smartphone", "iPhone", "Apple", "Iphone X"}, 000.99, "dancing dragon")]
        [TestCase("Gans 356 air Rubik's cube", 178, "games",
            new string[] {"games", "Rubik's cube", "Gans","356 air"}, 114.75, "Yossi's store")]
        [Test]      
        public async Task TestAddNewItemToStoreFailureLogic(string name, int amount, string category, string[] tags,
            double price, string store)
        {
            string token = _auth.Connect();
            Result<string> login = await _auth.Login(token, "singerMermaid", "130452abc", ServiceUserRole.Member);
            Result addItemResult = _inStore.AddNewItemToStore(login.Value,
                new SItem(name, store, amount, category, tags.ToList(), price));
            Assert.True(addItemResult.IsFailure, "item addition was suppose to fail for " + name + ", since the user does not own the store.");
            token = _auth.Logout(login.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("Tara milk", "Yossi's Store", 10, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)5.8)]
        [Test]      
        public async Task TestAddNewItemToStoreFailureInputDoubleAddition(string name, string store, int amount, string category, string[] tags,
            double price)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            _inStore.AddNewItemToStore(yossiLogin.Value, new SItem(name, storeName, amount, category, tags.ToList(), price));
            _inStore.AddNewItemToStore(yossiLogin.Value, new SItem(name, storeName, amount, category, new List<string>(tags), price));
            Result addItemResult = _inStore.AddNewItemToStore(yossiLogin.Value,
                new SItem(name, storeName, amount, category, tags.ToList(), price));
            Assert.True(addItemResult.IsFailure, "item addition was suppose to fail for " + name);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("iPhone X", 35, "smartphones",
            new string[] {"smartphone", "iPhone", "Apple", "Iphone X"}, (double) 5000.99)]
        [TestCase("Gans 356 air Rubik's cube", 178, "games",
            new string[] {"games", "Rubik's cube", "Gans","356 air"}, (double) 114.75)]
        [Test]      
        public void TestAddNewItemToStoreFailureLogicNoLogin(string name, int amount, string category, string[] tags,
            double price)
        {
            string token = _auth.Connect();
            Result addItemResult = _inStore.AddNewItemToStore(token,
                new SItem(name, storeName, amount, category, tags.ToList(), price));
            Assert.True(addItemResult.IsFailure, "item addition was suppose to fail for " + name + ", since the user is not logged in.");
            _auth.Disconnect(token);
        }
        
    }
}