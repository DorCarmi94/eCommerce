using System.Collections.Generic;


namespace eCommerce.Service
{
    public class ServiceStore
    {
        public string StoreName { get; }
        public IList<IItem> Items { get; }

        public ServiceStore(string storeName, IList<IItem> items)
        {
            StoreName = storeName;
            Items = items;
        }
    }
}