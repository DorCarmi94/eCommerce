using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce;
using eCommerce.Business;
using eCommerce.Business.Repositories;
using eCommerce.Common;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Tests.Business.Mokups;

namespace Tests.Business.UserTests
{
    public class UserTest
    {
        private User ja;
        private mokStore store1;
        private User tempguy;
        private mokStore store2;
        private User jaren;
        private User GM;
        private User dillon;


        [SetUp]
        public void Setup()
        {
            ja = new User(new MemberInfo("Ja Morant", "ja@mail.com", "Ja", DateTime.Now, "Memphis"));
            store1= new mokStore("BasketBall stuff.. buy here", ja);
            tempguy = new User("10-Day-Contract guy");
            store2 = new mokStore("More(!) BasketBall stuff.. buy here", ja);
            jaren = new User(new MemberInfo("Jaren Jackson Jr.", "jjj@mail.com", "Jaren", DateTime.Now, "Memphis"));
            GM = new User(Admin.State,new MemberInfo("GM", "gm@mail.com", "GM", DateTime.Now, "Memphis"));
            dillon = new User(new MemberInfo("Dillon Brooks","db@mail.com","Dillon",DateTime.Now, "Memphis"));
            ja.OpenStore(store1);

        }

        //possible Integration test too
        [Test]
        public void TestOpenStore_Pass()
        {
            Assert.True(ja.HasPermission(store1,StorePermission.ControlStaffPermission).IsSuccess);
            Assert.True(ja.StoresOwned.ContainsKey(store1.StoreName));
            Assert.True(ja.StoresOwned.KeyToValue(store1.StoreName).User.Equals(ja));
        } 
        [Test]
        public void TestOpenStore_Fail()
        {
            Assert.False(tempguy.OpenStore(store2).IsSuccess);
            Assert.False(tempguy.HasPermission(store2,StorePermission.ControlStaffPermission).IsSuccess);
        }
        [Test]
        public void TestOpenStore_Concurrent()
        {
            var store3 = new mokStore("New Location(!) BasketBall stuff.. buy here", ja);
            var store4 = new mokStore("Old Location Again(!) BasketBall stuff.. buy here", ja);
            Task<Result> task1 = new Task<Result>(() => { return jaren.OpenStore(store2); });
            Task<Result> task2 = new Task<Result>(() => { return jaren.OpenStore(store2); });
            task1.Start();
            task2.Start();
            Assert.True(task1.Result.IsSuccess != task2.Result.IsSuccess);
            Task<Result> task3 = new Task<Result>(() => { return jaren.OpenStore(store3); });
            Task<Result> task4 = new Task<Result>(() => { return jaren.OpenStore(store4); });
            task3.Start();
            task4.Start();
            Assert.True(task3.Result.IsSuccess);
            Assert.True(task4.Result.IsSuccess);
        }
        
