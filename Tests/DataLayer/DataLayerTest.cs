using System;
using System.Collections.Generic;
using eCommerce.Adapters;
using eCommerce.Business;
using eCommerce.Common;
using eCommerce.DataLayer;
using NUnit.Framework;

namespace Tests.DataLayer
{
    /**
     * DB should be clear
     */
    [TestFixture]
    public class DataLayerTest
    {
        private DataFacade df;
        private User ja;
        private User jaren;
        private User dillon;
        private Store store1;
        private Store store2;
        private Store store3;
        private Store store4;
        private Store alenbyStore;
       
        public DataLayerTest()
        {
            df = DataFacade.Instance;
            df.init(true);
        }

        

        [SetUp]
        public void Setup()
        {
            // set up business users and stores
            var info1 = new MemberInfo("Ja Morant", "ja@mail.com", "Ja", DateTime.Now, "Memphis");
            info1.Id = "12";
            var info2 = new MemberInfo("Jaren Jackson Jr.", "jjj@mail.com", "Jaren", DateTime.Now, "Memphis");
            info2.Id = "13";
            var info3 = new MemberInfo("Dillon Brooks", "db@mail.com", "Dillon", DateTime.Now, "Memphis");
            info3.Id = "24";

            ja = new User(info1);
            jaren = new User(info2);
            dillon = new User(info3);
            store1= new Store("BasketBall stuff.. buy here",ja);
            ja.OpenStore(store1);
            store2= new Store("More(!) BasketBall stuff.. buy here",ja);
            store3= new Store("EVEN MORE!! BasketBall stuff.. buy here",ja);
            store4= new Store("ok we had too much BasketBall stuff.. buy here",ja);
            alenbyStore= new Store("this is the store in alenby.. buy here",ja);

            //clear DB
            df.ClearTables();
        }

        [Test]
        [Order(1)]
        public void SaveUserTest()
        {
            Assert.True(df.SaveUser(ja).IsSuccess);
        }
        
        [Test]
        [Order(2)]
        public void ReadUserTest()
        {
            SaveUserTest();
            Assert.True(df.ResetConnection().IsSuccess);
            Assert.True(df.ReadUser(ja.Username).IsSuccess);
        }
        
        
        [Test]
        public void SaveUserWithStoreTest()
        {
            Assert.True(df.SaveUser(ja).IsSuccess);
            Assert.True(df.SaveUser(jaren).IsSuccess);
            Assert.True(df.SaveStore(store1).IsSuccess);
            Assert.True(df.SaveStore(store2).IsSuccess);
            Assert.True(df.SaveStore(store3).IsSuccess);
            Assert.True(df.SaveStore(store4).IsSuccess);

            ja.OpenStore(store1);
            ja.OpenStore(store2);
            ja.OpenStore(store3);
            ja.OpenStore(store4);
            ja.AppointUserToManager(store1,jaren);
            ja.AppointUserToManager(store2,jaren);
            ja.AppointUserToOwner(store3, jaren);
            ja.AppointUserToOwner(store4, jaren);
            var prems = new List<StorePermission>() {StorePermission.ChangeItemPrice, StorePermission.ControlStaffPermission, StorePermission.EditStorePolicy, StorePermission.AddItemToStore};
            ja.UpdatePermissionsToManager(store2,jaren, prems);
           
            Assert.True(df.UpdateUser(ja).IsSuccess);
            Assert.True(df.UpdateUser(jaren).IsSuccess);
            Assert.True(df.UpdateStore(store1).IsSuccess);
            Assert.True(df.UpdateStore(store3).IsSuccess);
        }

        [Test]
        public void SaveMillionPurchases()
        {
            var iphoneItem = new ItemInfo(3000000, "iPhone", store1._storeName, "tech", new List<string>(), 3500);
            var assignRes=iphoneItem.AssignStoreToItem(store1);
            Assert.True(assignRes.IsSuccess,assignRes.Error);
            var addToStoreREs=store1.AddItemToStore(iphoneItem, ja);
            Assert.True(addToStoreREs.IsSuccess,addToStoreREs.Error);
            iphoneItem.amount = 2;
            var addToCartRes=ja.AddItemToCart(iphoneItem);
            Assert.True(addToCartRes.IsSuccess,addToCartRes.Error);
            var buyRes=ja.BuyWholeCart(new PaymentInfo(ja.Username, "111", "111", "11/11", "111", "abc"));
            Assert.True(buyRes.IsSuccess,buyRes.Error);
            df.UpdateStore(store1);
            df.UpdateUser(ja);



        }

