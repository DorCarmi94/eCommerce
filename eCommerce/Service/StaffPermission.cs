using System.Collections.Generic;
using eCommerce.Business;

namespace eCommerce.Service
{
    public class StaffPermission
    {
        public string UserId { get; }
        public IEnumerable<StorePermission> Permissions { get; }

        public StaffPermission(string userId, IEnumerable<StorePermission> permissions)
        {
            UserId = userId;
            Permissions = permissions;
        }
    }
}