        [Test]
        public void TestHasPermissions_Pass()
        {
            Assert.True(ja.HasPermission(store1,StorePermission.ControlStaffPermission).IsSuccess);
            ja.AppointUserToManager(store1, jaren);
            ja.UpdatePermissionsToManager(store1,jaren, new List<StorePermission>(new [] {StorePermission.ChangeItemPrice,StorePermission.ControlStaffPermission}));
            Assert.True(jaren.HasPermission(store1, StorePermission.ChangeItemPrice).IsSuccess);
            Assert.True(jaren.HasPermission(store1, StorePermission.ControlStaffPermission).IsSuccess);
        }
        [Test]
        public void TestHasPermissions_Fail()
        {
            Assert.False(jaren.HasPermission(store1,StorePermission.ControlStaffPermission).IsSuccess);
            ja.AppointUserToManager(store1, jaren);
            ja.UpdatePermissionsToManager(store1,jaren, new List<StorePermission>(new [] {StorePermission.ChangeItemPrice,StorePermission.ControlStaffPermission}));
            Assert.False(jaren.HasPermission(store1, StorePermission.EditItemDetails).IsSuccess);
        }
        [Test]
        public void TestHasPermissions_Concurrent()
        {
            ja.AppointUserToManager(store1,jaren);
            var permissionList = new List<StorePermission>(new[] {StorePermission.ChangeItemPrice, StorePermission.ControlStaffPermission});
            var taskAdd1 = new Task<Result>(() => ja.UpdatePermissionsToManager(store1,jaren,permissionList));
            var taskAdd2 = new Task<Result>(() => ja.UpdatePermissionsToManager(store1,jaren,permissionList));
            var taskAdd3 = new Task<Result>(() => ja.UpdatePermissionsToManager(store1,jaren,permissionList));
            taskAdd1.Start();
            taskAdd2.Start();
            taskAdd3.Start();
            Assert.True(taskAdd1.Result.IsSuccess);
            Assert.True(taskAdd2.Result.IsSuccess);
            Assert.True(taskAdd3.Result.IsSuccess);
            Assert.True(jaren.HasPermission(store1, StorePermission.ChangeItemPrice).IsSuccess);
            Assert.True(jaren.HasPermission(store1, StorePermission.ControlStaffPermission).IsSuccess);
        }
        [Test]
        public void TestAppointUserToManager_Pass()
        {
            Assert.True(ja.AppointUserToManager(store1, jaren).IsSuccess);
            Assert.True(ja.AppointedManagers != null);
            Assert.True(ja.AppointedManagers.ContainsKey(store1.StoreName));
            Assert.True(ja.AppointedManagers.KeyToValue(store1.StoreName).FirstOrDefault(m => m.User == jaren)!= null);
        }
        [Test]
        public void TestAppointUserToManager_Fail()
        {
            Assert.False(jaren.AppointUserToManager(store1, ja).IsSuccess);
            ja.AppointUserToManager(store1, jaren);
            Assert.False(ja.AppointUserToManager(store1,jaren).IsSuccess);
            Assert.False(ja.AppointUserToManager(store1,tempguy).IsSuccess);
        }
        [Test]
        public void TestAppointUserToManager_Concurrent()
        {
            ja.AppointUserToOwner(store1, jaren);
            var task1 = new Task<Result>( ()=> ja.AppointUserToManager(store1, dillon));
            var task2 = new Task<Result>( ()=> jaren.AppointUserToManager(store1, dillon));
            task1.Start();
            task2.Start();
            Assert.True(task1.Result.IsSuccess != task2.Result.IsSuccess);
            Assert.True(ja.AppointedManagers.ContainsKey(store1.StoreName) != 
                        jaren.AppointedManagers.ContainsKey(store1.StoreName));
        }
        [Test]
        public void TestAppointUserToOwner_Pass()
        {
            Assert.True(ja.AppointUserToOwner(store1, jaren).IsSuccess);
            Assert.True(ja.AppointedOwners != null);
            Assert.True(ja.AppointedOwners.ContainsKey(store1.StoreName));
            Assert.True(ja.AppointedOwners.KeyToValue(store1.StoreName).FirstOrDefault(m => m.User == jaren)!= null);
        }
        [Test]
        public void TestAppointUserToOwner_Fail()
        {
            Assert.False(jaren.AppointUserToOwner(store1, ja).IsSuccess);
            ja.AppointUserToOwner(store1, jaren);
            Assert.False(ja.AppointUserToOwner(store1,jaren).IsSuccess);
            Assert.False(ja.AppointUserToOwner(store1,tempguy).IsSuccess);
        }
        [Test]
        public void TestAppointUserToOwner_Concurrent()
        {
            ja.AppointUserToOwner(store1, jaren);
            var task1 = new Task<Result>( ()=> ja.AppointUserToOwner(store1, dillon));
            var task2 = new Task<Result>( ()=> jaren.AppointUserToOwner(store1, dillon));
            task1.Start();
            task2.Start();
            Assert.True(task1.Result.IsSuccess != task2.Result.IsSuccess);
            int OwnersCount = 0;
            OwnersCount += ja.AppointedOwners.KeyToValue(store1.StoreName).Count;
            if (jaren.AppointedOwners.ContainsKey(store1.StoreName))
            {
                OwnersCount += jaren.AppointedOwners.KeyToValue(store1.StoreName).Count;
            }
            Assert.True(OwnersCount == 2);
        }
        [Test]
        public void TestUpdatePermissionsToManager_Pass()
        {
            Assert.True(ja.AppointUserToManager(store1,jaren).IsSuccess);
            Assert.True(ja.UpdatePermissionsToManager(store1,jaren,new List<StorePermission>(new [] {StorePermission.AddItemToStore})).IsSuccess);
            Assert.True(jaren.StoresManaged.KeyToValue(store1.StoreName).HasPermission(StorePermission.AddItemToStore).IsSuccess);
        }
        [Test]
        public void TestUpdatePermissionsToManager_Fail()
        {
            Assert.False(ja.UpdatePermissionsToManager(store1,jaren,new List<StorePermission>(new [] {StorePermission.AddItemToStore})).IsSuccess);
            ja.AppointUserToManager(store1,jaren);
            Assert.False(ja.UpdatePermissionsToManager(store1,jaren,new List<StorePermission>()).IsSuccess);
        }
        [Test]
        public void TestUpdatePermissionsToManager_Concurrent()
        {
            ja.AppointUserToManager(store1,jaren);
            var task1 = new Task<Result>(()=> ja.UpdatePermissionsToManager(store1,jaren,new List<StorePermission>(new [] {StorePermission.AddItemToStore, StorePermission.ChangeItemPrice})));
            var task2 = new Task<Result>(()=> ja.UpdatePermissionsToManager(store1,jaren,new List<StorePermission>(new [] {StorePermission.AddItemToStore, StorePermission.ChangeItemStrategy})));
            task1.Start();
            task2.Start();
            Assert.True(task1.Result.IsSuccess);
            Assert.True(task2.Result.IsSuccess);
            Assert.True(jaren.StoresManaged.KeyToValue(store1.StoreName).HasPermission(StorePermission.AddItemToStore).IsSuccess);
            Assert.True(jaren.StoresManaged.KeyToValue(store1.storeName).HasPermission(StorePermission.ChangeItemPrice).IsSuccess != jaren.StoresManaged.KeyToValue(store1.StoreName).HasPermission(StorePermission.ChangeItemStrategy).IsSuccess);
        }
        [Test]
        public void TestUserPurchaseHistory_Pass()
        {
            var result = ja.GetUserPurchaseHistory();
            Assert.True(result.IsSuccess);
            var records = result.Value;
            Assert.True(records.Count == 0);
            var purchaseRecord = new PurchaseRecord(store1,new MokBasket(ja.GetCartInfo().Value,store1),DateTime.Now);
            Assert.True(ja.EnterRecordToHistory(purchaseRecord).IsSuccess);
            result = ja.GetUserPurchaseHistory();
            Assert.True(result.IsSuccess);
            Assert.True(result.Value.Contains(purchaseRecord));
        } 

        
        [Test]
        public void TestUserPurchaseHistory_Concurrent()
        {
            var purchaseRecord1 = new PurchaseRecord(store1,new MokBasket(ja.GetCartInfo().Value,store1),DateTime.Now);
            var purchaseRecord2 = new PurchaseRecord(store1,new MokBasket(ja.GetCartInfo().Value,store2),DateTime.Now);
            var task1 =  new Task<Result>(()=>ja.EnterRecordToHistory(purchaseRecord1));
            var task2 =  new Task<Result>(()=>ja.EnterRecordToHistory(purchaseRecord2));
            task1.Start();
            task2.Start();
            Assert.True(task1.Result.IsSuccess);
            Assert.True(task2.Result.IsSuccess);
            var result = ja.GetUserPurchaseHistory();
            Assert.True(result.IsSuccess);
            Assert.True(result.Value.Contains(purchaseRecord1));
            Assert.True(result.Value.Contains(purchaseRecord2));
        } 
        [Test]
        public void TestAdminGetHistory_Pass()
        {
            var purchaseRecord = new PurchaseRecord(store1,new MokBasket(ja.GetCartInfo().Value,store1),DateTime.Now);
            ja.EnterRecordToHistory(purchaseRecord);
            var result = GM.GetUserPurchaseHistory(ja);
            Assert.True(result.IsSuccess);
            var records = result.Value;
            Assert.True(records.Count == 1);
        }
        [Test]
        public void TestAdminGetHistory_Fail()
        {
            Assert.False(GM.GetUserPurchaseHistory(tempguy).IsSuccess);
            Assert.False(ja.GetUserPurchaseHistory(GM).IsSuccess);
        }
        [Test]
        public void TestAdminGetHistory_Concurrent()
        {
            var purchaseRecord = new PurchaseRecord(store1,new MokBasket(ja.GetCartInfo().Value,store1),DateTime.Now);
            ja.EnterRecordToHistory(purchaseRecord);
            var VP = new User(Admin.State, new MemberInfo("VPofBballOps","vp@mail.com","Vp", DateTime.Now, "Memphis as well"));
            var task1 =new Task<Result<IList<PurchaseRecord>>>(()=> GM.GetUserPurchaseHistory(ja));
            var task2 =new Task<Result<IList<PurchaseRecord>>>(()=> VP.GetUserPurchaseHistory(ja));
            task1.Start();
            task2.Start();
            Assert.True(task1.Result.IsSuccess);
            Assert.True(task2.Result.IsSuccess);
            Assert.True(task1.Result.Value.Count == task2.Result.Value.Count);
        }

