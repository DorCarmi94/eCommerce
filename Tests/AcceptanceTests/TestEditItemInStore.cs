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
using Tests.AuthTests;

namespace Tests.AcceptanceTests
{
    /// <summary>
    /// <UC>
    /// Update product stock- add items
    /// Update product stock- subtract items
    /// Update existing product's details
    /// </UC>
    /// <Req>
    /// 4.1
    /// </Req>
    /// </summary>
    [TestFixture]
    [Order(9)]
    public class TestEditItemInStore
    {
        private IAuthService _auth;
        private INStoreService _inStore;
        private string storeName = "Yossi's Store9";

        [SetUpAttribute]
        public async Task SetUp()
        {
            ISystemService systemService = new SystemService();
            Result<Services> initRes = systemService.GetInstanceForTests(Path.GetFullPath("..\\..\\..\\testsConfig.json"));
            Assert.True(initRes.IsSuccess, "Error at test config file");
            Services services = initRes.Value;

            _auth = services.AuthService;
            _inStore = services.InStoreService;
            
            MemberInfo yossi = new MemberInfo("Yossi119", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("singerMermaid9", "shiran@gmail.com", "Shiran Moris",
                DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            string token = _auth.Connect();
            await _auth.Register(token, yossi, "qwerty123");
            await _auth.Register(token, shiran, "130452abc");
            Result<string> yossiLogInResult = await _auth.Login(token, "Yossi119", "qwerty123", ServiceUserRole.Member);
            IItem product = new SItem("Tara choclate milk", storeName, 10, "dairy",
                new List<string>{"dairy", "milk", "Tara"}, (double)5.4);
            _inStore.OpenStore(yossiLogInResult.Value, storeName);
            _inStore.AddNewItemToStore(yossiLogInResult.Value, product);
            _inStore.AddNewItemToStore(yossiLogInResult.Value, new SItem("iPhone X", storeName, 35, "smartphones", 
                new List<string>{"smartphone", "iPhone", "Apple", "Iphone X"}, (double) 5000.99));
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TearDownAttribute]
        public void Teardown()
        {
            _auth = null;
            _inStore = null;
        }
        
        [TestCase("Tara choclate milk", 15, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)] //edit amount (add)
        [TestCase("Tara choclate milk", 5, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)] //edit amount (subtract)
        [TestCase("Tara choclate milk", 0, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)] //edit amount (remove)
        [TestCase("Tara choclate milk", 10, "Tara",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)] //edit category
        [TestCase("Tara choclate milk", 10, "dairy",
            new string[]{"milk", "Tara"}, (double)5.4)] //edit keywords
        [TestCase("Tara choclate milk", 10, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)6.2)] // edit price
        [Order(0)]
        [Test]
        public async Task TestEditItemInStoreSuccess(string name, int amount, string category, string[] tags,
            double price)  
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = await _auth.Login(token, "Yossi119", "qwerty123", ServiceUserRole.Member);
            Result editItemResult = _inStore.EditItemInStore(yossiLogin.Value,
                new SItem(name, storeName, amount, category, new List<string>(tags), price));
            Assert.True(editItemResult.IsSuccess, "failed to edit item: " + editItemResult.Error);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("Tara choclate milk", "Yossi's Store9", -23, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)] //edit amount (invalid subtract)
        [TestCase("Tara choclate milk", "Yossi's Store9", 10, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)-6.2)] // edit invalid price
        [TestCase("Gans 356 air Rubik's cube", "Yossi's Store9", 178, "games",
            new string[] {"games", "Rubik's cube", "Gans","356 air"}, (double) 114.75)] // edit a non-existing item
        [TestCase("Tara choclate milk", "prancing dragon", 10, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)] //edit fail, can't change the store (store doesn't exist)
        [Test] 
        public async Task TestEditItemInStoreFailureInvalid(string name, string store, int amount, string category, string[] tags,
            double price)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = await _auth.Login(token, "Yossi119", "qwerty123", ServiceUserRole.Member);
            Result editItemResult = _inStore.EditItemInStore(yossiLogin.Value,
                new SItem(name, store, amount, category, new List<string>(tags), price));
            Assert.True(editItemResult.IsFailure, "was suppose to fail to edit item");
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("Tara choclate milk", 15, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)]
        [Test] 
        public async Task TestEditItemInStoreFailureNotOwner(string name, int amount, string category, string[] tags,
            double price)
        {
            string token = _auth.Connect();
            Result<string> login = await _auth.Login(token, "singerMermaid9", "130452abc", ServiceUserRole.Member);
            Result editItemResult = _inStore.EditItemInStore(login.Value,
                new SItem(name, storeName, amount, category, new List<string>(tags), price));
            Assert.True(editItemResult.IsFailure, "was suppose to fail to edit item, user doesn't own the store");
            token = _auth.Logout(login.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("Tara choclate milk", 10, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)6.2)]
        [Test]
        public void TestEditItemInStoreFailureNotLoggedIn(string name, int amount, string category, string[] tags,
            double price)  
        {
            string token = _auth.Connect();
            Result editItemResult = _inStore.EditItemInStore(token,
                new SItem(name, storeName, amount, category, new List<string>(tags), price));
            Assert.True(editItemResult.IsFailure, "was suppose to fail. user is not logged in");
            _auth.Disconnect(token);
        }
    }
}