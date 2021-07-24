using System.Collections.Generic;
using eCommerce.Business;

using NUnit.Framework;
using Tests.Business.Mokups;

namespace Tests.Business.StoreTests
{
    public class ItemTest
    {
        private Item item1;
        
        private string storeName = "Alenby";
        private string itemName = "IPhone5";
        private int itemPrice = 3500;
        private Category itemCategory = new Category("Electronics");
        private ItemInfo info1;
        
        
        private Item item2;
        private string storeName2 = "Hashalom";
        private string itemName2 = "IPhone5.5";
        private int itemPrice2 = 3000;
        private Category itemCategory2 = new Category("AppleProducts");
        private mokUser alice;
        

        private Store alenbyStore;
        
        
        [SetUp]
        public void Setup()
        {
            //Item item = new Item();
            alice = new mokUser("Alice");
            alenbyStore = new mokStore(storeName, alice);
            item1 = new Item(itemName, itemCategory, alenbyStore, itemPrice);
            mokStore mokStore = (mokStore) alenbyStore;
            mokStore.item = item1;
            info1 = new ItemInfo(0, itemName, storeName, itemCategory.getName(), new List<string>(), itemPrice);
        }

        [Test]
        public void TestGetItemInfo()
        {
            
            var info = item1.ShowItem();
            Assert.AreEqual(itemName,                   info.name      );
            Assert.AreEqual( 1,                      info.amount    );
            Assert.AreEqual( itemCategory.getName(), info.category  );
            Assert.AreEqual(info.keyWords.Count,0);
            Assert.AreEqual(info.storeName, storeName);
            Assert.AreEqual(info.pricePerUnit, itemPrice);

            var res=item1.EditItem(new ItemInfo(50, itemName2, storeName, itemCategory2.getName(), new List<string>() {"Good"},
                itemPrice2));
            Assert.AreEqual(true,res.IsFailure);
            res=item1.EditItem(new ItemInfo(50, itemName, storeName2, itemCategory2.getName(), new List<string>() {"Good"},
                itemPrice2));
            Assert.AreEqual(true,res.IsFailure);
            
            res=item1.EditItem(new ItemInfo(50, itemName, storeName, itemCategory2.getName(), new List<string>() {"Good"},
                itemPrice2));

            info = item1.ShowItem();
            Assert.AreEqual(itemName,                   info.name      );
            Assert.AreEqual( 50,                      info.amount    );
            Assert.AreEqual( itemCategory2.getName(), info.category  );
            Assert.AreEqual(1,info.keyWords.Count);
            Assert.AreEqual(storeName, info.storeName);
            Assert.AreEqual(itemPrice2, info.pricePerUnit);


            Assert.AreEqual(50, item1.GetAmount());
            Assert.AreEqual(itemCategory2.getName(),item1.GetCategory().getName());
            Assert.AreEqual(itemName,item1.GetName());
            Assert.AreEqual(alenbyStore,item1.GetStore());
            
            //item1.EditCategory()
            //item1.EditItem()
            //item1.AssignPurchaseStrategy()
        }

        [Test]
        public void CheckDealingWithItemStock()
        {
            //item1.CheckItemAvailability()
            
            
            int amount = item1.GetAmount();
            item1.AddItems(80);
            Assert.AreEqual(amount+80,item1.GetAmount());
            Assert.AreEqual(amount + 80, item1.ShowItem().amount);
            
            
            //item1.AddItems()
            info1.amount = 50;
            
            for (int i = 0; i < 10; i++)
            {
                var avialableItems = item1.AquireItems(info1);
                Assert.AreEqual(false, avialableItems.IsFailure);
                Assert.AreEqual(50, avialableItems.GetValue().amount);
            }

            var resCheck = item1.CheckItemAvailability(50);
            Assert.AreEqual(false,resCheck.IsFailure);
            Assert.AreEqual(true,resCheck.GetValue());
            
            var resFin=item1.FinalizeGetItems(30);
            Assert.AreEqual(false,resFin.IsFailure);
            
            resCheck = item1.CheckItemAvailability(50);
            Assert.AreEqual(false,resCheck.IsFailure);
            Assert.AreEqual(false,resCheck.GetValue());
            
            resCheck = item1.CheckItemAvailability(19);
            Assert.AreEqual(false,resCheck.IsFailure);
            Assert.AreEqual(true,resCheck.GetValue());
            
            info1.amount = 50;
            
            var avialableItems2 = item1.AquireItems(info1);
            Assert.AreEqual(true, avialableItems2.IsFailure);

            info1.amount = 20;
            for (int i = 0; i < 10; i++)
            {
                avialableItems2 = item1.AquireItems(info1);
                Assert.AreEqual(false, avialableItems2.IsFailure);
                Assert.AreEqual(20, avialableItems2.GetValue().amount);
            }

        }

        [Test]
        public void TestSearchItems()
        {
            item1.AddKeyWord(alice, "Phone");
            item1.AddKeyWord(alice, "Cool");
            item1.AddKeyWord(alice, "Smartphone");
            Assert.AreEqual(true,item1.ShowItem().keyWords.Contains("Phone"));
            Assert.AreEqual(true,item1.CheckForResemblance("Phone"));
            Assert.AreEqual(true,item1.CheckForResemblance("Cool"));
            Assert.AreEqual(false,item1.CheckForResemblance("Smart"));
            Assert.AreEqual(expected:true,actual:item1.CheckForResemblance("Elect"));
            
            
            //item1.AddKeyWord()
            //item1.CheckForResemblance()
        }
        
        [Test]
        public void TestPrices()
        {

            var pricePer = item1.GetPricePerUnit();
            item1.SetPrice(alice, (int)(pricePer * 1.5));
            Assert.AreEqual((int)(pricePer * 1.5),item1.GetPricePerUnit());

            int amount = item1.GetAmount();
            Assert.AreEqual(((int) (pricePer * 1.5)) * amount, item1.GetTotalPrice());

            //item1.SetPrice()\
            //item1.GetTotalPrice()
            
            Assert.AreEqual(false,item1.CheckPricesInBetween((pricePer*0.5),pricePer*2).IsFailure);
            Assert.AreEqual(true,item1.CheckPricesInBetween((pricePer*0.5),pricePer*2).GetValue());
            //item1.CheckPricesInBetween()  
        }

        [TearDown]
        public void TearDown()
        {
            
        }
    }
}