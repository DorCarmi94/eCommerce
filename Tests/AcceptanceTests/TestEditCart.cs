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
    /// Edit shopping cart
    /// </UC>
    /// <Req>
    /// 2.8
    /// </Req>
    /// </summary>
    [TestFixture]
    [Order(8)]
    public class TestEditCart
    {
        private IAuthService _auth;
        private INStoreService _inStore;
        private ICartService _cart;
        private string store = "Yossi's Store8";


        [SetUpAttribute]
        public async Task SetUp()
        {
            ISystemService systemService = new SystemService();
            Result<Services> initRes = systemService.GetInstanceForTests(Path.GetFullPath("..\\..\\..\\testsConfig.json"));
            Assert.True(initRes.IsSuccess, "Error at test config file");
            Services services = initRes.Value;

            _auth = services.AuthService;
            _inStore = services.InStoreService;
            _cart = services.CartService;
            
            MemberInfo yossi = new MemberInfo("Yossi118", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("singerMermaid8", "shiran@gmail.com", "Shiran Moris",
                DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            MemberInfo lior = new MemberInfo("Liorwork8", "lior@gmail.com", "Lior Lee",
                DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
            string token = _auth.Connect();
            await _auth.Register(token, yossi, "qwerty123");
            await _auth.Register(token, shiran, "130452abc");
            await _auth.Register(token, lior, "987654321");
            Result<string> yossiLogInResult = await _auth.Login(token, "Yossi118", "qwerty123", ServiceUserRole.Member);
            IItem product = new SItem("Tara cheese", store, 10, "dairy",
                new List<string> {"dairy", "milk", "Tara"}, (double) 5.4);
            _inStore.OpenStore(yossiLogInResult.Value, store);
            _inStore.AddNewItemToStore(yossiLogInResult.Value, product);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TearDownAttribute]
        public void Teardown()
        {
            _auth = null;
            _inStore = null;
            _cart = null;
        }
        
        [TestCase("Tara cheese", "Yossi's Store8", 9)]
        [TestCase("Tara cheese", "Yossi's Store8", 3)]
        [TestCase("Tara cheese", "Yossi's Store8", 0)]
        [Test]
        public void TestEditItemAmountOfCart(string itemId, string storeName, int amount)
        {
            string token = _auth.Connect();
            Result result = _cart.AddItemToCart(token, itemId, storeName, 5);
            Assert.True(result.IsSuccess, "failed to edit item: " + result.Error);
            result = _cart.EditItemAmountOfCart(token, itemId, storeName, amount);
            Assert.True(result.IsSuccess, "failed to edit item: " + result.Error);
            _auth.Disconnect(token);
        }
        
        
        [TestCase("Tara cheese", "Yossi's Store8", -6)]
        [TestCase("Tnuva cream cheese", "Yossi's Store8", 3)]
        [TestCase("Tara cheese", "dancing dragon", 0)]
        [TestCase("Tara cheese", "Yossi's Store8", 15)]
        [Test]
        public void TestEditItemAmountOfCartFailure(string itemId, string storeName, int amount)
        {
            string token = _auth.Connect();
            _cart.AddItemToCart(token, itemId, storeName, 5);
            Result result = _cart.EditItemAmountOfCart(token, itemId, storeName, amount);
            Assert.True(result.IsFailure);
            _auth.Disconnect(token);
        }
    }
}