        [Test]
        public void TestRemoveOwnerFromStore_Pass()
        {
            Assert.True(ja.AppointUserToOwner(store1, jaren).IsSuccess);
            Assert.True(jaren.AppointUserToOwner(store1, dillon).IsSuccess);
            Assert.True(ja.RemoveOwnerFromStore(store1, jaren).IsSuccess);
            
            Assert.True(jaren.HasPermission(store1,StorePermission.ControlStaffPermission).IsFailure);
            Assert.True(dillon.HasPermission(store1,StorePermission.ControlStaffPermission).IsFailure);
        }

        [Test]
        public void TestRemoveOwnerFromStore_Fail()
        {
            Assert.False(jaren.RemoveOwnerFromStore(store1, ja).IsSuccess);
            Assert.False(ja.RemoveOwnerFromStore(store1, jaren).IsSuccess);
            ja.AppointUserToOwner(store1, jaren);
            ja.AppointUserToOwner(store1, dillon);
            Assert.False(dillon.RemoveOwnerFromStore(store1,jaren).IsSuccess);
            Assert.False(jaren.RemoveOwnerFromStore(store1,dillon).IsSuccess);
            Assert.False(ja.RemoveOwnerFromStore(store1,tempguy).IsSuccess);
        }
        [Test]
        public void TestRemoveOwnerFromStore_Concurrent()
        {
            ja.AppointUserToOwner(store1, jaren);
            ja.AppointUserToOwner(store1, dillon);
            var task1 = new Task<Result>( ()=> ja.RemoveOwnerFromStore(store1, jaren));
            var task2 = new Task<Result>( ()=> ja.RemoveOwnerFromStore(store1, jaren));
            var task3 = new Task<Result>( ()=> ja.RemoveOwnerFromStore(store1, dillon));
            var task4 = new Task<Result>( ()=> ja.RemoveOwnerFromStore(store1, dillon));
            task1.Start();
            task2.Start();
            task3.Start();
            task4.Start();
            var res1 = task1.Result;
            var res2 = task2.Result;
            var res3 = task3.Result;
            var res4 = task4.Result;
            Assert.True(res1.IsSuccess != res2.IsSuccess);
            Assert.True(res3.IsSuccess != res4.IsSuccess);
            int ownersCount = ja.AppointedOwners.KeyToValue(store1.storeName).Count;
            Assert.True(ownersCount == 0,ownersCount.ToString());
        }

