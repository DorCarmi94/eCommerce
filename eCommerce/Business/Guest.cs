using System;
using System.Collections.Generic;

using eCommerce.Common;
using eCommerce.Statistics;

namespace eCommerce.Business
{
    public sealed class Guest : UserToSystemState
    {  
        private static readonly Guest state = new Guest();  
        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit  
        static Guest(){}  
        private Guest(){}  
        public static Guest State  
        {  
            get  
            {  
                return state;  
            }  
        }

        public Result Login(User user,UserToSystemState systemState, MemberData memberData)
        {
            return user.Login(this, systemState, memberData);
        }

        public Result Logout(User user, string toGuestName)
        {
            return Result.Fail("Illegal action for guest (Logout).");
        }

        public Result OpenStore(User user, Store store)
        {
            return Result.Fail("Illegal action for guest (Open-Store).");
        }

        public Result<List<string>> GetStoreIds(User user)
        {
            return Result.Fail<List<string>>("Illegal action for guest (GetDiscount-Store-Ids).");
        }

        public Result<IList<string>> GetManagedStoreIds(User user)
        {
            return Result.Fail<IList<string>>("Illegal action for guest (Get-Managed-Stores-Ids).");
        }

        public Result AppointUserToOwner(User user, Store store, User otherUser)
        {
            return Result.Fail("Illegal action for guest (Appoint-User).");
        }

        public Result AppointUserToManager(User user, Store store, User otherUser)
        {
            return Result.Fail("Illegal action for guest (Appoint-User).");
        }

        public Result<OwnerAppointment> MakeOwner(User user, Store store)
        {
            return Result.Fail<OwnerAppointment>("Illegal action for guest (Make-Owner).");
        }

        public Result<ManagerAppointment> MakeManager(User user, Store store)
        {
            return Result.Fail<ManagerAppointment>("Illegal action for guest (Make-Manager).");
        }

        public Result AddPermissionsToManager(User user, Store store, User otherUser, StorePermission permission)
        {
            return Result.Fail("Illegal action for guest (Give-Store-Permissions-To-User).");
        }

        public Result RemovePermissionsToManager(User user, Store store, User otherUser, StorePermission permission)
        {
            return Result.Fail("Illegal action for guest (Take-Store-Permissions-From-User).");
        }

        public Result UpdatePermissionsToManager(User user, Store store, User user1, IList<StorePermission> permissions)
        {
            return Result.Fail("Illegal action for guest (Update-Store-Permissions-From-User).");
        }

        public Result<IList<PurchaseRecord>> GetUserPurchaseHistory(User user)
        {
            return Result.Fail<IList<PurchaseRecord>>("Illegal action for guest (get-purchase-history).");
            
        }

        public Result<IList<PurchaseRecord>> GetUserPurchaseHistory(User user, User otherUser)
        {
            return Result.Fail<IList<PurchaseRecord>>("Illegal action for guest (get-other-user's-purchase-history).");
        }

        public Result<IList<PurchaseRecord>> GetStorePurchaseHistory(User user, Store store)
        {
            return Result.Fail<IList<PurchaseRecord>>("Illegal action for guest (get-Store-purchase-history).");
        }


        public Result HasPermission(User user, Store store, StorePermission storePermission)
        {
            return Result.Fail("Guest has no permissions in a store.");
        }

        public Result EnterRecordToHistory(User user, PurchaseRecord record)
        {
            return Result.Ok();
        }

        public Result<IList<User>> GetAllStoreStakeholders(User user, Store store)
        {
            return Result.Fail<IList<User>>("Guest can not view stores stakeholders.");
        }

        public Result RemoveOwnerFromStore(User user, Store store, User otherUser)
        {
            return Result.Fail<IList<User>>("Guest can not remove an owner from a store.");
        }

        public Result<OwnerAppointment> RemoveOwner(User user, Store store)
        {
            return Result.Fail<OwnerAppointment>("Guest can not be an owner of a store.");
        }
        
        public Result<ManagerAppointment> RemoveManager(User user, Store store)
        {
            return Result.Fail<ManagerAppointment>("Guest can not be a manager of a store.");
        }

        public Result AnnexStakeholders(User user, Store store, IList<OwnerAppointment> owners, IList<ManagerAppointment> managers)
        {
            return Result.Fail("Guest can not be an founder of a store.");
        }

        public string GetRole()
        {
            return "Guest";
        }

        public Result<LoginDateStat> GetLoginStats(DateTime date)
        {
            return Result.Fail<LoginDateStat>("Guest can not get stats");
        }
    }  
}