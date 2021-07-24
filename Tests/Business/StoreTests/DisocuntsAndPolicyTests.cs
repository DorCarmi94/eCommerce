using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Adapters;
using eCommerce.Business;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.DiscountsAndPurchases.Purchases.RulesInfo;
using eCommerce.Business.Purchases;

using eCommerce.Controllers;
using NUnit.Framework;
using Tests.Business.Mokups;
using Tests.Service;
using MemberInfo = eCommerce.Business.MemberInfo;

namespace Tests.Business.StoreTests
{
    public class DiscountsAndPolicyTests
    {
        private Store MyStore;
        private User Alice;
        private User Bob;
        ICart cart;
        private ItemInfo item1;
        private ItemInfo item1b;
        private ItemInfo item2;
        private ItemInfo item3;

        private List<ItemInfo> items;
        public DiscountsAndPolicyTests()
        {
            Alice = new mokUser("Alice");
            Bob = new mokUser("Bob");
            this.MyStore = new Store("Alenby", Alice);
            
            SupplyProxy.AssignSupplyService(new mokSupplyService(true,true));
            PaymentProxy.AssignPaymentService(new mokPaymentService(true,true,true));
            items = new List<ItemInfo>();
            cart = new Cart(Bob); 
            
            items.Add(new ItemInfo(50, "3% Yotveta Milk", MyStore.GetStoreName(), "Milk", new List<string>(), 10));
            items.Add(new ItemInfo(50, "Emek yellow cheese", MyStore.GetStoreName(), "Milk", new List<string>(), 5));
            items.Add(new ItemInfo(50, "Black turkish coffee", MyStore.GetStoreName(), "Coffee", new List<string>(), 8));
            items.Add(new ItemInfo(50, "Pasta", MyStore.GetStoreName(), "Breads", new List<string>(), 11));
            items.Add(new ItemInfo(50, "Milk", MyStore.GetStoreName(), "Milk", new List<string>(), 11));
            items.Add(new ItemInfo(50, "Orange", MyStore.GetStoreName(), "Vegi", new List<string>(), 11));
            items.Add(new ItemInfo(50, "Vodka", MyStore.GetStoreName(), "Alcohol", new List<string>(), 70));
            
        }
        
        [SetUp]
        public void Setup()
        {
            MyStore.ResetStoreDiscount(Alice);
            MyStore.ResetStorePolicy(Alice);
            
            foreach (var item in items)
            {
                item.amount = 50;
                item.AssignStoreToItem(MyStore);
                MyStore.AddItemToStore(item, Alice);
                item.amount = 10;
                cart.AddItemToCart(Bob, item);
            }

        }

