using eCommerce.Common;

namespace eCommerce.Business
{
    public interface PurchaseStrategy
    {
        public Result<ItemInfo> AquireItems(ItemInfo item);
        public Result AquireItems(IBasket basket);
        public Result BuyItems(ItemInfo item);
        public Result BuyItems(IBasket basket);
        PurchaseStrategyName GetStrategyName();
    }
}