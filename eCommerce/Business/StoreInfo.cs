

using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;

namespace eCommerce.Business
{
    public class StoreInfo
    {
        [Key]
        public string _storeName {get; set; }

        public StoreInfo()
        {
        }

        public StoreInfo(Store store)
        {
            this._storeName = store.GetStoreName();
        }

        public string GetStoreName()
        {
            return this._storeName;
        }

    }
}