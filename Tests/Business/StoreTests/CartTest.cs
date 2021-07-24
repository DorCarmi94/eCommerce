using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using eCommerce.Business;

using NUnit.Framework;
using Tests.Business.Mokups;

namespace Tests.Business.StoreTests
{
    public class CartTest
    {
        private ICart MyCart;
        private Store MyStore;
        private User Alice;
        private ItemInfo item1;
        private ItemInfo item2;
        private ItemInfo item3;
        private ItemInfo sano;
        private Store MyNewStore;
        private mokUser gedalia;
        private bool firstBidTest = false;
        public CartTest()
        {
            Alice = new mokUser("Alice");
            item1 = new ItemInfo(50, "IPhone", "Alenby", "Tech", new List<string>(), 5000);
            item2 = new ItemInfo(50, "Dell6598", "Alenby", "Tech", new List<string>(), 10000);
            MyStore = new mokStore("Alenby", Alice);
            MyCart = new Cart(Alice);
            item1.AssignStoreToItem(MyStore);
            item2.AssignStoreToItem(MyStore);
            MyNewStore = new Store("Emek haprahim", Alice);
            sano= new ItemInfo(50, "Sano Maxima", MyNewStore.StoreName, "Clean", new List<string>(), 25);
            gedalia = new mokUser("Gedalia");
            var resAddIeItemToStore = MyNewStore.AddItemToStore(sano, gedalia);
            
            //MyCart.BuyWholeCart()
            //MyCart.CalculatePricesForCart()
            //MyCart.CheckForCartHolder()
        }
        
        [SetUp]
        public void Setup()
        {
            
            //Item item = new Item();
            Debug.WriteLine("Dor");
        }
        
        [Test]
        [Order(0)]
        public void TestBidItemToCart()
        {
            int newPrice = 20;
            
            var resBidOnItem = MyNewStore.AskToBidOnItem(gedalia, sano, newPrice, 30);
            var resBidsInfos = MyNewStore.GetAllMyWaitingBids(Alice);
            Assert.True(resBidOnItem.IsSuccess);
            Assert.True(resBidsInfos.IsSuccess);
            var resBidsInfosNotOwner = MyNewStore.GetAllMyWaitingBids(gedalia);
            Assert.False(resBidsInfosNotOwner.IsSuccess);
            var firstBid = resBidsInfos.Value[0];
            var resApprove=MyNewStore.ApproveOrDissaproveBid(Alice, firstBid.BidID, true);
            Assert.True(resApprove.IsSuccess);
            var theBakset =
                gedalia._myCart._baskets.FirstOrDefault(x => x.Value._store._storeName.Equals(MyNewStore.StoreName));
            Assert.AreNotEqual(null,theBakset);
            var theItemInBasket = theBakset.Value._itemsInBasket.FirstOrDefault(x => x.name.Equals(sano.name));
            Assert.AreNotEqual(null,theItemInBasket);
            Assert.AreEqual(newPrice,theItemInBasket.pricePerUnit);
            this.firstBidTest = true;
        }
        [Test]
        [Order(1)]
        public void TestAddItemToCart()
        {
            

            MyCart.AddItemToCart(Alice, item1);
            var baskets = MyCart.GetBaskets();
            bool foundItem = false;
            for (int i = 0; i <   baskets.Count && !foundItem; i++)
            {
                var allItems = baskets[i].GetAllItems();
                if (!allItems.IsFailure)
                {
                    if (allItems.Value.FirstOrDefault(x=>x.name.Equals(item1.name))!=null)
                        
                    {
                        foundItem = true;
                    }
                }
            }
            Assert.AreEqual(true,foundItem);
        }
        [Test]
        [Order(2)]
        public void TestBidItemToCartItemExists()
        {
            if (!firstBidTest)
            {
                TestBidItemToCart();
            }
            int newPrice = 15;
            var resBidOnItem = MyNewStore.AskToBidOnItem(gedalia, sano, newPrice, 30);
            var resBidsInfos = MyNewStore.GetAllMyWaitingBids(Alice);
            Assert.True(resBidOnItem.IsSuccess);
            Assert.True(resBidsInfos.IsSuccess);
            var resBidsInfosNotOwner = MyNewStore.GetAllMyWaitingBids(gedalia);
            Assert.False(resBidsInfosNotOwner.IsSuccess);
            var firstBid = resBidsInfos.Value[0];
            var resApprove=MyNewStore.ApproveOrDissaproveBid(Alice, firstBid.BidID, true);
            Assert.True(resApprove.IsSuccess);
            var theBakset =
                gedalia._myCart._baskets.FirstOrDefault(x => x.Value._store._storeName.Equals(MyNewStore.StoreName));
            Assert.AreNotEqual(null,theBakset);
            var theItemInBasket = theBakset.Value._itemsInBasket.FirstOrDefault(x => x.name.Equals(sano.name));
            Assert.AreNotEqual(null,theItemInBasket);
            Assert.AreEqual(newPrice,theItemInBasket.pricePerUnit);
        }
        
