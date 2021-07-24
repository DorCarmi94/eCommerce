using System;

using eCommerce.Common;

namespace eCommerce.Business
{
    public class DefaultPurchaseStrategy : PurchaseStrategy
    {
        private Store _store;
        public static Result<PurchaseStrategy> GetPurchaseStrategyByName(PurchaseStrategyName purchaseStrategyName, Store store)
        {
            if (PurchaseStrategyName.Regular == purchaseStrategyName)
            {
                return Result.Ok<PurchaseStrategy>(new  DefaultPurchaseStrategy(store));
            }
            else
            {
                return Result.Fail<PurchaseStrategy>("No such strategy");
            }
        }

        public DefaultPurchaseStrategy(Store store)
        {
            this._store = store;
        }

        public Result<ItemInfo> AquireItems(ItemInfo itemInfo)
        {
            var res = this._store.GetItem(itemInfo);
            if (res.IsFailure)
            {
                return Result.Fail<ItemInfo>(res.Error);
            }
            else
            {
                var resItems=res.GetValue().GetItems(itemInfo.amount);
                return resItems;

            }
        }
        
        public Result AquireItems(IBasket basket)
        {
            return basket.SetTotalPrice();
        }

        public Result BuyItems(IBasket basket)
        {
            return _store.FinishPurchaseOfBasket(basket);
        }

        public PurchaseStrategyName GetStrategyName()
        {
            return PurchaseStrategyName.Regular;
        }

        public Result BuyItems(ItemInfo item)
        {
            return _store.FinishPurchaseOfItems(item);
        }
    }
    
}