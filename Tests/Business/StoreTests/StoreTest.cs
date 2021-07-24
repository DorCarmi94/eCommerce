

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using eCommerce.Adapters;
using eCommerce.Business;

using eCommerce.Common;
using eCommerce.Publisher;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
using Tests.Business.Mokups;
using Tests.Service;

namespace Tests.Business.StoreTests
{
    public class StoreTest
    {
        private Store MyStore;
        private User Alice;
        private User Bob;
        private ItemInfo item1;
        private ItemInfo item1b;
        private ItemInfo item2;
        private ItemInfo item3;

        private mokPublisherListener _mokObserver;
        private MainPublisher _mainPublisher;
        public StoreTest()
        {
            Alice = new mokUser("Alice");
            Bob = new mokUser("Bob");
            this.MyStore = new Store("Alenby", Alice);
            SupplyProxy.AssignSupplyService(new mokSupplyService(true,true));
            PaymentProxy.AssignPaymentService(new mokPaymentService(true,true,true));

            _mokObserver = new mokPublisherListener();
            _mainPublisher=MainPublisher.Instance;
            _mainPublisher.Register(_mokObserver);
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
        }

        [Test]
        public void TestUsersInStore()
        {
            ManagerAppointment managerAppointment = new ManagerAppointment(Bob, MyStore.StoreName);
            OwnerAppointment ownerAppointment = new OwnerAppointment(Bob, MyStore.StoreName);
            var manager = this.MyStore.AppointNewManager(Alice, managerAppointment);
            var owner = this.MyStore.AppointNewOwner(Alice, ownerAppointment);
            
            Assert.AreEqual(true,manager.IsSuccess);
            Assert.AreEqual(true,owner.IsSuccess);

            var staff=MyStore.GetStoreStaffAndTheirPermissions(Alice);
            Assert.AreEqual(true,staff.IsSuccess);

            bool AliceInStore =
                staff.Value.FirstOrDefault(x => x.Item1.Equals(Alice.Username)) != null;
            bool BobInStore =
                staff.Value.FirstOrDefault(x => x.Item1.Equals(Bob.Username)) != null;
            
            Assert.AreEqual(true,AliceInStore);
            Assert.AreEqual(true,BobInStore);
            //MyStore.GetStoreStaffAndTheirPermissions()
        }
        
        [Test]
        [Order(0)]
        public void TestItemsInStore()
        {
            Assert.AreEqual(0,MyStore.GetAllItems().Count);
            var addRes1 = MyStore.AddItemToStore(item1, Alice);
            Assert.AreEqual(true,addRes1.IsSuccess);
            var AllItems = MyStore.GetAllItems();
            Assert.AreNotEqual(null,AllItems.FirstOrDefault(x=>x.GetName().Equals(item1.name)));

            var item1res=MyStore.GetItem(item1);
            Assert.AreEqual(true,item1res.IsSuccess);
            Assert.AreEqual(item1.name, item1res.Value.GetName());
            Assert.AreEqual(item1.Amount, item1res.Value.GetAmount());
            Assert.AreEqual(item1.category, item1res.Value.GetCategory().getName());
            Assert.AreEqual(item1.pricePerUnit, item1res.Value.GetPricePerUnit());

            var item1res2=MyStore.GetItem(item1.name);
            Assert.AreEqual(true,item1res2.IsSuccess);
            Assert.AreEqual(item1.name, item1res2.Value.GetName());
            Assert.AreEqual(item1.Amount, item1res2.Value.GetAmount());
            Assert.AreEqual(item1.category, item1res2.Value.GetCategory().getName());
            Assert.AreEqual(item1.pricePerUnit, item1res2.Value.GetPricePerUnit());

            var searchRes=MyStore.SearchItem(item1.name);
            Assert.AreNotEqual(null,searchRes.FirstOrDefault(x=>x.GetName().Equals(item1.name)));
            Assert.AreNotEqual(null,searchRes.FirstOrDefault(x=>x.GetCategory().getName().Equals(item1.category)));
            Assert.AreNotEqual(null,searchRes.FirstOrDefault(x=>x.GetPricePerUnit().Equals(item1.pricePerUnit)));
            Assert.AreNotEqual(null,searchRes.FirstOrDefault(x=>x.GetAmount().Equals(item1.amount)));

            
            var resEdit=MyStore.EditItemToStore(item1b, Alice);
            Assert.AreEqual(true,resEdit.IsSuccess);
            var item1resEdit=MyStore.GetItem(item1.name);
            Assert.AreEqual(true,item1res2.IsSuccess);
            Assert.AreEqual(item1b.name, item1res2.Value.GetName());
            Assert.AreEqual(item1b.Amount, item1res2.Value.GetAmount());
            Assert.AreEqual(item1b.category, item1res2.Value.GetCategory().getName());
            Assert.AreEqual(item1b.pricePerUnit, item1res2.Value.GetPricePerUnit());
            Assert.AreNotEqual(0,item1b.keyWords.Count);
            
            Assert.Greater(MyStore.SearchItem("IPho").Count  ,0);
            Assert.Greater(MyStore.SearchItem("Phone").Count ,0);
            Assert.Greater(MyStore.SearchItem("Comp").Count  ,0);
            Assert.Greater(MyStore.SearchItem("puters").Count,0 );
            
            Assert.Greater(MyStore.SearchItemWithCategoryFilter("IPho","Computers").Count  ,0);
            Assert.AreEqual(MyStore.SearchItemWithCategoryFilter("Phone","Else").Count ,0);

            Assert.Greater(MyStore.SearchItemWithPriceFilter("IPho",1000,10000).Count  ,0);
            Assert.AreEqual(MyStore.SearchItemWithPriceFilter("Phone",1000,1500).Count ,0);
            
            
            var resRemove=MyStore.RemoveItemToStore(item1, Alice);
            Assert.AreEqual(true,resRemove.IsSuccess);
            Assert.AreEqual(false,MyStore.GetItem(item1).IsSuccess);
            
        }
        
