using System.Collections.Concurrent;
using System.Collections.Generic;


namespace eCommerce.Business
{
    public class MemberData
    {
        
        public string Username { get => MemberInfo.Username; }
        public ICart Cart { get; }
        public ConcurrentDictionary<Store, bool> StoresFounded{ get; }
        public ConcurrentDictionary<Store, OwnerAppointment> StoresOwned{ get; }
        public ConcurrentDictionary<Store, ManagerAppointment> StoresManaged{ get; }
        public ConcurrentDictionary<Store, IList<OwnerAppointment>> AppointedOwners{ get; }
        public ConcurrentDictionary<Store, IList<ManagerAppointment>> AppointedManagers{ get; }
        public UserTransactionHistory History { get; }
        
        public MemberInfo MemberInfo;

        
        public MemberData(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
        }

        public MemberData(UserTransactionHistory history, MemberInfo memberInfo, ICart cart, ConcurrentDictionary<Store, bool> storesFounded, ConcurrentDictionary<Store, OwnerAppointment> storesOwned, ConcurrentDictionary<Store, ManagerAppointment> storesManaged, ConcurrentDictionary<Store, IList<ManagerAppointment>> appointedManagers, ConcurrentDictionary<Store, IList<OwnerAppointment>> appointedOwners)
        {
            History = history;
            MemberInfo = memberInfo;
            Cart = cart;
            StoresFounded = storesFounded;
            StoresOwned = storesOwned;
            StoresManaged = storesManaged;
            AppointedManagers = appointedManagers;
            AppointedOwners = appointedOwners;
        }

        
        private void test()
        {
        }
    }
}