        [Test]
        [Order(0)]
        public void SuccessMilkProducts()
        {
            var resBeforeDiscount=cart.CalculatePricesForCart();
            Assert.True(resBeforeDiscount.IsSuccess,resBeforeDiscount.Error);

            double priceBefore = 0;
            foreach (var itemInfo in items)
            {
                priceBefore += itemInfo.amount * itemInfo.pricePerUnit;
            }
            
            Assert.AreEqual(priceBefore,resBeforeDiscount.Value);

            RuleInfo ruleInfo = new RuleInfo(RuleType.Category, "Milk", "", "");
            RuleInfoNodeLeaf ruleInfoNodeLeaf = new RuleInfoNodeLeaf(ruleInfo);

            DiscountInfoLeaf discountInfoLeaf = new DiscountInfoLeaf(0.5, ruleInfoNodeLeaf);

            var addDiscount=MyStore.AddDiscountToStore(Alice, discountInfoLeaf);
            Assert.True(addDiscount.IsSuccess,addDiscount.Error);
            
            
            double priceAfter = 0;
            foreach (var itemInfo in items)
            {
                if (itemInfo.category.Equals("Milk"))
                {
                    priceAfter += itemInfo.amount * itemInfo.pricePerUnit * 0.5;
                }
                else
                {
                    priceAfter += itemInfo.amount * itemInfo.pricePerUnit;
                }
            }
            
            var resAfterDiscount=cart.CalculatePricesForCart();
            Assert.True(resAfterDiscount.IsSuccess,resAfterDiscount.Error);
            
            Assert.AreEqual(priceAfter,resAfterDiscount.Value);
        }
        
        
        [Test]
        [Order(1)]
        public void AllStore20precent()
        {
            var resBeforeDiscount=cart.CalculatePricesForCart();
            Assert.True(resBeforeDiscount.IsSuccess,resBeforeDiscount.Error);

            double priceBefore = 0;
            foreach (var itemInfo in cart.GetAllItems())
            {
                priceBefore += itemInfo.amount * itemInfo.pricePerUnit;
            }
            
            Assert.AreEqual(priceBefore,resBeforeDiscount.Value);

            RuleInfo ruleInfo = new RuleInfo(RuleType.Total_Amount, "1", "", "");
            RuleInfoNodeLeaf ruleInfoNodeLeaf = new RuleInfoNodeLeaf(ruleInfo);

            DiscountInfoLeaf discountInfoLeaf = new DiscountInfoLeaf(0.8, ruleInfoNodeLeaf);

            var addDiscount=MyStore.AddDiscountToStore(Alice, discountInfoLeaf);
            Assert.True(addDiscount.IsSuccess,addDiscount.Error);
            
            
            double priceAfter = 0;
            foreach (var itemInfo in cart.GetAllItems())
            {
                priceAfter += itemInfo.amount * itemInfo.pricePerUnit*0.8;
            }
            
            var resAfterDiscount=cart.CalculatePricesForCart();
            Assert.True(resAfterDiscount.IsSuccess,resAfterDiscount.Error);
            
            Assert.AreEqual(priceAfter,resAfterDiscount.Value);
        }