        [Test]
        [Order(3)]
        public void SaveStoreTest()
        {
            SaveUserTest();
            var store = new Store("Store", df.ReadUser(ja.Username).Value);
            Assert.True(df.SaveStore(store).IsSuccess);
        }
        
        [Test]
        public void SaveStoreWithItemTest()
        {
            SaveUserTest();
            User user = df.ReadUser(ja.Username).Value;
            
            Store store = new Store("Store", df.ReadUser(ja.Username).Value);
            Result openRes = user.OpenStore(store);
            Assert.True(openRes.IsSuccess, $"Opening store error {openRes.Error}");
            
            Result addRes = store.AddItemToStore(new ItemInfo(10, "item1", store._storeName, "items",
                new List<string>() {"keyword1", "keyword1"}, 10.0), user);
            Assert.True(addRes.IsSuccess, $"Adding item to store error {addRes.Error}");
            
            Assert.True(df.SaveStore(store).IsSuccess, "Save store to db");
        }


        [Test]
        public void ReadUserWithStoreTest()
        {
            SaveUserWithStoreTest();
            Assert.True(df.ResetConnection().IsSuccess);
            var username = ja.Username;
            var res = df.ReadUser(username);
            Assert.True(res.IsSuccess, res.Error);
            
            username = jaren.Username;
            res = df.ReadUser(username);
            Assert.True(res.IsSuccess, res.Error);
        }

        [Test]
        public void SaveUserCartTest()
        {
            
            Assert.True(df.SaveUser(ja).IsSuccess);
            Assert.True(df.SaveUser(jaren).IsSuccess);
            ja.OpenStore(alenbyStore);
            ja.OpenStore(store1);
            Assert.True(df.SaveStore(alenbyStore).IsSuccess);
            Assert.True(df.SaveStore(store1).IsSuccess);

            
            var pstation4 = new ItemInfo(100, "Playstation4", alenbyStore.GetStoreName(), "Tech",
                new List<string>(), 2500);
            var pstation5 = new ItemInfo(100, "Playstation5", alenbyStore.GetStoreName(), "Tech",
                new List<string>(), 4500);
            alenbyStore.AddItemToStore(pstation4,ja);
            alenbyStore.AddItemToStore(pstation5,ja);

            
            var xboxOne = new ItemInfo(100, "xboxOne", store1.GetStoreName(), "Tech",
                new List<string>(), 2500);
            var xboxX = new ItemInfo(100, "xboxX", store1.GetStoreName(), "Tech",
                new List<string>(), 4500);
            store1.AddItemToStore(xboxOne,ja);
            store1.AddItemToStore(xboxX,ja);

            
            var resGetItem=alenbyStore.GetItem(pstation4);
            var showItem = resGetItem.Value.ShowItem();
            showItem.amount = 4;
            jaren.AddItemToCart(showItem);
            resGetItem=alenbyStore.GetItem(pstation5);
            showItem = resGetItem.Value.ShowItem();
            showItem.amount = 5;
            jaren.AddItemToCart(showItem);
            
            resGetItem=store1.GetItem(xboxOne);
            showItem = resGetItem.Value.ShowItem();
            showItem.amount = 6;
            jaren.AddItemToCart(showItem);
            resGetItem=store1.GetItem(xboxX);
            showItem = resGetItem.Value.ShowItem();
            showItem.amount = 7;
            jaren.AddItemToCart(showItem);
            
            Assert.True(df.UpdateUser(ja).IsSuccess);
            Assert.True(df.UpdateUser(jaren).IsSuccess);
            Assert.True(df.UpdateStore(alenbyStore).IsSuccess);
            Assert.True(df.UpdateStore(store1).IsSuccess);
        }

