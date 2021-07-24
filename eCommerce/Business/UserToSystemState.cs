using System;
using System.Collections.Generic;
using eCommerce.Common;
using eCommerce.Statistics;

namespace eCommerce.Business
{
    public interface UserToSystemState
    {
        public Result Login(User user,UserToSystemState systemState, MemberData memberData);
        Result Logout(User user,string toGuestName);
        Result OpenStore(User user,Store store);
        Result<List<string>> GetStoreIds(User user);
        Result<IList<string>> GetManagedStoreIds(User user);
        Result AppointUserToOwner(User user,Store store, User otherUser);
        Result AppointUserToManager(User user,Store store, User otherUser);
        Result<OwnerAppointment> MakeOwner(User user,Store store);
        Result<ManagerAppointment> MakeManager(User user,Store store);
        /*Result AddPermissionsToManager(User user,Store store, User otherUser, StorePermission permission);
        Result RemovePermissionsToManager(User user,Store store, User otherUser, StorePermission permission);*/
        Result UpdatePermissionsToManager(User user, Store store, User otherUser, IList<StorePermission> permissions);
        public Result<IList<PurchaseRecord>> GetUserPurchaseHistory(User user);
        public Result<IList<PurchaseRecord>> GetUserPurchaseHistory(User user, User otherUser);
        public Result<IList<PurchaseRecord>> GetStorePurchaseHistory(User user, Store store);
        Result HasPermission(User user,Store store, StorePermission storePermission);
        Result EnterRecordToHistory(User user, PurchaseRecord record);
        Result<IList<User>> GetAllStoreStakeholders(User user, Store store);
        Result RemoveOwnerFromStore(User user, Store store, User otherUser);
        Result<OwnerAppointment> RemoveOwner(User user, Store store);
        Result<ManagerAppointment> RemoveManager(User user, Store store);
        Result AnnexStakeholders(User user, Store store, IList<OwnerAppointment> owners, IList<ManagerAppointment> managers);
        public  string GetRole();
        Result<LoginDateStat> GetLoginStats(DateTime date);
    }
}