        [Test]
        [Order(2)]
        public void TotalPriceOver100()
        {

            User Charlie = new mokUser("Charlie");
            ICart cartCharlie = new Cart(Charlie);

            ItemInfo tomato = new ItemInfo(50, "Tomato", MyStore.GetStoreName(), "Vegtebales", new List<string>(), 5);
            var AssignRes= tomato.AssignStoreToItem(MyStore);
            Assert.True(AssignRes.IsSuccess,AssignRes.Error);
            var AddTomatoToStoreRes=MyStore.AddItemToStore(tomato, Alice);
            Assert.True(AddTomatoToStoreRes.IsSuccess,AddTomatoToStoreRes.Error);
            items.Add(tomato);
            
            
            foreach (var itemInfo in items)
            {
                itemInfo.amount = 1;
                var addItemToCartRes = cartCharlie.AddItemToCart(Charlie, itemInfo);
                Assert.True(addItemToCartRes.IsSuccess,addItemToCartRes.Error);
            }
            
            var resBeforeDiscount = cartCharlie.CalculatePricesForCart();
            
            double priceBefore = 0;
            foreach (var itemInfo in items)
            {
                priceBefore += itemInfo.amount * itemInfo.pricePerUnit;
            }
            
            Assert.AreEqual(priceBefore,resBeforeDiscount.Value);

            RuleInfo ruleInfo = new RuleInfo(RuleType.Total_Price, "100", "", "",Comperators.BIGGER);
            RuleInfoNodeLeaf ruleInfoNodeLeaf = new RuleInfoNodeLeaf(ruleInfo);

            RuleInfo itemsToDiscount = new RuleInfo(RuleType.IsItem, "Tomato", "", "Tomato");
            RuleInfoNodeLeaf whichItemsRule = new RuleInfoNodeLeaf(itemsToDiscount);

            DiscountInfoLeaf discountInfoLeaf = new DiscountInfoLeaf(0.9, ruleInfoNodeLeaf,whichItemsRule);

            var addDiscount=MyStore.AddDiscountToStore(Alice, discountInfoLeaf);
            Assert.True(addDiscount.IsSuccess,addDiscount.Error);
            
            foreach (var itemInfo in items)
            {
                int amountBeforeChange = itemInfo.amount;
                itemInfo.amount = 10;
                cartCharlie.AddItemToCart(Charlie, itemInfo);
                itemInfo.amount += amountBeforeChange;
            }
            
            
            double priceAfter = 0;
            foreach (var itemInfo in items)
            {
                if (itemInfo.name.Equals("Tomato"))
                {
                    priceAfter += itemInfo.amount * itemInfo.pricePerUnit * 0.9;
                }
                else
                {
                    priceAfter += itemInfo.amount * itemInfo.pricePerUnit;
                }
            }
            
            var resAfterDiscount=cartCharlie.CalculatePricesForCart();
            Assert.True(resAfterDiscount.IsSuccess,resAfterDiscount.Error);
            
            Assert.AreEqual(priceAfter,resAfterDiscount.Value);
        }

        
        [Test]
        [Order(3)]
        public void XorMilksAndBreads()
        {
            ItemInfo bread = new ItemInfo(50, "Pitah", MyStore.GetStoreName(), "Breads", new List<string>(), 11);
            items.Add(bread);
            var AssignRes= bread.AssignStoreToItem(MyStore);
            Assert.True(AssignRes.IsSuccess,AssignRes.Error);
            var AddTomatoToStoreRes=MyStore.AddItemToStore(bread, Alice);
            Assert.True(AddTomatoToStoreRes.IsSuccess,AddTomatoToStoreRes.Error);

            User David = new mokUser("David");
            ICart davidCart = new Cart(David);
            foreach (var itemInfo in items)
            {
                if (itemInfo.category.ToUpper().Equals("MILK"))
                {
                    itemInfo.amount = 10;
                }
                else
                {
                    itemInfo.amount = 2;
                }
                var addItemToCartRes = davidCart.AddItemToCart(David, itemInfo);
                Assert.True(addItemToCartRes.IsSuccess,addItemToCartRes.Error);
            }
            
            var resBeforeDiscount = davidCart.CalculatePricesForCart();
            
            double priceBefore = 0;
            foreach (var itemInfo in items)
            {
                priceBefore += itemInfo.amount * itemInfo.pricePerUnit;
            }
            
            Assert.AreEqual(priceBefore,resBeforeDiscount.Value);

            RuleInfo ruleInfoMilks = new RuleInfo(RuleType.Category, "Milk", "", "");
            RuleInfoNodeLeaf ruleInfoNodeLeafMilks = new RuleInfoNodeLeaf(ruleInfoMilks);
            DiscountInfoLeaf discountInfoLeafMilks = new DiscountInfoLeaf(0.5, ruleInfoNodeLeafMilks);
            
            RuleInfo ruleInfoBreads = new RuleInfo(RuleType.Category, "Breads", "", "");
            RuleInfoNodeLeaf ruleInfoNodeLeafBreads = new RuleInfoNodeLeaf(ruleInfoBreads);
            DiscountInfoLeaf discountInfoLeafBreads = new DiscountInfoLeaf(0.5, ruleInfoNodeLeafBreads);


            DiscountInfoCompositeNode xorDiscount = new DiscountInfoCompositeNode(discountInfoLeafBreads,
                discountInfoLeafMilks, Comperators.SMALLER, FieldToCompare.NumberOfItems);
            

            var addDiscount=MyStore.AddDiscountToStore(Alice, xorDiscount);
            Assert.True(addDiscount.IsSuccess,addDiscount.Error);


            double priceAfter = 0;
            foreach (var itemInfo in davidCart.GetAllItems()) 
            {
                if (itemInfo.category.ToUpper().Equals("MILK"))
                {
                    priceAfter += itemInfo.amount * itemInfo.pricePerUnit * 0.5;
                }
                else
                {
                    priceAfter += itemInfo.amount * itemInfo.pricePerUnit;
                }
            }
            
            var resAfterDiscount=davidCart.CalculatePricesForCart();
            Assert.True(resAfterDiscount.IsSuccess,resAfterDiscount.Error);
            
            Assert.AreEqual(priceAfter,resAfterDiscount.Value);

        }
        