        [Test]
        public void ReadStoreBasketsTest()
        {
            SaveUserCartTest();
            Assert.True(df.ResetConnection().IsSuccess);
            var storeRes = df.ReadStore(alenbyStore.StoreName);
            Assert.True(storeRes.IsSuccess, storeRes.Error);
            Assert.True(storeRes.Value.GetAllBaskets().Count == 1);
        }
        [Test]
        public void ReadUserBasketsTest()
        {
            SaveUserCartTest();
            Assert.True(df.ResetConnection().IsSuccess);
            var userRes = df.ReadUser(jaren.Username);
            Assert.True(userRes.IsSuccess, userRes.Error);
            Assert.True(userRes.Value._myCart._baskets.Count == 2);
        }
        
        [Test]
        public void SaveUserPurchaseTest()
        {
            var trans = df.BeginTransaction();
            SaveUserCartTest();
            var purchaseRes=jaren.BuyWholeCart(new PaymentInfo(
                userName:jaren.Username,
                idNumber:jaren.MemberInfo.Id,
                creditCardNumber:"1234567789",
                creditCardExpirationDate:"03-01-22",
                threeDigitsOnBackOfCard:"123",
                fullAddress:"TLV"
            ));
            Assert.True(df.UpdateUser(ja).IsSuccess);
            Assert.True(df.UpdateUser(jaren).IsSuccess);
            Assert.True(df.UpdateStore(alenbyStore).IsSuccess);
            Assert.True(df.UpdateStore(store1).IsSuccess);
            df.CommitTransaction(trans);
        }
        
        [Test]
        public void SaveUserPurchaseTest2()
        {
            Assert.True(df.SaveUser(ja).IsSuccess);
            Assert.True(df.SaveUser(jaren).IsSuccess);
            ja.OpenStore(alenbyStore);
            Assert.True(df.SaveStore(alenbyStore).IsSuccess);

            
            var pstation = new ItemInfo(100, "Playstation4", alenbyStore.GetStoreName(), "Tech",
                new List<string>(), 3500);
            var addItemRes= alenbyStore.AddItemToStore(pstation,ja);
           
            
            var resGetItem=alenbyStore.GetItem(pstation);
            var showItem = resGetItem.Value.ShowItem();
            showItem.amount = 5;
            jaren.AddItemToCart(showItem);
            
            Assert.True(df.UpdateUser(ja).IsSuccess);
            Assert.True(df.UpdateUser(jaren).IsSuccess);
            Assert.True(df.UpdateStore(alenbyStore).IsSuccess);
            Console.WriteLine($"JAREN CART HASH: {jaren._myCart.GetHashCode()}");
            var purchaseRes=jaren.BuyWholeCart(new PaymentInfo(
                userName:jaren.Username,
                idNumber:jaren.MemberInfo.Id,
                creditCardNumber:"1234567789",
                creditCardExpirationDate:"03-01-22",
                threeDigitsOnBackOfCard:"123",
                fullAddress:"TLV"
            ));
            
            Console.WriteLine($"JAREN CART HASH: {jaren._myCart.GetHashCode()}");
            // Assert.True(df.UpdateUser(ja).IsSuccess);
            Assert.True(df.UpdateUser(jaren).IsSuccess);

        }
        
        [Test]
        public void ReadUserPurchaseTest()
        {
            SaveUserPurchaseTest();
            Assert.True(df.ResetConnection().IsSuccess);
            var username = jaren.Username;
            var userRes = df.ReadUser(username);
            Assert.True(userRes.IsSuccess);
            Assert.True(userRes.Value._transHistory._purchases.Count == 2);
            
            var storeRes = df.ReadStore(alenbyStore.StoreName);
            Assert.True(storeRes.IsSuccess);
            Assert.True(storeRes.Value._transactionHistory._history.Count == 1);
            
            storeRes = df.ReadStore(store1.StoreName);
            Assert.True(storeRes.IsSuccess);
            Assert.True(storeRes.Value._transactionHistory._history.Count == 1);
        }

