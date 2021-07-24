using System;
using eCommerce.Business;

namespace eCommerce.Service
{
    public class SPurchaseRecord
    {
        public string StoreId { get; }

        public string Username { get; }
        public SBasket Basket { get; }
        public DateTime PurchaseTime { get; }

        public SPurchaseRecord(string storeId, string username, SBasket basket, DateTime purchaseTime)
        {
            StoreId = storeId;
            Username = username;
            Basket = basket;
            PurchaseTime = purchaseTime;
        }
        
        internal SPurchaseRecord(PurchaseRecord purchaseRecord, string username)
        {
            Username = username;
            StoreId = purchaseRecord.StoreId;
            Basket = new SBasket(StoreId, purchaseRecord.BasketInfo);
            PurchaseTime = purchaseRecord.PurchaseTime;
        }
    }
}