        [Test]
        [Order(3)]
        public void BansAndBreads()
        {
            ItemInfo bread = new ItemInfo(50, "Bread", MyStore.GetStoreName(), "Breads", new List<string>(), 11);
            items.Add(bread);
            var AssignRes= bread.AssignStoreToItem(MyStore);
            Assert.True(AssignRes.IsSuccess,AssignRes.Error);
            var AddBreadToStoreRes=MyStore.AddItemToStore(bread, Alice);
            Assert.True(AddBreadToStoreRes.IsSuccess,AddBreadToStoreRes.Error);
            
            ItemInfo bans = new ItemInfo(50, "Ban", MyStore.GetStoreName(), "Breads", new List<string>(), 11);
            items.Add(bans);
            var AssignBans= bans.AssignStoreToItem(MyStore);
            Assert.True(AssignBans.IsSuccess,AssignBans.Error);
            var AddBansToStoreRes=MyStore.AddItemToStore(bans, Alice);
            Assert.True(AddBansToStoreRes.IsSuccess,AddBansToStoreRes.Error);

            User Elie = new mokUser("Elie");
            Cart elieCart = new Cart(Elie);
            
            foreach (var itemInfo in items)
            {
                itemInfo.amount = 1;
                var addItemToCartRes = elieCart.AddItemToCart(Elie, itemInfo);
                Assert.True(addItemToCartRes.IsSuccess,addItemToCartRes.Error);
            }
            
            
            RuleInfo ruleInfoBread = new RuleInfo(RuleType.Amount, "5", "", "Bread",Comperators.BIGGER_EQUALS);
            RuleInfoNodeLeaf ruleInfoNodeLeafBread = new RuleInfoNodeLeaf(ruleInfoBread);
            RuleInfo ruleInfoBans = new RuleInfo(RuleType.Amount, "2", "", "Ban",Comperators.BIGGER_EQUALS);
            RuleInfoNodeLeaf ruleInfoNodeLeafBan = new RuleInfoNodeLeaf(ruleInfoBans);
            RuleInfoNodeComposite ruleInfoNodeComposite =
                new RuleInfoNodeComposite(ruleInfoNodeLeafBread, ruleInfoNodeLeafBan, Combinations.AND);

            DiscountInfoLeaf discountInfoLeafBread = new DiscountInfoLeaf(0.95, ruleInfoNodeLeafBread);
            DiscountInfoLeaf discountInfoLeafBan = new DiscountInfoLeaf(0.95, ruleInfoNodeLeafBan);

            DiscountInfoCompositeNode discountInfoCompositeNode =
                new DiscountInfoCompositeNode(discountInfoLeafBan, discountInfoLeafBread, Combinations.AND);


            var addDiscount=MyStore.AddDiscountToStore(Alice, discountInfoCompositeNode);
            Assert.True(addDiscount.IsSuccess,addDiscount.Error);

            var resBeforeDiscount = elieCart.CalculatePricesForCart();
            
            double priceBefore = 0;
            foreach (var itemInfo in items)
            {
                priceBefore += itemInfo.amount * itemInfo.pricePerUnit;
            }
            
            Assert.AreEqual(priceBefore,resBeforeDiscount.Value);

            bans.amount = 2;
            bread.amount = 5;

            elieCart.AddItemToCart(Elie,bans);
            elieCart.AddItemToCart(Elie,bread);

            bans.amount += 1; //for what was before, aggregate
            bread.amount += 1;//for what was before, aggregate


            double priceAfter = 0;
            foreach (var itemInfo in items)
            {
                if (itemInfo.name.Equals(bread.name) || itemInfo.name.Equals(bans.name))
                {
                    priceAfter += itemInfo.amount * itemInfo.pricePerUnit * 0.95;
                }
                else
                {
                    priceAfter += itemInfo.amount * itemInfo.pricePerUnit;
                }
            }
            
            var resAfterDiscount=elieCart.CalculatePricesForCart();
            Assert.True(resAfterDiscount.IsSuccess,resAfterDiscount.Error);
            
            Assert.True(Math.Abs(priceAfter-resAfterDiscount.Value)<0.1,Math.Abs(priceAfter-resAfterDiscount.Value).ToString());

        }
        
