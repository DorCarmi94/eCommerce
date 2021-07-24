using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace eCommerce.Business
{
    public class BasketInfo
    {
        private IList<ItemInfo> _itemsInBasket;
        private double _totalPrice;
        private String storeName;
        public BasketInfo(IBasket basket)
        {
            _itemsInBasket = new List<ItemInfo>();
            var itemsRes = basket.GetAllItems();
            if (!itemsRes.IsFailure)
            {
                foreach (var itemInf in itemsRes.GetValue())
                {
                   _itemsInBasket.Add(new ItemInfo(itemInf));
                }
            }

            this._totalPrice = basket.GetTotalPrice().GetValue();
            this.storeName = basket.GetStoreName();
        }



        public BasketInfo()
        {
        }

        [Key]
        public Guid Id { get; set; } 
        public IList<ItemInfo> ItemsInBasket
        {
            get => _itemsInBasket;
            set => _itemsInBasket = value;
        }
        public double TotalPrice
        {
            get => _totalPrice;
            set => _totalPrice = value;
        }
    }
}