        [Test]
        [Order(3)]
        public void TestBidItemToCartManyApproves()
        {
            int newPrice = 9;

            mokUser Guy = new mokUser("Guy");
            mokUser Raviv = new mokUser("Raviv");
            mokUser Rinat = new mokUser("Rinat");


            ItemInfo pumpkin = new ItemInfo(100, "Pumpkin", MyNewStore._storeName, "Fruit", new List<string>(), 11);
            var resAddIte=MyNewStore.AddItemToStore(pumpkin, Alice);
            Assert.True(resAddIte.IsSuccess,resAddIte.Error);
            
            
            OwnerAppointment ownerAppointmentGuy = new OwnerAppointment(Guy, MyNewStore._storeName);
            var resAppointGuy=MyNewStore.AppointNewOwner(Alice, ownerAppointmentGuy);
            Assert.True(resAppointGuy.IsSuccess,resAppointGuy.Error);
            
            OwnerAppointment ownerAppointmentRaviv = new OwnerAppointment(Raviv, MyNewStore._storeName);
            var resAppointRaviv=MyNewStore.AppointNewOwner(Alice, ownerAppointmentRaviv);
            Assert.True(resAppointRaviv.IsSuccess,resAppointRaviv.Error);
            
            OwnerAppointment ownerAppointmentRinat = new OwnerAppointment(Rinat, MyNewStore._storeName);
            var resAppointRinat=MyNewStore.AppointNewOwner(Alice, ownerAppointmentRinat);
            Assert.True(resAppointRinat.IsSuccess,resAppointRinat.Error);
            
            
            
            
            
            var resBidOnItem = MyNewStore.AskToBidOnItem(gedalia, pumpkin, newPrice, 30);
            var resBidsInfos = MyNewStore.GetAllMyWaitingBids(Alice);
            Assert.True(resBidOnItem.IsSuccess);
            Assert.True(resBidsInfos.IsSuccess);

            var firstBid = resBidsInfos.Value[0];
            var resApprove=MyNewStore.ApproveOrDissaproveBid(Alice, firstBid.BidID, true);
            Assert.True(resApprove.IsSuccess);
            var theBakset =
                gedalia._myCart._baskets.FirstOrDefault(x => x.Value._store._storeName.Equals(MyNewStore.StoreName));
            if(theBakset!=null)
            {
                var theItemInBasketInside = theBakset.Value._itemsInBasket.FirstOrDefault(x => x.name.Equals(pumpkin.name));
                Assert.Null(theItemInBasketInside);
            }
            

            resApprove=MyNewStore.ApproveOrDissaproveBid(Guy, firstBid.BidID, true);
            Assert.True(resApprove.IsSuccess);
            theBakset =
                gedalia._myCart._baskets.FirstOrDefault(x => x.Value._store._storeName.Equals(MyNewStore.StoreName));
            if(theBakset!=null)
            {
                var theItemInBasketInside = theBakset.Value._itemsInBasket.FirstOrDefault(x => x.name.Equals(pumpkin.name));
                Assert.Null(theItemInBasketInside);
            }


            resApprove=MyNewStore.ApproveOrDissaproveBid(Raviv, firstBid.BidID, true);
            Assert.True(resApprove.IsSuccess);
            theBakset =
                gedalia._myCart._baskets.FirstOrDefault(x => x.Value._store._storeName.Equals(MyNewStore.StoreName));
            if(theBakset!=null)
            {
                var theItemInBasketInside = theBakset.Value._itemsInBasket.FirstOrDefault(x => x.name.Equals(pumpkin.name));
                Assert.Null(theItemInBasketInside);
            }


            resApprove=MyNewStore.ApproveOrDissaproveBid(Rinat, firstBid.BidID, true);
            Assert.True(resApprove.IsSuccess);
            theBakset =
                gedalia._myCart._baskets.FirstOrDefault(x => x.Value._store._storeName.Equals(MyNewStore.StoreName));
            Assert.AreNotEqual(null,theBakset);
            var theItemInBasket = theBakset.Value._itemsInBasket.FirstOrDefault(x => x.name.Equals(pumpkin.name));
            Assert.AreNotEqual(null,theItemInBasket);
            Assert.AreEqual(newPrice,theItemInBasket.pricePerUnit);
        }
        
        
        
        [Test]
        public void TestEditCartItem()
        {
            var res1=this.MyCart.EditCartItem(Alice, item2);
            Assert.AreEqual(false,res1.IsSuccess);
            item1.amount = 3;
            var res2=this.MyCart.EditCartItem(Alice, item1);
            Assert.AreEqual(true,res2.IsSuccess);
        }

        [Test]
        public void TestCheckCartHolder()
        {
            var ans=this.MyCart.CheckForCartHolder(Alice);
            Assert.AreEqual(true, ans);
            User bob = new mokUser("Bob");
            Assert.AreEqual(false,MyCart.CheckForCartHolder(bob));

        }
        
        
    }
}