        [Test]
        [Order(4)]
        public void MilksOr()
        {
            ItemInfo kotej = new ItemInfo(50, "Kotej", MyStore.GetStoreName(), "Milk", new List<string>(), 11);
            items.Add(kotej);
            var AssignRes= kotej.AssignStoreToItem(MyStore);
            Assert.True(AssignRes.IsSuccess,AssignRes.Error);
            var AddKotejToStoreRes=MyStore.AddItemToStore(kotej, Alice);
            Assert.True(AddKotejToStoreRes.IsSuccess,AddKotejToStoreRes.Error);
            
            ItemInfo yogurt = new ItemInfo(50, "Yogurt", MyStore.GetStoreName(), "Milk", new List<string>(), 11);
            items.Add(yogurt);
            var AssignYogurt= yogurt.AssignStoreToItem(MyStore);
            Assert.True(AssignYogurt.IsSuccess,AssignYogurt.Error);
            var AddYogurtToStoreRes=MyStore.AddItemToStore(yogurt, Alice);
            Assert.True(AddYogurtToStoreRes.IsSuccess,AddYogurtToStoreRes.Error);

            User Fredie = new mokUser("Fredie");
            Cart fredieCart = new Cart(Fredie);
            
            foreach (var itemInfo in items)
            {
                itemInfo.amount = 1;
                var addItemToCartRes = fredieCart.AddItemToCart(Fredie, itemInfo);
                Assert.True(addItemToCartRes.IsSuccess,addItemToCartRes.Error);
            }
            
            
            RuleInfo ruleInfoKotej = new RuleInfo(RuleType.Amount, "3", "", kotej.name,Comperators.BIGGER_EQUALS);
            RuleInfoNodeLeaf ruleInfoNodeLeafKotej = new RuleInfoNodeLeaf(ruleInfoKotej);
            RuleInfo ruleInfoYogurt = new RuleInfo(RuleType.Amount, "2", "", yogurt.name,Comperators.BIGGER_EQUALS);
            RuleInfoNodeLeaf ruleInfoNodeLeafYogurt = new RuleInfoNodeLeaf(ruleInfoYogurt);
            RuleInfoNodeComposite ruleInfoNodeComposite =
                new RuleInfoNodeComposite(ruleInfoNodeLeafKotej, ruleInfoNodeLeafYogurt, Combinations.OR);

            RuleInfo whichItemsForDiscount = new RuleInfo(RuleType.Category, "Milk", "", "");
            RuleInfoNodeLeaf whichItemRulLeaf = new RuleInfoNodeLeaf(whichItemsForDiscount);

            DiscountInfoLeaf discountInfoLeafMilks = new DiscountInfoLeaf(0.95, ruleInfoNodeComposite,whichItemRulLeaf);
            
            var addDiscount=MyStore.AddDiscountToStore(Alice, discountInfoLeafMilks);
            Assert.True(addDiscount.IsSuccess,addDiscount.Error);

            var resBeforeDiscount = fredieCart.CalculatePricesForCart();
            
            double priceBefore = 0;
            foreach (var itemInfo in items)
            {
                priceBefore += itemInfo.amount * itemInfo.pricePerUnit;
            }
            
            Assert.AreEqual(priceBefore,resBeforeDiscount.Value);

            int amountBeforeChange = yogurt.amount;
            yogurt.amount = 2;
            

            fredieCart.AddItemToCart(Fredie,yogurt);

            yogurt.amount += amountBeforeChange;
            
            double priceAfter = 0;
            foreach (var itemInfo in items)
            {
                if (itemInfo.category.Equals("Milk"))
                {
                    priceAfter += itemInfo.amount * itemInfo.pricePerUnit*0.95;
                }
                else
                {
                    priceAfter += itemInfo.amount * itemInfo.pricePerUnit;
                }
            }
            
            var resAfterDiscount=fredieCart.CalculatePricesForCart();
            Assert.True(resAfterDiscount.IsSuccess,resAfterDiscount.Error);
            
            Assert.True(Math.Abs(priceAfter-resAfterDiscount.Value)<0.1,Math.Abs(priceAfter-resAfterDiscount.Value).ToString());

        }
        