        [Test]
        [Order(2)]
        public void TestItemsStock()
        {
            MyStore.AddItemToStore(item2, Alice);
            Assert.GreaterOrEqual(item2.amount, MyStore.GetItem(item2).Value.GetAmount());

            MyStore.UpdateStock_AddItems(item2, Alice);
            Assert.GreaterOrEqual(2*item2.amount, MyStore.GetItem(item2).Value.GetAmount());
            
            MyStore.UpdateStock_SubtractItems(item2, Alice);
            Assert.GreaterOrEqual(item2.amount, MyStore.GetItem(item2).Value.GetAmount());
            
            var resSubtract=MyStore.UpdateStock_SubtractItems(item2, Alice);
            Assert.GreaterOrEqual(false,resSubtract.IsSuccess);

            item2.amount = item2.amount / 2;
            resSubtract=MyStore.UpdateStock_SubtractItems(item2, Alice);
            Assert.AreEqual(true,resSubtract.IsSuccess,resSubtract.Error);
        }
        
        [Test]
        [Order(1)]
        public void TestPurchaseProcess()
        {
            MyStore.AddItemToStore(item2, Alice);
            ICart cart = new Cart(Alice);
            item2.amount = 10;
            int countMessages = _mokObserver.count;
            cart.AddItemToCart(Alice, item2);

            var buyRes = cart.BuyWholeCart(Alice,
                new PaymentInfo("Alice", "369852147", "7894789478947894", "01/23", "123",
                    "Even Gavirol 30, TLV, Israel"));
            
            Assert.AreEqual(true,buyRes.IsSuccess, buyRes.Error);

        }
        
        [Test]
        [Order(3)]
        public void TestFailPurchaseProcess()
        {
            
            MyStore.AddItemToStore(item3, Alice);
            ICart alicecart = new Cart(Alice);
            item3.amount = 9;
            alicecart.AddItemToCart(Alice, item3);
            
            
            User bob = new mokUser("Bob");
            ICart bobcart = new Cart(bob);
            bobcart.AddItemToCart(bob, item3);
            
            Assert.AreEqual("",bobcart.BuyWholeCart(bob,
                new PaymentInfo("Bob", "369852147", "7894789478947894", "01/23", "123",
                    "Even Gavirol 30, TLV, Israel")).Error);
            
            Assert.AreEqual(false,alicecart.BuyWholeCart(Alice,
                new PaymentInfo("Alice", "369852147", "7894789478947894", "01/23", "123",
                    "Even Gavirol 30, TLV, Israel")).IsSuccess);

        }

        public void TestStoreInfoAndPolicy()
        {
            Assert.AreEqual("Alenby",MyStore.GetStoreName());
            //MyStore.CheckWithPolicy()
            //MyStore.UpdatePurchaseStrategies()
            //MyStore.AddPurchaseStrategyToStore()
            //MyStore.AddPurchaseStrategyToStoreItem()
            //MyStore.GetPurchaseStrategyToStoreItem()
            //MyStore.RemovePurchaseStrategyToStoreItem()
            
        }
        
        [Test]
        [Order(0)]
        public void TestBasketsInStore()
        {
            MyStore.AddItemToStore(item2, Alice);
            Cart cart = new Cart(Alice);
            Assert.AreEqual(false, MyStore.CheckConnectionToCart(cart));
            Assert.AreEqual(true,MyStore.TryAddNewCartToStore(cart));
            Basket basket = new Basket(cart, MyStore);
            var resBasket=MyStore.ConnectNewBasketToStore(basket);
            Assert.AreEqual(true,resBasket.IsSuccess);
            resBasket = MyStore.ConnectNewBasketToStore(basket);
            Assert.AreEqual(false,resBasket.IsSuccess);
            Assert.AreEqual(true, MyStore.CheckConnectionToCart(cart));
            //MyStore.CheckConnectionToCart()
            //MyStore.AddBasketToStore()
            item2.amount = 5;
            var resAddItemToBasket=basket.AddItemToBasket(Alice, item1);
            Assert.AreEqual(false,resAddItemToBasket.IsSuccess);
            resAddItemToBasket=basket.AddItemToBasket(Alice, item2);
            Assert.AreEqual(true,resAddItemToBasket.IsSuccess,resAddItemToBasket.Error);

            MyStore.CalculateBasketPrices(basket);
            Assert.AreEqual(true,basket.GetTotalPrice().IsSuccess);
            Assert.AreEqual(item2.amount*item2.pricePerUnit ,basket.GetTotalPrice().Value);

            Assert.AreEqual(false,MyStore.TryAddNewCartToStore(cart));
        }

        [Test]
        [Order(4)]
        public void TestBids()
        {
            mokUser gedalia = new mokUser("Gedalia"); 
            ItemInfo sano = new ItemInfo(50, "Sano Maxima", MyStore.StoreName, "Clean", new List<string>(), 25);
            var resAddIeItemToStore = MyStore.AddItemToStore(sano, gedalia);
            var resBidOnItem = MyStore.AskToBidOnItem(gedalia, sano, 20, 30);
            var resBidsInfos = MyStore.GetAllMyWaitingBids(Alice);
            Assert.True(resBidOnItem.IsSuccess);
            var resBidsInfosNotOwner = MyStore.GetAllMyWaitingBids(gedalia);
            Assert.False(resBidsInfosNotOwner.IsSuccess);


        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}