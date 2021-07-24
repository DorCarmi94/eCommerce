using System.Collections;
using System.Collections.Generic;
using eCommerce.Business;


namespace eCommerce.Service
{
    public class SCart
    {
        public IList<SBasket> Baskets { get; }
        public string CartHolderID { get; }

        public SCart(IList<SBasket> baskets, string CartHolder)
        {
            Baskets = baskets;
            this.CartHolderID = CartHolder;
        }
        
        internal SCart(IList<Basket> baskets, string CartHolder)
        {
            Baskets = new List<SBasket>();
            CartHolderID = CartHolder;
            foreach (var basket in baskets)
            {
                var newItems = new List<SItem>();
                var items = basket.GetAllItems().Value;
                foreach (var item in items)
                {
                    newItems.Add(new SItem(item.ItemName, item.StoreName, item.Amount, 
                        item.Category, item.KeyWords, item.PricePerUnit));
                }
                
                Baskets.Add(new SBasket(basket.GetStoreName() , newItems, basket.GetTotalPrice().Value));
            }
        }
    }
}