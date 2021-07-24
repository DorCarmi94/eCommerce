using System;
using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Business.Discounts;

using eCommerce.Common;

namespace Tests.Business.Mokups
{
    public class mokStore: Store
    {
        public string storeName { get; set; }
        public Item item;
        public ICart cart;

        public mokStore(string storeName, User founder): base(storeName, founder)
        {
            this.storeName = storeName;
        }

        public override Result<Item> GetItem(ItemInfo item)
        {
            return Result.Ok<Item>(this.item);
        }

        public override Result TryGetItems(ItemInfo item)
        {
            return Result.Ok();
        }

        public override Result AppointNewOwner(User user, OwnerAppointment ownerAppointment)
        {
            Console.WriteLine("MokStore: "+user.Username+" Appointed new Owner: "+ownerAppointment.User.Username);
            return Result.Ok();
        }

        public override Result AppointNewManager(User user, ManagerAppointment managerAppointment)
        {
            Console.WriteLine("MokStore: "+user.Username+" Appointed new Manager: "+managerAppointment.User.Username);
            return Result.Ok();
        }

        public override Result RemoveOwnerFromStore(User theOneWhoFires, User theFierd, OwnerAppointment ownerAppointment)
        {
            Console.WriteLine("MokStore: "+theOneWhoFires.Username+" removed the Owner: "+theFierd.Username);
            return Result.Ok();
        }
        

        public override Result<IList<PurchaseRecord>> GetPurchaseHistory(User user)
        {
            Console.WriteLine("MokStore: Getting Purchase History (its empty..)");
            return Result.Ok<IList<PurchaseRecord>>(new List<PurchaseRecord>());
        }

        public override Result EnterBasketToHistory(IBasket basket)
        {
            Console.WriteLine("MokStore: Entered Basket to History.");
            return  Result.Ok();
        }

        public override string GetStoreName()
        {
            return this.storeName;
        }

        public override bool TryAddNewCartToStore(ICart cart)
        {
            Console.WriteLine("MokStore: Added New Cart To Store");
            return true;
        }

        public override Result ConnectNewBasketToStore(Basket newBasket)
        {
            Console.WriteLine("MockStore: Connected new basket to store");
            return Result.Ok();
        }

        public override bool CheckConnectionToCart(ICart cart)
        {
            return cart == this.cart;
        }

        public Result<double> CheckDiscount(Basket basket)
        {
            throw new System.NotImplementedException();
        }

        public Result CheckWithStorePolicy(IBasket basket, User user)
        {
            throw new NotImplementedException();
        }

        public Result AddDiscountToStore(User user, DiscountInfoNode infoNode)
        {
            throw new NotImplementedException();
        }

        public Result AddRuleToStorePolicy(User user, RuleInfoNode ruleInfoNode)
        {
            throw new NotImplementedException();
        }

        public Result<IList<RuleInfoNode>> GetStorePolicy(User user)
        {
            throw new NotImplementedException();
        }

        public Result<IList<DiscountInfoNode>> GetStoreDiscounts(User user)
        {
            throw new NotImplementedException();
        }

        public Result ResetStorePolicy(User user)
        {
            throw new NotImplementedException();
        }

        public Result ResetStoreDiscount(User user)
        {
            throw new NotImplementedException();
        }

        public Result ReturnItemsToStore(ItemInfo itemInfo)
        {
            throw new NotImplementedException();
        }

        public void FreeBasket(Basket basket)
        {
            throw new NotImplementedException();
        }

        public Result AddPurchaseStrategyToStore(User user, PurchaseStrategyName purchaseStrategy)
        {
            throw new NotImplementedException();
        }

        public Result<IList<PurchaseStrategyName>> GetStorePurchaseStrategy(User user)
        {
            throw new NotImplementedException();
        }

        public Result UpdatePurchaseStrategies(User user, PurchaseStrategyName purchaseStrategy)
        {
            throw new NotImplementedException();
        }

        public Result AddPurchaseStrategyToStoreItem(User user, string storeId, string itemId, PurchaseStrategyName strategyName)
        {
            throw new NotImplementedException();
        }

        public Result RemovePurchaseStrategyToStoreItem(User user, string storeId, string itemId, PurchaseStrategyName strategyName)
        {
            throw new NotImplementedException();
        }

        public Result<IList<PurchaseStrategyName>> GetPurchaseStrategyToStoreItem(User user, string storeId, string itemId, PurchaseStrategyName strategyName)
        {
            throw new NotImplementedException();
        }

        public Result AddDiscountToStore(DiscountInfoNode infoNode)
        {
            throw new NotImplementedException();
        }

        public Result<PurchaseRecord> AddBasketRecordToStore(Basket basket)
        {
            throw new NotImplementedException();
        }
    }
}