        [Test]
        public void SaveStorePurchaseTest()
        {
            var payInstance = PaymentProxy._adapter;
            var supplyInstance = SupplyProxy._adapter;
            Assert.True(df.SaveUser(ja).IsSuccess);
            Assert.True(df.SaveUser(jaren).IsSuccess);
            Assert.True(df.SaveUser(dillon).IsSuccess);
            Assert.True(df.SaveStore(alenbyStore).IsSuccess);
            
            ja.OpenStore(alenbyStore);
            Assert.True(df.UpdateUser(ja).IsSuccess);
            var pstation4 = new ItemInfo(3000000, "Playstation4", alenbyStore.GetStoreName(), "Gaming",
                new List<string>(), 2500);
            var pstation5 = new ItemInfo(100, "Playstation5", alenbyStore.GetStoreName(), "Gaming",
                new List<string>(), 4000);
            alenbyStore.AddItemToStore(pstation4,ja);
            alenbyStore.AddItemToStore(pstation5,ja);
            ja.AppointUserToOwner(alenbyStore, jaren);
            jaren.AppointUserToManager(alenbyStore, dillon);

            var resAssign=pstation4.AssignStoreToItem(alenbyStore);
            pstation4.amount = 1;
            Assert.True(resAssign.IsSuccess,resAssign.Error);
            var updateRes = df.UpdateStore(alenbyStore);
            Assert.True(updateRes.IsSuccess,updateRes.Error);
            
            for (int i = 0; i < 1; i++)
            {
                var addToCartRes = ja.AddItemToCart(pstation4);
                Assert.True(addToCartRes.IsSuccess, addToCartRes.Error);
                var resBuy = ja.BuyWholeCart(new PaymentInfo(ja.Username, "111", "111", "11/11", "111", "abc"));
                Assert.True(resBuy.IsSuccess, resBuy.Error);
            }
            Assert.True(df.UpdateStore(alenbyStore).IsSuccess);
            
        }

        [Test]
        public void ReadStorePurchaseTest()
        {
            SaveStorePurchaseTest();
            Assert.True(df.ResetConnection().IsSuccess);
            var storeName = alenbyStore.StoreName;
            alenbyStore = null;
            var res = df.ReadStore(storeName);
            Assert.True(res.IsSuccess, res.Error);
        }

        [Test]
        public void TheReadStoreTest()
        {
            SaveUserWithStoreTest();
            Assert.True(df.ResetConnection().IsSuccess);
            
            var storeRes = df.ReadStore(store3.StoreName);
            Assert.True(storeRes.IsSuccess,storeRes.Error);
            Assert.True(storeRes.Value._ownersAppointments.Count == 2);
            
            storeRes = df.ReadStore(store1.StoreName);
            Assert.True(storeRes.IsSuccess,storeRes.Error);
            Assert.True(storeRes.Value._managersAppointments.Count == 1);
            Assert.True(storeRes.Value._managersAppointments[0].User.Username == jaren.Username);
            
            var userRes = df.ReadUser(jaren.Username);
            Assert.True(userRes.IsSuccess,userRes.Error);
            Assert.True(userRes.Value.StoresOwned.Count == 2);
            Assert.True(userRes.Value.StoresOwned.ContainsKey(store3.StoreName));
            Assert.True(userRes.Value.StoresOwned.ContainsKey(store4.StoreName));
            Assert.True(userRes.Value.StoresManaged.Count == 2);
            Assert.True(userRes.Value.StoresManaged.ContainsKey(store1.StoreName));
            Assert.True(userRes.Value.StoresManaged.ContainsKey(store2.StoreName));
            
            userRes = df.ReadUser(ja.Username);
            Assert.True(userRes.IsSuccess,userRes.Error);
            Assert.True(userRes.Value.AppointedOwners.Count == 2);
            Assert.True(userRes.Value.AppointedOwners.ContainsKey(store3.StoreName));
            Assert.True(userRes.Value.AppointedOwners.KeyToValue(store3.StoreName).Count == 1);
            Assert.True(userRes.Value.AppointedOwners.ContainsKey(store4.StoreName));
            Assert.True(userRes.Value.AppointedOwners.KeyToValue(store4.StoreName).Count == 1);
            Assert.True(userRes.Value.AppointedManagers.Count == 2);
            Assert.True(userRes.Value.AppointedManagers.ContainsKey(store1.StoreName));
            Assert.True(userRes.Value.AppointedManagers.KeyToValue(store1.StoreName).Count == 1);
            Assert.True(userRes.Value.AppointedManagers.ContainsKey(store2.StoreName));
            Assert.True(userRes.Value.AppointedManagers.KeyToValue(store2.StoreName).Count == 1);
        }

