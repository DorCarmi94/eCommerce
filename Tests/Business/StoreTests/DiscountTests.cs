using System.Collections.Generic;
using eCommerce.Adapters;
using eCommerce.Business;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.Purchases;

using NUnit.Framework;
using Tests.Business.Mokups;
using Tests.Service;

namespace Tests.Business.StoreTests
{
    public class DiscountTests
    {
        private Store MyStore;
        private User Alice;
        private User Bob;
        private ItemInfo item1;
        private ItemInfo item1b;
        private ItemInfo item2;
        private ItemInfo item3;
        public DiscountTests()
        {
            Alice = new mokUser("Alice");
            Bob = new mokUser("Bob");
            this.MyStore = new Store("Alenby", Alice);
            SupplyProxy.AssignSupplyService(new mokSupplyService(true,true));
            PaymentProxy.AssignPaymentService(new mokPaymentService(true,true,true));
            
        }
        
        [SetUp]
        public void Setup()
        {
            
            item1 = new ItemInfo(50, "IPhone", "Alenby", "Tech", new List<string>(), 5000);
            item1.AssignStoreToItem(MyStore);
            item1b = new ItemInfo(50, "IPhone", "Alenby", "Computers", new List<string>(){"A"}, 3000);
            item1b.AssignStoreToItem(MyStore);
            item2 = new ItemInfo(50, "Dell6598", "Alenby", "Tech", new List<string>(), 10000);
            item2.AssignStoreToItem(MyStore);
            item3 = new ItemInfo(10, "HP200", "Alenby", "Tech", new List<string>(), 10000);
            item3.AssignStoreToItem(MyStore);
            var addItemToStore = MyStore.AddItemToStore(item1, Alice);
        }

        [Test]
        [Order(0)]
        public void SuccessTestDiscountLeaf()
        {
            double theDiscount = 0.5;
            RuleInfo rule = new RuleInfo(
                RuleType.Amount,
                "10",
                "Amount",
                item1.name
            );
            RuleInfoNodeLeaf ruleInfoNodeLeaf = new RuleInfoNodeLeaf(rule);
            DiscountInfoLeaf discountInfoLeaf = new DiscountInfoLeaf(theDiscount,ruleInfoNodeLeaf);
            var addDiscountToStore = MyStore.AddDiscountToStore(Alice,discountInfoLeaf);
            Assert.True(addDiscountToStore.IsSuccess,addDiscountToStore.Error);
            Cart cart = new Cart(Alice);
            item1.amount = 15;
            var addToCart=cart.AddItemToCart(Alice, item1);
            Assert.True(addToCart.IsSuccess,addToCart.Error);
            var price=cart.CalculatePricesForCart();
            Assert.True(price.IsSuccess,price.Error);
            Assert.AreEqual(item1.amount * item1.pricePerUnit * theDiscount,price.Value);

        }

        [Test]
        [Order(1)]
        public void TestSuccessDiscountRulesComposite()
        {
            double theDiscount = 0.5;
            RuleInfo rule1 = new RuleInfo(
                RuleType.Amount,
                "10",
                "Amount",
                item1.name
            );
            
            RuleInfo rule2 = new RuleInfo(
                RuleType.Category,
                "Tech",
                "Category",
                item1.name
            );

            RuleInfoNodeLeaf ruleInfoNodeLeaf1 = new RuleInfoNodeLeaf(rule1);
            RuleInfoNodeLeaf ruleInfoNodeLeaf2 = new RuleInfoNodeLeaf(rule2);

            RuleInfoNodeComposite composite = new RuleInfoNodeComposite(ruleInfoNodeLeaf1, ruleInfoNodeLeaf2, Combinations.AND);
            DiscountInfoLeaf discountInfoComp = new DiscountInfoLeaf(theDiscount,composite);
            var addDiscountToStore = MyStore.AddDiscountToStore(Alice,discountInfoComp);
            Assert.True(addDiscountToStore.IsSuccess,addDiscountToStore.Error);
            Cart cart = new Cart(Alice);
            item1.amount = 15;
            var addToCart=cart.AddItemToCart(Alice, item1);
            Assert.True(addToCart.IsSuccess,addToCart.Error);
            var price=cart.CalculatePricesForCart();
            Assert.True(price.IsSuccess,price.Error);
            Assert.AreEqual(item1.amount * item1.pricePerUnit * theDiscount,price.Value);
            
        }
        