        [Test]
        [Order(5)]
        public void TotalPriceAndPasta()
        {
            ItemInfo pasta = items.Where(x => x.name.ToUpper().Equals("PASTA")).FirstOrDefault();
            
            
            User Gool = new mokUser("Gool");
            Cart goolCart = new Cart(Gool);
            
            foreach (var itemInfo in items)
            {
                itemInfo.amount = 1;
                var addItemToCartRes = goolCart.AddItemToCart(Gool, itemInfo);
                Assert.True(addItemToCartRes.IsSuccess,addItemToCartRes.Error);
            }


            RuleInfo totalprice = new RuleInfo(RuleType.Total_Price, "100", "", "",Comperators.BIGGER);
            RuleInfoNodeLeaf totalPriceLeaf = new RuleInfoNodeLeaf(totalprice);

            RuleInfo pastas = new RuleInfo(RuleType.Amount, "3", "", pasta.name, Comperators.BIGGER_EQUALS);
            RuleInfoNodeLeaf pastaLeaf = new RuleInfoNodeLeaf(pastas);

            RuleInfoNodeComposite ruleInfoNodeComposite =
                new RuleInfoNodeComposite(totalPriceLeaf, pastaLeaf, Combinations.AND);
            

            RuleInfo whichItemsForDiscount = new RuleInfo(RuleType.Category, "Milk", "", "");
            RuleInfoNodeLeaf whichItemRulLeaf = new RuleInfoNodeLeaf(whichItemsForDiscount);

            DiscountInfoLeaf discountInfoLeafMilks = new DiscountInfoLeaf(0.95, ruleInfoNodeComposite,whichItemRulLeaf);
            
            var addDiscount=MyStore.AddDiscountToStore(Alice, discountInfoLeafMilks);
            Assert.True(addDiscount.IsSuccess,addDiscount.Error);

            var resBeforeDiscount = goolCart.CalculatePricesForCart();

            double priceBefore = 0;
            foreach (var itemInfo in items)
            {
                priceBefore += itemInfo.amount * itemInfo.pricePerUnit;
            }
            
            Assert.AreEqual(priceBefore,resBeforeDiscount.Value);

            int items0amountbeforeChange = items[0].amount;
            items[0].amount = 12;
            

            goolCart.AddItemToCart(Gool,items[0]);

            items[0].amount += items0amountbeforeChange;
            
            double priceBefore2 = 0;
            foreach (var itemInfo in items)
            {
                priceBefore2 += itemInfo.amount * itemInfo.pricePerUnit;
            }
            
            resBeforeDiscount = goolCart.CalculatePricesForCart();
            
            Assert.AreEqual(priceBefore2,resBeforeDiscount.Value);

            int amountBeforeChange = pasta.amount;
            pasta.amount = 3;
            goolCart.AddItemToCart(Gool,pasta);
            pasta.amount += amountBeforeChange;
            
            double priceAfter = 0;
            foreach (var itemInfo in items)
            {
                if (itemInfo.category.Equals("Milk"))
                {
                    priceAfter += itemInfo.amount * itemInfo.pricePerUnit*0.95;
                }
                else
                {
                    priceAfter += itemInfo.amount * itemInfo.pricePerUnit;
                }
            }
            
            var resAfterDiscount=goolCart.CalculatePricesForCart();
            Assert.True(resAfterDiscount.IsSuccess,resAfterDiscount.Error);
            
            Assert.True(Math.Abs(priceAfter-resAfterDiscount.Value)<0.1,Math.Abs(priceAfter-resAfterDiscount.Value).ToString());

        }
        
