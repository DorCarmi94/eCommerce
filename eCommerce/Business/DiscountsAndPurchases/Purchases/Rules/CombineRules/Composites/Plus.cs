using System;
using System.Collections.Generic;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.DiscountsAndPurchases.Purchases.Rules.CombineRules;

using eCommerce.Common;

namespace eCommerce.Business.DiscountPoliciesCombination
{
    public class Plus : CombinatedComposite
    {
        private Composite _A;
        private Composite _B;
        public Plus(Composite A, Composite B)
        {
            this._A = A;
            this._B = B;
        }
        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, User checkItem2)
        {
            Dictionary<string, ItemInfo> itemsList = new Dictionary<string, ItemInfo>();
            foreach (var item in checkItem1.GetAllItems().Value)
            {
                itemsList.Add(item.name,item);
            }

            return itemsList;
        }

        public override bool CheckOneItem(ItemInfo itemInfo, User checkItem2)
        {
            return true;
        }

        public override bool CheckIfDiscount()
        {
            return _A.CheckIfDiscount() && _B.CheckIfDiscount();
        }
        
        
        public override Result<double> Get(IBasket basket, User user)
        {
            if (CheckIfDiscount())
            {
                var price = basket.GetRegularTotalPrice();
                var discountA=_A.Get(basket, user);
                if (discountA.IsFailure)
                {
                    return discountA;
                }

                var discountB = _B.Get(basket, user);
                if (discountB.IsFailure)
                {
                    return discountB;
                }

                var theDiscoutA = discountA.Value / price;
                var theDiscoutB = discountB.Value / price;
                var plusDiscount = (1-theDiscoutA) + (1-theDiscoutB);
                return Result.Ok(price * (1 - plusDiscount));
            }
            else
            {
                return Result.Fail<double>("Not a discount");
            }
        }

        public override Result<double> GetOneItem(ItemInfo itemInfo, User user)
        {
            var aGet = _A.GetOneItem(itemInfo, user);
            var bGet = _B.GetOneItem(itemInfo, user);
            if (aGet.IsSuccess && bGet.IsSuccess)
            {
                if (aGet.Value >= bGet.Value)
                {
                    return aGet;
                }
                else
                {
                    return bGet;
                }
            }
            else if(aGet.IsSuccess)
            {
                return aGet;
            }
            else if(bGet.IsSuccess)
            {
                return bGet;
            }
            else
            {
                return aGet;
            }
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