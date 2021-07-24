using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
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
    /// Store Owner get purchase history of a store
    /// </UC>
    /// <Req>
    /// 4.11
    /// </Req>
    /// </summary>
    [TestFixture]
    [Order(10)]
    public class TestGetPurchaseHistoryOfStore
    {
        private IAuthService _auth;
        private INStoreService _inStore;
        private ICartService _cart;
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
            _cart = services.CartService;
            
            MemberInfo yossi = new MemberInfo("Yossi11", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("singerMermaid", "shiran@gmail.com", "Shiran Moris",
                DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            string token = _auth.Connect();
            await _auth.Register(token, yossi, "qwerty123");
            await _auth.Register(token, shiran, "130452abc");
            Result<string> yossiLogInResult = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            IItem product = new SItem("Tara milk", storeName, 10, "dairy",
                new List<string>{"dairy", "milk", "Tara"}, (double)5.4);
            _inStore.OpenStore(yossiLogInResult.Value, storeName);
            _inStore.AddNewItemToStore(yossiLogInResult.Value, product);;
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

        [Test] 
        public async Task TestGetPurchaseHistoryOfStoreSuccessEmpty()
        {
            string token = _auth.Connect();
            Result<string> yossiLogInResult = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result<SPurchaseHistory> result = _inStore.GetPurchaseHistoryOfStore(yossiLogInResult.Value, storeName);
            Assert.True(result.IsSuccess && result.Value.Records.Count == 0);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [Test] 
        public async Task TestGetPurchaseHistoryOfStoreSuccessNonEmpty()
        {
            string token = _auth.Connect();
            _cart.AddItemToCart(token, "Tara milk", storeName, 5);
            _cart.PurchaseCart(token, new PaymentInfo("Yossi11","123456789","1234567890123456","12-34","123","address"));
            _auth.Disconnect(token);
            token = _auth.Connect();
            Result<string> yossiLogInResult = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result<SPurchaseHistory> result = _inStore.GetPurchaseHistoryOfStore(yossiLogInResult.Value, storeName);
            Assert.True(result.IsSuccess && result.Value.Records.Count != 0);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("Yossi11", "qwerty123","dancing dragon")]
        [TestCase("singerMermaid", "130452abc", "Borca")]
        [Test] 
        public async Task TestGetPurchaseHistoryOfStoreFailureLogic(string member, string password, string store)
        {
            string token = _auth.Connect();
            Result<string> yossiLogInResult = await _auth.Login(token, member, password, ServiceUserRole.Member);
            Result<SPurchaseHistory> result = _inStore.GetPurchaseHistoryOfStore(yossiLogInResult.Value, store);
            Assert.True(result.IsFailure);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
    }
}