using System;
using System.Collections.Generic;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.Purchases;

using eCommerce.Common;

namespace eCommerce.Business
{
    public class PurchasePolicy
    {
        private List<Composite> _storeRules;
        private Store _store;

        public PurchasePolicy(Store store)
        {
            this._store = store;
            this._storeRules = new List<Composite>();
        }

        public Result AddRuleToStorePolicy(User user, Composite rule)
        {
            if (user.HasPermission(_store, StorePermission.EditStorePolicy).IsSuccess)
            {
                this._storeRules.Add(rule);
                return Result.Ok();
            }
            else
            {
                return Result.Fail("User doesn't have permission to edit store's policy");
            }
        }

        public Result CheckWithStorePolicy(IBasket basket, User user)
        {
            foreach (var storeRule in _storeRules)
            {
                var dict = storeRule.Check(basket, user);
                foreach (var itemInfo in basket.GetAllItems().Value)
                {
                    if (dict.ContainsKey(itemInfo.name))
                    {
                        return Result.Fail($"Item {itemInfo} in basket has problem with store policy- check user information and item information with store policy");
                    }
                }
            }

            return Result.Ok();
        }

        public Result<IList<RuleInfoNode>> GetPolicy(User user)
        {
            if (!user.HasPermission(_store, StorePermission.EditStorePolicy).IsSuccess)
            {
                return Result.Fail<IList<RuleInfoNode>>("User doesn't have permission to edit/view store's policy");
            }
            
            IList<RuleInfoNode> ruleInfoNodes = new List<RuleInfoNode>();
            foreach (var rule in _storeRules)
            {
                var res = rule.GetRuleInfo();
                if (res.IsFailure)
                {
                    return Result.Fail<IList<RuleInfoNode>>(res.Error);
                }
                ruleInfoNodes.Add(res.Value);
            }
            return Result.Ok(ruleInfoNodes);
        }

        public Result Reset(User user)
        {
            if (!user.HasPermission(_store, StorePermission.EditStorePolicy).IsSuccess)
            {
                return Result.Fail<IList<DiscountInfoNode>>("User doesn't have the permission to handle store discounts and policies");
            }

            this._storeRules = new List<Composite>();
            return Result.Ok();
        }
    }
}