        [Test]
        public void TheABC_OwnersTest()
        {
            //ja = new User(new MemberInfo("Ja Morant", "ja@mail.com", "Ja", DateTime.Now, "Memphis"));
            var Abraham = new User(new MemberInfo("Abraham", "a@a.com", "Ab", DateTime.Now, "TLV"));
            var Bond = new User(new MemberInfo("Bon", "b@a.com", "Bon", DateTime.Now, "TLV"));
            var Chumacher = new User(new MemberInfo("Chumacher", "c@a.com", "Chu", DateTime.Now, "TLV"));
            
            //A->B
            var AbiAndHisSons= new mokStore("AbiAndHisSons", Abraham);
            var openStoreRes=Abraham.OpenStore(AbiAndHisSons);
            Assert.True(openStoreRes.IsSuccess,openStoreRes.Error);
            var resNominateBond=Abraham.AppointUserToOwner(AbiAndHisSons, Bond);
            Assert.True(resNominateBond.IsSuccess,resNominateBond.Error);
            
            //B->C
            var resNominateChu=Bond.AppointUserToOwner(AbiAndHisSons, Chumacher);
            Assert.True(resNominateChu.IsSuccess,resNominateChu.Error);
            
            Assert.True(Bond.StoresOwned.ContainsKey(AbiAndHisSons.storeName));
            Assert.True(Chumacher.StoresOwned.ContainsKey(AbiAndHisSons.storeName));

            var resRemoveBond=Abraham.RemoveOwnerFromStore(AbiAndHisSons, Bond);
            Assert.True(resRemoveBond.IsSuccess,resRemoveBond.Error);

            Assert.False(Bond.StoresOwned.ContainsKey(AbiAndHisSons.storeName));
            Assert.False(Chumacher.StoresOwned.ContainsKey(AbiAndHisSons.storeName));

        }
        
       
    }
}