        [Test]
        [Order(2)]
        public void TestSuccessDiscountComposite()
        {
            double theDiscount = 0.5;
            RuleInfo rule1 = new RuleInfo(
                RuleType.Amount,
                "10",
                "Amount",
                item1.name
            );
            
            RuleInfo rule2 = new RuleInfo(
                RuleType.Category,
                "Tech",
                "Category",
                item1.name
            );

            RuleInfoNodeLeaf ruleInfoNodeLeaf1 = new RuleInfoNodeLeaf(rule1);
            RuleInfoNodeLeaf ruleInfoNodeLeaf2 = new RuleInfoNodeLeaf(rule2);

            DiscountInfoLeaf discountInfoLeaf1 = new DiscountInfoLeaf(theDiscount, ruleInfoNodeLeaf1);
            DiscountInfoLeaf discountInfoLeaf2 = new DiscountInfoLeaf(theDiscount, ruleInfoNodeLeaf2);

            //RuleInfoNodeComposite composite = new RuleInfoNodeComposite(ruleInfoNodeLeaf1, ruleInfoNodeLeaf2, Combinations.AND);
            DiscountInfoCompositeNode discountInfoComp =
                new DiscountInfoCompositeNode(discountInfoLeaf1, discountInfoLeaf2, Combinations.AND);
            var addDiscountToStore = MyStore.AddDiscountToStore(Alice,discountInfoComp);
            Assert.True(addDiscountToStore.IsSuccess,addDiscountToStore.Error);
            Cart cart = new Cart(Alice);
            item1.amount = 15;
            var addToCart=cart.AddItemToCart(Alice, item1);
            Assert.True(addToCart.IsSuccess,addToCart.Error);
            var price=cart.CalculatePricesForCart();
            Assert.True(price.IsSuccess,price.Error);
            Assert.AreEqual(item1.amount * item1.pricePerUnit * theDiscount*theDiscount,price.Value);
            
        }

        [Test]
        [Order(3)]
        public void TestOrBothForDiscountAndRules()
        {
            double theDiscount = 0.5;
            RuleInfo rule1 = new RuleInfo(
                RuleType.Amount,
                "10",
                "Amount",
                item1.name
            );
            
            RuleInfo rule2 = new RuleInfo(
                RuleType.Category,
                "Tech",
                "Category",
                item1.name
            );

            RuleInfoNodeLeaf ruleInfoNodeLeaf1 = new RuleInfoNodeLeaf(rule1);
            RuleInfoNodeLeaf ruleInfoNodeLeaf2 = new RuleInfoNodeLeaf(rule2);

            DiscountInfoLeaf discountInfoLeaf1 = new DiscountInfoLeaf(theDiscount, ruleInfoNodeLeaf1);
            DiscountInfoLeaf discountInfoLeaf2 = new DiscountInfoLeaf(theDiscount, ruleInfoNodeLeaf2);

            //RuleInfoNodeComposite composite = new RuleInfoNodeComposite(ruleInfoNodeLeaf1, ruleInfoNodeLeaf2, Combinations.AND);
            DiscountInfoCompositeNode discountInfoComp =
                new DiscountInfoCompositeNode(discountInfoLeaf1, discountInfoLeaf2, Combinations.AND);
            var addDiscountToStore = MyStore.AddDiscountToStore(Alice,discountInfoComp);
            Assert.True(addDiscountToStore.IsSuccess,addDiscountToStore.Error);
            Cart cart = new Cart(Alice);
            item1.amount = 15;
            var addToCart=cart.AddItemToCart(Alice, item1);
            Assert.True(addToCart.IsSuccess,addToCart.Error);
            var price=cart.CalculatePricesForCart();
            Assert.True(price.IsSuccess,price.Error);
            Assert.AreEqual(item1.amount * item1.pricePerUnit * theDiscount*theDiscount,price.Value);
        }
    }
}