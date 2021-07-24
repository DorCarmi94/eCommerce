using System.Collections.Generic;
using eCommerce;
using eCommerce.Business;
using eCommerce.Common;

namespace Tests.Business.Mokups
{
    public class mokCart : ICart
    {
        public Result AddItemToCart(User user, ItemInfo item)
        {
            throw new System.NotImplementedException();
        }

        public Result EditCartItem(User user, ItemInfo item)
        {
            throw new System.NotImplementedException();
        }

        public Result<double> CalculatePricesForCart()
        {
            throw new System.NotImplementedException();
        }

        public Result BuyWholeCart(User user, PaymentInfo paymentInfo)
        {
            throw new System.NotImplementedException();
        }

        public Result<PurchaseInfo> BuyWholeCart(User user)
        {
            throw new System.NotImplementedException();
        }

        public bool CheckForCartHolder(User user)
        {
            throw new System.NotImplementedException();
        }

        public List<Basket> GetBaskets()
        {
            throw new System.NotImplementedException();
        }

        public CartInfo GetCartInfo()
        {
            throw new System.NotImplementedException();
        }

        public User GetUser()
        {
            throw new System.NotImplementedException();
        }

        public List<ItemInfo> GetAllItems()
        {
            throw new System.NotImplementedException();
        }

        public void Free()
        {
            throw new System.NotImplementedException();
        }
    }
}