        [Test]
        [Order(6)]
        public void TotalMax()
        {
            ItemInfo pasta = items.Where(x => x.name.ToUpper().Equals("PASTA")).FirstOrDefault();
            
            ItemInfo milk = items.Where(x => x.name.ToUpper().Equals("MILK")).FirstOrDefault();

            User Gool = new mokUser("Gool");
            Cart goolCart = new Cart(Gool);

            int i = 1;
            foreach (var itemInfo in items)
            {
                itemInfo.amount = i;
                i++;
                var addItemToCartRes = goolCart.AddItemToCart(Gool, itemInfo);
                Assert.True(addItemToCartRes.IsSuccess,addItemToCartRes.Error);
            }


            RuleInfo pastaRule = new RuleInfo(RuleType.IsItem, "", "", pasta.name);
            RuleInfoNodeLeaf pastaRuleLeaf = new RuleInfoNodeLeaf(pastaRule);
            DiscountInfoLeaf discountInfoLeafPasta = new DiscountInfoLeaf(0.95, pastaRuleLeaf);

            RuleInfo milkRule = new RuleInfo(RuleType.IsItem, "", "", milk.name);
            RuleInfoNodeLeaf milkRuleLeaf = new RuleInfoNodeLeaf(milkRule);
            DiscountInfoLeaf discountInfoLeafMilk = new DiscountInfoLeaf(0.83, milkRuleLeaf);

            DiscountInfoCompositeNode discountInfoCompositeNode =
                new DiscountInfoCompositeNode(discountInfoLeafPasta, discountInfoLeafMilk, Combinations.MAX);
            
            var addDiscount=MyStore.AddDiscountToStore(Alice, discountInfoCompositeNode);
            Assert.True(addDiscount.IsSuccess,addDiscount.Error);

            var resBeforeDiscount = goolCart.CalculatePricesForCart();
            
            
            double pricePasta = 0;
            double priceMilk = 0;
            foreach (var itemInfo in items)
            {
                if (itemInfo.name.Equals(milk.name))
                {
                    priceMilk += itemInfo.amount * itemInfo.pricePerUnit*0.83;
                    pricePasta += itemInfo.amount * itemInfo.pricePerUnit;
                }
                else if (itemInfo.name.Equals(pasta.name))
                {
                    pricePasta += itemInfo.amount * itemInfo.pricePerUnit*0.95;
                    priceMilk += itemInfo.amount * itemInfo.pricePerUnit;
                }
                else
                {
                    priceMilk += itemInfo.amount * itemInfo.pricePerUnit;
                    pricePasta += itemInfo.amount * itemInfo.pricePerUnit;
                }
            }

            double priceAfter = priceMilk > pricePasta ? priceMilk : pricePasta;
            
            var resAfterDiscount=goolCart.CalculatePricesForCart();
            Assert.True(resAfterDiscount.IsSuccess,resAfterDiscount.Error);
            
            Assert.True(Math.Abs(priceAfter-resAfterDiscount.Value)<0.1,Math.Abs(priceAfter-resAfterDiscount.Value).ToString());

        }
        
        [Test]
        [Order(7)]
        public void PlusTest()
        {
            
            

            User Gool = new mokUser("Gool");
            Cart goolCart = new Cart(Gool);

            int i = 1;
            foreach (var itemInfo in items)
            {
                itemInfo.amount = i;
                i++;
                var addItemToCartRes = goolCart.AddItemToCart(Gool, itemInfo);
                Assert.True(addItemToCartRes.IsSuccess,addItemToCartRes.Error);
            }
            
            double priceBefore = 0;
            foreach (var itemInfo in items)
            {
                priceBefore += itemInfo.amount * itemInfo.pricePerUnit;
            }

            var resBeforeDiscount = goolCart.CalculatePricesForCart();
            
            Assert.AreEqual(priceBefore,resBeforeDiscount.Value);
            

            RuleInfo milkRule = new RuleInfo(RuleType.Category, "Milk", "", "");
            RuleInfoNodeLeaf milkRuleLeaf = new RuleInfoNodeLeaf(milkRule);
            DiscountInfoLeaf discountInfoLeafMilk = new DiscountInfoLeaf(0.95, milkRuleLeaf);
            
            RuleInfo allStoreRule = new RuleInfo(RuleType.Total_Amount, "1", "", "");
            RuleInfoNodeLeaf allStoreRuleInfoNodeLeaf = new RuleInfoNodeLeaf(allStoreRule);
            DiscountInfoLeaf discountInfoLeafAllStore = new DiscountInfoLeaf(0.80, allStoreRuleInfoNodeLeaf);

            DiscountInfoCompositeNode discountInfoCompositeNode =
                new DiscountInfoCompositeNode(discountInfoLeafMilk, discountInfoLeafAllStore, Combinations.PLUS);
            
            var addDiscount=MyStore.AddDiscountToStore(Alice, discountInfoCompositeNode);
            Assert.True(addDiscount.IsSuccess,addDiscount.Error);


            double priceAfter = 0;
            foreach (var itemInfo in items)
            {
                if (itemInfo.category.Equals("Milk"))
                {
                    priceAfter += itemInfo.amount * itemInfo.pricePerUnit * 0.75;
                }
                else
                {
                    priceAfter += itemInfo.amount * itemInfo.pricePerUnit * 0.8;
                }
            }

           
            
            var resAfterDiscount=goolCart.CalculatePricesForCart();
            Assert.True(resAfterDiscount.IsSuccess,resAfterDiscount.Error);
            
            Assert.True(Math.Abs(priceAfter-resAfterDiscount.Value)<0.1,Math.Abs(priceAfter-resAfterDiscount.Value).ToString());

        }


