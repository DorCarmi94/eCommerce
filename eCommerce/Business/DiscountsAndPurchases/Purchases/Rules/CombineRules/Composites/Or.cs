using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.DiscountsAndPurchases.Purchases.Rules.CombineRules;

using eCommerce.Common;
using Rule = System.Data.Rule;

namespace eCommerce.Business.DiscountPoliciesCombination
{
    public class Or : CombinatedComposite
    {
        private Composite _A;
        private Composite _B;

        public Or(Composite A, Composite B)
        {
            this._A = A;
            this._B = B;
        }

        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, User checkItem2)
        {
            Dictionary<string, ItemInfo> itemsList = new Dictionary<string, ItemInfo>();
            var aLst = _A.Check(checkItem1, checkItem2);
            if (aLst.Count > 0)
            {
                itemsList=CombineDictionaries(itemsList, aLst);
            }
            var bLst = _B.Check(checkItem1, checkItem2);
            if (bLst.Count > 0)
            {
                itemsList=CombineDictionaries(itemsList, bLst);
            }

            return itemsList;
        }

        public override bool CheckOneItem(ItemInfo itemInfo, User checkItem2)
        {
            return _A.CheckOneItem(itemInfo, checkItem2) || _B.CheckOneItem(itemInfo, checkItem2);
        }

        public override bool CheckIfDiscount()
        {
            return _A.CheckIfDiscount() && _B.CheckIfDiscount();
        }

        public override Result<double> Get(IBasket basket, User user)
        {
            if (CheckIfDiscount())
            {
                double price = basket.GetRegularTotalPrice();
                var priceA = _A.Get(basket, user);
                if (priceA.IsSuccess)
                {
                    var diffA = priceA.Value/price;
                    price = price * diffA;
                }
                var priceB = _B.Get(basket, user);
                if (priceB.IsSuccess)
                {
                        
                    var diffB = priceB.Value/price;
                    price=price*diffB;
                }

                return Result.Ok(price);
            }
            else
            {
                return Result.Fail<double>("Not a discount");
            }
        }

        public override Result<double> GetOneItem(ItemInfo itemInfo, User user)
        {
            var getA = _A.GetOneItem(itemInfo, user);
            var getB = _B.GetOneItem(itemInfo, user);
            if (getA.IsFailure)
            {
                return getA;
            }

            if (getB.IsFailure)
            {
                return getB;
            }

            var price = itemInfo.amount * itemInfo.pricePerUnit;
            var diffA = getA.Value/price ;
            var diffB = getB.Value/price;
            return Result.Ok(price*diffA*diffB);

        }

        public override Result<RuleInfoNode> GetRuleInfo_A()
        {
            return _A.GetRuleInfo();
        }

        public override Result<RuleInfoNode> GetRuleInfo_B()
        {
            return _B.GetRuleInfo();
        }

        public override Result<DiscountInfoNode> GetDiscountInfo_A()
        {
            return _A.GetDisocuntInfo();
        }

        public override Result<DiscountInfoNode> GetDiscountInfo_B()
        {
            return _B.GetDisocuntInfo();
        }

        public override Combinations GetCombination()
        {
            return Combinations.OR;
        }
    }
}