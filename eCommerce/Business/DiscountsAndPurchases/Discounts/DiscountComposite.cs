using System.Collections.Generic;
using eCommerce.Business.Discounts;
using eCommerce.Business.Purchases;

using eCommerce.Common;

namespace eCommerce.Business.CombineRules
{
    public class DiscountComposite : Composite
    {
        private Composite _rule;
        private double _theDiscount;
        private Composite _theItemsToPerformTheDiscountOn;

        public DiscountComposite(Composite rule,double theDiscount, Composite _theItemsToPerformTheDiscountOn)
        {
            this._rule = rule;
            this._theDiscount = theDiscount;
            this._theItemsToPerformTheDiscountOn = _theItemsToPerformTheDiscountOn;
        }
        public DiscountComposite(Composite rule,double theDiscount)
        {
            this._rule = rule;
            this._theDiscount = theDiscount;
            _theItemsToPerformTheDiscountOn = null;
        }

        public Result<double> GetDiscount(IBasket basket, User user)
        {
            double newPrice = basket.GetRegularTotalPrice();
            var lst = this._rule.Check(basket,user);
            if (lst.Count > 0)
            {
                if (_theItemsToPerformTheDiscountOn == null)
                {
                    foreach (var item in lst)
                    {
                        var price = item.Value.amount * item.Value.pricePerUnit;
                        var priceAfterDiscount = item.Value.amount * item.Value.pricePerUnit * _theDiscount;
                        var diff = price - priceAfterDiscount;
                        newPrice -= diff;
                    }
                }
                else
                {
                    var theItems = _theItemsToPerformTheDiscountOn.Check(basket, user);
                    foreach (var item in theItems)
                    {
                        var price = item.Value.amount * item.Value.pricePerUnit;
                        var priceAfterDiscount = item.Value.amount * item.Value.pricePerUnit * _theDiscount;
                        var diff = price - priceAfterDiscount;
                        newPrice -= diff;
                    }
                }
            }

            return Result.Ok(newPrice);
        }

        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, User checkItem2)
        {
            if (_rule == null)
            {
                return new Dictionary<string, ItemInfo>();
            }
            return _rule.Check(checkItem1, checkItem2);
        }

        public override bool CheckOneItem(ItemInfo itemInfo, User checkItem2)
        {
            throw new System.NotImplementedException();
        }

        public override bool CheckIfDiscount()
        {
            return true;
        }

        public override Result<double> Get(IBasket basket, User user)
        {
            return GetDiscount(basket, user);
        }

        public override Result<double> GetOneItem(ItemInfo itemInfo, User user)
        {
            return Result.Ok(itemInfo.amount * itemInfo.pricePerUnit * _theDiscount);
        }

        public override Result<RuleInfoNode> GetRuleInfo()
        {
            return this._rule.GetRuleInfo();
        }

        public override Result<DiscountInfoNode> GetDisocuntInfo()
        {
            var resRule = _rule.GetRuleInfo();
            if (resRule.IsFailure)
            {
                return Result.Fail<DiscountInfoNode>("Problem getting discount rule info");
            }

            return Result.Ok<DiscountInfoNode>(new DiscountInfoLeaf(_theDiscount,resRule.Value));
        }
    }
}