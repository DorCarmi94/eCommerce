using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business
{
    public interface IBasket
    {
        public Result CalculateBasketPrices();

        public Result AddItemToBasket(User user,ItemInfo item);
        public Result EditItemInBasket(User user,ItemInfo item);
        public Result BuyWholeBasket();
        public Result<double>GetTotalPrice();
        public Result<List<ItemInfo>> GetAllItems();
        public ICart GetCart();
        Result SetTotalPrice();
        public void SetTotalPrice(double newTotalPrice);
        public string GetStoreName();
        public Result<ItemInfo> GetItem(User user, string itemName);
        public BasketInfo GetBasketInfo();

        public Result AddBasketRecords();
        public double GetRegularTotalPrice();
        Result CheckWithStorePolicy();
        Result ReturnAllItemsToStore();
        void Free();
    }
}