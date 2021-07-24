using System;
using System.Collections.Generic;
using eCommerce.Business.Repositories;
using eCommerce.Common;
using eCommerce.Statistics;

namespace eCommerce.Business
{
    public class Member : UserToSystemState
    {
        private static readonly Member state = new Member();  
        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit  
        static Member(){}
        protected Member(){}  
        public static Member State  
        {  
            get  
            {  
                return state;  
            }  
        }

        public Result Login(User user,UserToSystemState systemState, MemberData memberData)
        {
            return Result.Fail("Illegal action for member (login).");
        }

        public Result Logout(User user, string toGuestName)
        {
            return user.Logout(this, toGuestName);
        }

        public Result OpenStore(User user,Store store)
        {
            return user.OpenStore(this, store);
        }

        public Result<List<string>> GetStoreIds(User user)
        {
            List<string> storeIds = ListHelper<string, OwnerAppointment>.Keys(user.StoresOwned);
            // List<string> storeIds = new List<string>();
            // foreach (var store in ListHelper<string, OwnerAppointment>.Keys(user.StoresOwned))
            // {
            //     storeIds.Add(store.GetStoreName());
            // }

            return Result.Ok(storeIds);
        }

        public Result<IList<string>> GetManagedStoreIds(User user)
        {
            List<string> storeIds = new List<string>();
            foreach (var storename in user.StoresManaged.Keys())
            {
                storeIds.Add(storename);
            }

            return Result.Ok<IList<string>>(storeIds);
        }


        public Result AppointUserToOwner(User user,Store store, User otherUser)
        {
            return user.AppointUserToOwner(this, store, otherUser);
        }

        public Result AppointUserToManager(User user, Store store, User otherUser)
        {
            return user.AppointUserToManager(this, store, otherUser);
        }

        public Result<OwnerAppointment> MakeOwner(User user, Store store)
        {
            return user.MakeOwner(this, store);
        }

        public Result<ManagerAppointment> MakeManager(User user, Store store)
        {
            return user.MakeManager(this, store);
        }

       /* public Result AddPermissionsToManager(User user, Store store, User otherUser, StorePermission permission)
        {
            return user.AddPermissionsToManager(this,store,otherUser,permission);
        }

        public Result RemovePermissionsToManager(User user, Store store, User otherUser, StorePermission permission)
        {
            return user.RemovePermissionsToManager(this,store, otherUser,permission);
        }*/

        public Result UpdatePermissionsToManager(User user, Store store, User otherUser, IList<StorePermission> permissions)
        {
            return user.UpdatePermissionsToManager(this,store, otherUser,permissions);
        }

        public Result<IList<PurchaseRecord>> GetUserPurchaseHistory(User user)
        {
            return user.GetUserHistory();
        }

        public virtual Result<IList<PurchaseRecord>> GetUserPurchaseHistory(User user, User otherUser)
        {
            return Result.Fail<IList<PurchaseRecord>>("Illegal action for member (GetDiscount-Other-User-History)");
        }

        public Result<IList<PurchaseRecord>> GetStorePurchaseHistory(User user, Store store)
        {
            return store.GetPurchaseHistory(user);
        }

        public virtual Result HasPermission(User user, Store store, StorePermission storePermission)
        {
            return user.HasPermission(this, store,storePermission);
        }

        public Result EnterRecordToHistory(User user, PurchaseRecord record)
        {
            return user.EnterRecordToHistory(this, record);
        }

        public Result<IList<User>> GetAllStoreStakeholders(User user, Store store)
        {
            return user.GetAllStoreStakeholders(this, store);
        }

        public Result RemoveOwnerFromStore(User user, Store store, User otherUser)
        {
            return user.RemoveOwnerFromStore(this, store,otherUser);
        }

        public Result<OwnerAppointment> RemoveOwner(User user, Store store)
        {
            return user.RemoveOwner(this, store);
        }
        
        public Result<ManagerAppointment> RemoveManager(User user, Store store)
        {
            return user.RemoveManager(this, store);
        }

        public Result AnnexStakeholders(User user, Store store, IList<OwnerAppointment> owners, IList<ManagerAppointment> managers)
        {
            return user.AnnexStakeholders(this, store, owners, managers);
        }

        public virtual Result<LoginDateStat> GetLoginStats(DateTime date)
        {
            return Result.Fail<LoginDateStat>("Guest can not get stats");
        }
        
        public virtual string GetRole()
        {
            return "Member";
        }
    }
}