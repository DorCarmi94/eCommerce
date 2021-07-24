using System.Collections.Generic;

namespace eCommerce.Business
{
    public class CartInfo
    {
        public IList<BasketInfo> baskets;
        public double totalPrice;

        public CartInfo(IList<BasketInfo> baskets, double totalPrice)
        {
            this.baskets = baskets;
            this.totalPrice = totalPrice;
        }
        
    }
}