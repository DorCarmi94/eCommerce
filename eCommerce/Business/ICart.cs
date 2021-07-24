using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using eCommerce.Common;

namespace eCommerce.Business
{
    public interface ICart
    {
        public Result AddItemToCart(User user,ItemInfo item);
        
        public Result EditCartItem(User user,ItemInfo item);
        

        public Result<double> CalculatePricesForCart();

        public Result BuyWholeCart(User user, PaymentInfo paymentInfo);
        public bool CheckForCartHolder(User user);

        public List<Basket> GetBaskets();


        public CartInfo GetCartInfo();

        public User GetUser();
        List<ItemInfo> GetAllItems();
        void Free();
    }
}