        [Test]
        [Order(8)]
        public void TestRuleTomatos()
        {
            User Hook = new mokUser("Hook");
            ICart hookCart = new Cart(Hook);
            
            ItemInfo orange = items.Where(x => x.name.ToUpper().Equals("ORANGE")).FirstOrDefault();
            orange.amount = 8;

            RuleInfo ruleInfo = new RuleInfo(RuleType.Amount, "8", "", orange.name, Comperators.BIGGER);
            RuleInfoNodeLeaf ruleInfoNodeLeaf = new RuleInfoNodeLeaf(ruleInfo);

            var resAddToPolicy=MyStore.AddRuleToStorePolicy(Alice, ruleInfoNodeLeaf);
            
            Assert.True(resAddToPolicy.IsSuccess);

            var addItemToCart=hookCart.AddItemToCart(Hook, orange);

            foreach (var basket in hookCart.GetBaskets())
            {
                var resPolicy=MyStore.CheckWithStorePolicy(basket, Hook);
                Assert.True(resPolicy.IsSuccess);
            }

            orange.amount = 15;
            addItemToCart=hookCart.AddItemToCart(Hook, orange);

            bool oneFales = true;
            
            foreach (var basket in hookCart.GetBaskets())
            {
                var resPolicy=MyStore.CheckWithStorePolicy(basket, Hook);
                oneFales &= resPolicy.IsSuccess;
            }
            
            Assert.False(oneFales);
        }
        
        
        [Test]
        [Order(9)]
        public void TestRuleAlcohol()
        {
            mokUser Hook = new mokUser("Hook");
            //public MemberInfo(string username, string email, string name, DateTime birthday, string address)
            MemberInfo memberInfo =
                new MemberInfo("Hook", "", "", new DateTime(2000, 5, 3), "");
            Hook.SetMemberInfo(memberInfo);
            ICart hookCart = new Cart(Hook);
            
            ItemInfo vodka = items.FirstOrDefault(x => x.name.ToUpper().Equals("VODKA"));
            vodka.amount = 8;

            RuleInfo ruleInfo = new RuleInfo(RuleType.Age, "18", "", "", Comperators.SMALLER_EQUALS);
            RuleInfoNodeLeaf ruleInfoNodeLeaf = new RuleInfoNodeLeaf(ruleInfo);

            RuleInfo alco = new RuleInfo(RuleType.Category, vodka.category, "", "");
            RuleInfoNodeLeaf alcoLeaf = new RuleInfoNodeLeaf(alco);

            RuleInfoNodeComposite andAlcoAge = new RuleInfoNodeComposite(alcoLeaf, ruleInfoNodeLeaf, Combinations.AND);

            var resAddToPolicy=MyStore.AddRuleToStorePolicy(Alice, andAlcoAge);
            
            Assert.True(resAddToPolicy.IsSuccess);

            var addItemToCart=hookCart.AddItemToCart(Hook, vodka);

            foreach (var basket in hookCart.GetBaskets())
            {
                var resPolicy=MyStore.CheckWithStorePolicy(basket, Hook);
                Assert.True(resPolicy.IsSuccess,resPolicy.Error);
            }
            
            mokUser Jake = new mokUser("Jake");
            MemberInfo memberInfoJake =
                new eCommerce.Controllers.MemberInfo("Jake", "", "", new DateTime(2010, 5, 3), "", "");
            Jake.MemberInfo = memberInfoJake;
            ICart jakeCart = new Cart(Jake);

            vodka.amount = 15;
            addItemToCart=jakeCart.AddItemToCart(Jake, vodka);

            bool oneFales = true;
            
            foreach (var basket in jakeCart.GetBaskets())
            {
                var resPolicy=MyStore.CheckWithStorePolicy(basket, Jake);
                oneFales &= resPolicy.IsSuccess;
            }
            
            Assert.False(oneFales);
        }

    }
}