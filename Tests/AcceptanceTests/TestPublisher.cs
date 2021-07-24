using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using eCommerce.Adapters;
using eCommerce.Auth;
using eCommerce.Business;
using eCommerce.Business.Repositories;
using eCommerce.Common;
using eCommerce.Publisher;
using eCommerce.Service;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Tests.Business.Mokups;
using Tests.Service;

namespace Tests.AcceptanceTests
{
    public class TestPublisher
    {
        private IAuthService _auth;
        private INStoreService _inStore;
        private ICartService _cart;
        private IUserService _user;
        private string store_name = "Borca";
        private bool shouldTearDown = false;
        private string IvanLoginToken;

        private MainPublisher _mainPublisher;
        private mokPublisherListener _publisherListener;

        public TestPublisher()
        {
            store_name += this.GetType().Name;


        }
        
        [SetUp]
        //The process:
        // Policy -> Discounts -> GetItems -> Pay -> Supply -> History -> :-)
        public async Task SetUp()
        {
           _mainPublisher=MainPublisher.Instance;
            _publisherListener = new mokPublisherListener();
            _mainPublisher.Register(_publisherListener);

            PaymentProxy.AssignPaymentService(new mokPaymentService(true,true,true));
            SupplyProxy.AssignSupplyService(new mokSupplyService(true,true));

            ISystemService systemService = new SystemService();
            Result<Services> initRes = systemService.GetInstanceForTests(Path.GetFullPath("..\\..\\..\\testsConfig.json"));
            Assert.True(initRes.IsSuccess, "Error at test config file");
            Services services = initRes.Value;

            _auth = services.AuthService;
            _inStore = services.InStoreService;
            _user = services.UserService;
            _cart = services.CartService;
            
            MemberInfo Ivan = new MemberInfo("Ivan11", "Ivan@gmail.com", "Ivan Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            string token = _auth.Connect();
            _auth.Register(token, Ivan, "qwerty123");
            var IvanLogInTask = _auth.Login(token, "Ivan11", "qwerty123", ServiceUserRole.Member);
            var IvanLogInResult = IvanLogInTask.Result;
            IItem product = new SItem("Tara milk", store_name, 200, "dairy",
                new List<string> {"dairy", "milk", "Tara"}, (double) 5.4);
            IItem product2 = new SItem("Chocolate milk", store_name, 200, "Sweet",
                new List<string> {"dairy", "milk", "sweet"}, (double) 3.5);
            var openStore = _inStore.OpenStore(IvanLogInResult.Value, store_name);
            var addItem=_inStore.AddNewItemToStore(IvanLogInResult.Value, product);
            addItem=_inStore.AddNewItemToStore(IvanLogInResult.Value, product2);
            token = _auth.Logout(IvanLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        public void Teardown()
        {
            if (shouldTearDown)
            {
                string token = _auth.Logout(IvanLoginToken).Value;
                _auth.Disconnect(token);
            }
        }
        
        [Test]
        [Order(0)]
        public void TestDirectMessageAfterBuyCart()
        {
            string token = _auth.Connect();
            string ITEM_NAME = "Tara milk";
            string IVAN_USERNAME = "Ivan11";
            int count = _publisherListener.count;
            int countInFile=_publisherListener.GetNumberOfFileLines();
            _mainPublisher.Connect(IVAN_USERNAME);
            //To check later
            
            _cart.AddItemToCart(token, ITEM_NAME, store_name, 5);
            Result purchaseResult = _cart.PurchaseCart(token, new PaymentInfo("Ivan11","123456789","1234567890123456","12/34","123","address"));
            Assert.True(purchaseResult.IsSuccess, purchaseResult.Error);
            _auth.Disconnect(token);
            var newToken = _auth.Connect();
            Assert.Less(count,_publisherListener.count);
            Assert.Less(countInFile,_publisherListener.GetNumberOfFileLines());
        }
        
        [Test]
        [Order(1)]
        public void TestDelayedMessageAfterBuyCart()
        {
            string token = _auth.Connect();
            string ITEM_NAME = "Tara milk";
            string IVAN_USERNAME = "Ivan11";
            int count = _publisherListener.count;
            int countInFile=_publisherListener.GetNumberOfFileLines();
            
            //To check later
            
            var addToCartRes=_cart.AddItemToCart(token, ITEM_NAME, store_name, 5);
            Assert.True(addToCartRes.IsSuccess,addToCartRes.Error);
            Result purchaseResult = _cart.PurchaseCart(token, new PaymentInfo("Ivan11","123456789","1234567890123456","12/34","123","address"));
            Assert.True(purchaseResult.IsSuccess, purchaseResult.Error);
            _auth.Disconnect(token);

            
            _mainPublisher.Connect(IVAN_USERNAME);
            Assert.Less(count,_publisherListener.count);
            Assert.Less(countInFile,_publisherListener.GetNumberOfFileLines());
        }
        
        
        
    }
}