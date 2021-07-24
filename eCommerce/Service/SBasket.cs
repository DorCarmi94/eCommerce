using System.Collections.Generic;
using eCommerce.Business;


namespace eCommerce.Service
{
    public class SBasket
    {
        public string StoreId { get; }
        public IList<SItem> Items { get; }
        public double TotalPrice { get; }

        public SBasket(string storeId, IList<SItem> items, double totalPrice)
        {
            StoreId = storeId;
            Items = items;
            TotalPrice = totalPrice;
        }

        internal SBasket(string storeId, BasketInfo basketInfo)
        {
            StoreId = storeId;
            Items = new List<SItem>();
            var items = basketInfo.ItemsInBasket;
            foreach (var item in items)
            {
                Items.Add(new SItem(item.ItemName, item.StoreName, item.Amount, 
                    item.Category, item.KeyWords, item.PricePerUnit));
            }
            
            TotalPrice = basketInfo.TotalPrice;
        }
    }
}