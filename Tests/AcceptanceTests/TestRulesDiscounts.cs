using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using eCommerce.Adapters;
using eCommerce.Auth;
using eCommerce.Business;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Repositories;
using eCommerce.Common;
using eCommerce.Service;
using eCommerce.Service.StorePolicies;
using NUnit.Framework;
using Tests.AuthTests;
using Tests.Service;

namespace Tests.AcceptanceTests
{
    [TestFixture]
    public class TestRulesDiscounts
    {
        private IAuthService _auth;
        private INStoreService _store;
        private ICartService _cart;
        private IUserService _user;
        private string store_name = "Borca";
        private bool shouldTearDown = false;
        private string IvanLoginToken;


        [SetUp]
        //The process:
        // Policy -> Discounts -> GetItems -> Pay -> Supply -> History -> :-)
        public void SetUp()
        {
            
            PaymentProxy.AssignPaymentService(new mokPaymentService(true,true,true));
            SupplyProxy.AssignSupplyService(new mokSupplyService(true,true));
            ISystemService systemService = new SystemService();
            Result<Services> initRes = systemService.GetInstanceForTests(Path.GetFullPath("..\\..\\..\\testsConfig.json"));
            Assert.True(initRes.IsSuccess, "Error at test config file");
            Services services = initRes.Value;

            _auth = services.AuthService;
            _user = services.UserService;
            _store = services.InStoreService;
            _cart = services.CartService;
            
            MemberInfo Ivan = new MemberInfo("Ivan11", "Ivan@gmail.com", "Ivan Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            string token = _auth.Connect();
            _auth.Register(token, Ivan, "qwerty123");
            var IvanLogInTask = _auth.Login(token, "Ivan11", "qwerty123", ServiceUserRole.Member);
            var IvanLogInResult = IvanLogInTask.Result;
            IItem product = new SItem("Tara milk", store_name, 10, "dairy",
                new List<string> {"dairy", "milk", "Tara"}, (double) 5.4);
            IItem product2 = new SItem("Chocolate milk", store_name, 200, "Sweet",
                new List<string> {"dairy", "milk", "sweet"}, (double) 3.5);
            IItem vodka = new SItem("Vodka", store_name, 20, "Alcohol",
                new List<string> {"Alcohol"}, (double) 70);
            
            _store.OpenStore(IvanLogInResult.Value, store_name);
            _store.AddNewItemToStore(IvanLogInResult.Value, vodka);
            _store.AddNewItemToStore(IvanLogInResult.Value, product);
            _store.AddNewItemToStore(IvanLogInResult.Value, product2);
            token = _auth.Logout(IvanLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TearDown]
        public void Teardown()
        {
            

            if (shouldTearDown)
            {
                string token = _auth.Logout(IvanLoginToken).Value;
                _auth.Disconnect(token);
            }
        }
        
        //Rules
        
        [Test]
        [TestCase(5)]
        [Order(1)]
        public void TestBuyCartSuccessPolicy(int amount)
        {
            string token = _auth.Connect();
            string ITEM_NAME = "Vodka";
            string CATEGORY_NAME = "Alcohol";
            var IvanLogInTask = _auth.Login(token, "Ivan11", "qwerty123", ServiceUserRole.Member);
            var IvanLogInResult = IvanLogInTask.Result;
            Assert.True(IvanLogInResult.IsSuccess,IvanLogInResult.Error);
            this.IvanLoginToken = IvanLogInResult.Value;
            this.shouldTearDown = true;

            //To check later
            Result<SPurchaseHistory> historyResult = _store.GetPurchaseHistoryOfStore(IvanLogInResult.Value, store_name);
            Assert.True(historyResult.IsSuccess,historyResult.Error);
            int countStoreHistory = historyResult.Value.Records.Count;
            int userHistory = _user.GetPurchaseHistory(IvanLogInResult.Value).Value.Records.Count;
            int countRealPaymentHits = PaymentProxy.REAL_HITS;
            int countRealRefund = PaymentProxy.REAL_REFUNDS;
            int countRealSupplyHits = SupplyProxy.REAL_HITS;
            var resGetItem = _store.GetItem(IvanLogInResult.Value, store_name, ITEM_NAME);
            Assert.True(resGetItem.IsSuccess,resGetItem.Error);
            int itemsInStock = resGetItem.Value.Amount;

            SRuleInfo ruleInfoAge = new SRuleInfo(RuleType.Age, "18", "", "", Comperators.SMALLER_EQUALS);
            SRuleNode ruleNodeAge = new SRuleNode(SRuleNodeType.Leaf, ruleInfoAge);
            
            SRuleInfo alcoRule = new SRuleInfo(RuleType.Category, CATEGORY_NAME, "", "");
            SRuleNode ruleNodeAlco = new SRuleNode(SRuleNodeType.Leaf, alcoRule);
            
            SRuleNode ruleCombiAnd = new SRuleNode(SRuleNodeType.Composite,ruleNodeAlco,ruleNodeAge,Combinations.AND);

            var resAddToPolicy=_store.AddRuleToStorePolicy(IvanLogInResult.Value, store_name,ruleCombiAnd);
            
            Assert.True(resAddToPolicy.IsSuccess);
            
            token = _auth.Logout(IvanLogInResult.Value).Value;
            _auth.Disconnect(token);
            
            
            MemberInfo Jake = new MemberInfo("Jake11", "Ivan@gmail.com", "Jake Park",
                DateTime.ParseExact("19/04/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            string tokenJake = _auth.Connect();
            _auth.Register(tokenJake, Jake, "qwerty123");
            var JakeLogInTask = _auth.Login(tokenJake, "Jake11", "qwerty123", ServiceUserRole.Member);
            var JakeLogInResult = JakeLogInTask.Result;
            
            
            var resAddItemToCart = _cart.AddItemToCart(JakeLogInResult.Value, ITEM_NAME, store_name, amount);
            Assert.True(resAddItemToCart.IsSuccess, resAddItemToCart.Error);
            
            Result purchaseResult = _cart.PurchaseCart(JakeLogInResult.Value,
                new PaymentInfo("Ivan11", "123456789", "1234567890123456", "12/34", "123", "address"));
            
            Assert.False(purchaseResult.IsSuccess);
            Assert.True(purchaseResult.Error.Contains("<Policy>"),purchaseResult.Error);
            token = _auth.Logout(JakeLogInResult.Value).Value;
            _auth.Disconnect(token);
            
            token = _auth.Connect();
            IvanLogInTask = _auth.Login(token, "Ivan11", "qwerty123", ServiceUserRole.Member);
            IvanLogInResult = IvanLogInTask.Result;
            Assert.True(IvanLogInResult.IsSuccess,IvanLogInResult.Error);
            
            //Make sure nothing changed
            
            Assert.AreEqual(itemsInStock,_store.GetItem(IvanLogInResult.Value,store_name,ITEM_NAME).Value.Amount);
            
            Assert.AreEqual(countStoreHistory,historyResult.Value.Records.Count);
            Assert.AreEqual(userHistory,_user.GetPurchaseHistory(IvanLogInResult.Value).Value.Records.Count);
            Assert.AreEqual(countRealPaymentHits,PaymentProxy.REAL_HITS);
            Assert.AreEqual(countRealRefund,PaymentProxy.REAL_REFUNDS);
            Assert.AreEqual(countRealSupplyHits,SupplyProxy.REAL_HITS);
            token = _auth.Logout(IvanLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        
        [Test]
        [TestCase(5)]
        [Order(2)]
        public void TestBuyCartFailurePolicy(int amount)
        {
            string token = _auth.Connect();
            string ITEM_NAME = "Vodka";
            string CATEGORY_NAME = "Alcohol";
            var IvanLogInTask = _auth.Login(token, "Ivan11", "qwerty123", ServiceUserRole.Member);
            var IvanLogInResult = IvanLogInTask.Result;
            Assert.True(IvanLogInResult.IsSuccess,IvanLogInResult.Error);
            this.IvanLoginToken = IvanLogInResult.Value;
            this.shouldTearDown = true;

            //To check later
            Result<SPurchaseHistory> historyResult = _store.GetPurchaseHistoryOfStore(IvanLogInResult.Value, store_name);
            Assert.True(historyResult.IsSuccess,historyResult.Error);
            int countStoreHistory = historyResult.Value.Records.Count;
            int userHistory = _user.GetPurchaseHistory(IvanLogInResult.Value).Value.Records.Count;
            int countRealPaymentHits = PaymentProxy.REAL_HITS;
            int countRealRefund = PaymentProxy.REAL_REFUNDS;
            int countRealSupplyHits = SupplyProxy.REAL_HITS;
            var resGetItem = _store.GetItem(IvanLogInResult.Value, store_name, ITEM_NAME);
            Assert.True(resGetItem.IsSuccess,resGetItem.Error);
            int itemsInStock = resGetItem.Value.Amount;

            SRuleInfo ruleInfoAge = new SRuleInfo(RuleType.Age, "18", "", "", Comperators.SMALLER_EQUALS);
            SRuleNode ruleNodeAge = new SRuleNode(SRuleNodeType.Leaf, ruleInfoAge);
            
            SRuleInfo alcoRule = new SRuleInfo(RuleType.Category, CATEGORY_NAME, "", "");
            SRuleNode ruleNodeAlco = new SRuleNode(SRuleNodeType.Leaf, alcoRule);
            
            SRuleNode ruleCombiAnd = new SRuleNode(SRuleNodeType.Composite,ruleNodeAlco,ruleNodeAge,Combinations.AND);

            var resAddToPolicy=_store.AddRuleToStorePolicy(IvanLogInResult.Value, store_name,ruleCombiAnd);
            
            Assert.True(resAddToPolicy.IsSuccess);
            
            token = _auth.Logout(IvanLogInResult.Value).Value;
            _auth.Disconnect(token);
            
            
            MemberInfo Jake = new MemberInfo("Jake11", "Ivan@gmail.com", "Jake Park",
                DateTime.ParseExact("19/04/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            string tokenJake = _auth.Connect();
            _auth.Register(tokenJake, Jake, "qwerty123");
            var JakeLogInTask = _auth.Login(tokenJake, "Jake11", "qwerty123", ServiceUserRole.Member);
            var JakeLogInResult = JakeLogInTask.Result;
            
            
            var resAddItemToCart = _cart.AddItemToCart(JakeLogInResult.Value, ITEM_NAME, store_name, amount);
            Assert.True(resAddItemToCart.IsSuccess, resAddItemToCart.Error);
            
            Result purchaseResult = _cart.PurchaseCart(JakeLogInResult.Value,
                new PaymentInfo("Ivan11", "123456789", "1234567890123456", "12/34", "123", "address"));
            
            Assert.False(purchaseResult.IsSuccess);
            Assert.True(purchaseResult.Error.Contains("<Policy>"),purchaseResult.Error);
            token = _auth.Logout(JakeLogInResult.Value).Value;
            _auth.Disconnect(token);
            
            token = _auth.Connect();
            IvanLogInTask = _auth.Login(token, "Ivan11", "qwerty123", ServiceUserRole.Member);
            IvanLogInResult = IvanLogInTask.Result;
            Assert.True(IvanLogInResult.IsSuccess,IvanLogInResult.Error);
            
            //Make sure nothing changed
            
            Assert.AreEqual(itemsInStock,_store.GetItem(IvanLogInResult.Value,store_name,ITEM_NAME).Value.Amount);
            
            Assert.AreEqual(countStoreHistory,historyResult.Value.Records.Count);
            Assert.AreEqual(userHistory,_user.GetPurchaseHistory(IvanLogInResult.Value).Value.Records.Count);
            Assert.AreEqual(countRealPaymentHits,PaymentProxy.REAL_HITS);
            Assert.AreEqual(countRealRefund,PaymentProxy.REAL_REFUNDS);
            Assert.AreEqual(countRealSupplyHits,SupplyProxy.REAL_HITS);
            token = _auth.Logout(IvanLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        
        
        
    }
}