        [Test]
        public void NoDuplicateInstancesTest()
        {
            Assert.True(df.SaveLocalUser(ja).IsSuccess);
            // Assert.True(df.SaveUser(ja).IsSuccess);

            ja.OpenStore(store1);
            ja.OpenStore(store2);
            ja.AppointUserToManager(store1,jaren);
            ja.AppointUserToManager(store2,jaren);

            var res = df.ReadLocalUser(ja.Username);
            Assert.True(df.ResetConnection().IsSuccess);
            // var res = df.ReadUser(ja.Username);
            
            Assert.True(res.IsSuccess);
            Console.WriteLine($"Equals:\n\t{res.Value.GetHashCode()}\n\t{ja.GetHashCode()}");
            Assert.True(res.Value.GetHashCode() == ja.GetHashCode());
        }


        [Test]
        public void AddRemoveAddOwnerTest()
        {
            var trans = df.BeginTransaction();
            Assert.True(df.SaveUser(ja).IsSuccess);
            Assert.True(df.SaveUser(jaren).IsSuccess);
            Assert.True(df.SaveStore(store1).IsSuccess);
            ja.OpenStore(store1);
            ja.AppointUserToOwner(store1,jaren);
            Assert.True(df.UpdateUser(ja).IsSuccess);
            Assert.True(df.UpdateUser(jaren).IsSuccess);
            Assert.True(df.UpdateStore(store1).IsSuccess);
            ja.RemoveOwnerFromStore(store1,jaren);
            Assert.True(df.UpdateUser(ja).IsSuccess);
            Assert.True(df.UpdateUser(jaren).IsSuccess);
            Assert.True(df.UpdateStore(store1).IsSuccess);

            df.CommitTransaction(trans);
            
            Assert.True(df.ResetConnection().IsSuccess);

            var jaRes = df.ReadUser(ja.Username);
            Assert.True(jaRes.IsSuccess);
            var ja_2 = jaRes.Value;
            var jarenRes = df.ReadUser(jaren.Username);
            Assert.True(jarenRes.IsSuccess);
            var jaren_2 = jarenRes.Value;
            var storeRes = df.ReadStore(store1.StoreName);
            Assert.True(storeRes.IsSuccess);
            var store_2 = storeRes.Value;
            
            ja_2.AppointUserToOwner(store_2,jaren_2);
            Assert.True(df.UpdateUser(ja_2).IsSuccess);
            Assert.True(df.UpdateUser(jaren_2).IsSuccess);
            Assert.True(df.UpdateStore(store_2).IsSuccess);



        }

        [Test]
        public void SaveStoreWithoutGuestCartTest()
        {
            // Assert.Warn("Not Implemented");
            var guest = new User("guest_1");
            Assert.True(df.SaveUser(ja).IsSuccess);
            Assert.True(df.SaveStore(store1).IsSuccess);
            ja.OpenStore(store1);
            
            var pstation = new ItemInfo(100, "Playstation4", store1.GetStoreName(), "Tech",
                new List<string>(), 3500);
            var addItemRes= store1.AddItemToStore(pstation,ja);
           
            
            var resGetItem=store1.GetItem(pstation);
            var showItem = resGetItem.Value.ShowItem();
            showItem.amount = 5;
            guest.AddItemToCart(showItem);
            
            Assert.True(df.UpdateUser(ja).IsSuccess);
            Assert.True(df.UpdateStore(store1).IsSuccess);

        }
        
    }
}