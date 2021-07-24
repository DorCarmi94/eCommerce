using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eCommerce.Service;

namespace eCommerce.Business
{
    public class PurchaseRecord
    {
        
        private string _storeInfo;
        private string _storeId;
        private BasketInfo _basketInfo;
        private DateTime _dateTime;
        private string _username;
        public PurchaseRecord(Store store, IBasket basket, DateTime now)
        {
            this._storeInfo = store._storeName;
            this._storeId = store.StoreName;
            this._basketInfo = new BasketInfo(basket);
            this._username = basket.GetCart().GetUser().Username;
            this._dateTime = now;
        }

        public string GetStoreInfo()
        {
            return _storeInfo;
        }

        public BasketInfo GetBasketInfo()
        {
            return _basketInfo;
        }

        public DateTime GetDate()
        {
            return _dateTime;
        }



        public PurchaseRecord()
        {
            
        }

        public string StoreId { get => _storeId; set => _storeId = value; }
        public string Username { get => _username; set => _username = value; }
        public string StoreInfo { get => _storeInfo; set => _storeInfo = value; }
        public BasketInfo BasketInfo { get => _basketInfo; set => _basketInfo = value;}
        public DateTime PurchaseTime { get => _dateTime; set => _dateTime = value; }
    }
}