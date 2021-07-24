using System;
using System.Collections.Generic;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.DiscountsAndPurchases.Purchases.Rules.CombineRules;

using eCommerce.Common;

namespace eCommerce.Business.DiscountPoliciesCombination
{
    public class Min : CombinatedComposite
    {
        private Composite _A;
        private Composite _B;
        public Min(Composite A, Composite B)
        {
            this._A = A;
            this._B = B;
        }
        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, User checkItem2)
        {
            Dictionary<string, ItemInfo> itemsList = new Dictionary<string, ItemInfo>();
            var aLst = _A.Check(checkItem1, checkItem2);
            var bLst = _B.Check(checkItem1, checkItem2);
            if (aLst.Count > 0 && bLst.Count>0)
            {
                var aGet = _A.Get(checkItem1, checkItem2);
                var bGet = _B.Get(checkItem1, checkItem2);
                if (aGet.IsSuccess && bGet.IsSuccess)
                {
                    if (aGet.Value <= bGet.Value)
                    {
                        return aLst;
                    }
                    else
                    {
                        return bLst;
                    }
                }
                else
                {
                    return itemsList;
                }
            }
            else if(aLst.Count>0)
            {
                return aLst;
            }
            else if (bLst.Count > 0)
            {
                return bLst;
            }
            else
            {
                return itemsList;
            }
        }

        public override bool CheckOneItem(ItemInfo itemInfo, User checkItem2)
        {
            return _A.CheckOneItem(itemInfo, checkItem2) || _B.CheckOneItem(itemInfo, checkItem2);
        }


        private Composite CheckWhich(IBasket checkItem1, User checkItem2)
        {
            Dictionary<string, ItemInfo> itemsList = new Dictionary<string, ItemInfo>();
            var aLst = _A.Check(checkItem1, checkItem2);
            var bLst = _B.Check(checkItem1, checkItem2);
            if (aLst.Count > 0 && bLst.Count>0)
            {
                var aGet = _A.Get(checkItem1, checkItem2);
                var bGet = _B.Get(checkItem1, checkItem2);
                if (aGet.IsSuccess && bGet.IsSuccess)
                {
                    if (aGet.Value <= bGet.Value)
                    {
                        return _A;
                    }
                    else
                    {
                        return _B;
                    }
                }
                else
                {
                    return _A;
                }
            }
            else if(aLst.Count>0)
            {
                return _A;
            }
            else if (bLst.Count > 0)
            {
                return _B;
            }
            else
            {
                return _A;
            }
        }
        
        public override bool CheckIfDiscount()
        {
            return _A.CheckIfDiscount() && _B.CheckIfDiscount();
        }
        
        
        public override Result<double> Get(IBasket basket, User user)
        {
            if (CheckIfDiscount())
            {
                var theOne = CheckWhich(basket, user);
                return theOne.Get(basket, user);
                
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
                if (aGet.Value <= bGet.Value)
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
            return Combinations.MIN;
        }
    }
}