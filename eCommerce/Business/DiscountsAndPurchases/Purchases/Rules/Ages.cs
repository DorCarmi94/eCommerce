using System;
using System.Collections.Generic;
using eCommerce.Business.Discounts;
using eCommerce.Business.Purchases;

using eCommerce.Common;

namespace eCommerce.Business.PurchaseRules
{
    public class Ages : CompositeRule
    {
        private int age;
        private Compare compare;

        public Ages(int age, Compare compare)
        {
            this.age = age;
            this.compare = compare;
        }
        
        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, User checkItem2)
        {
            Dictionary<string, ItemInfo> itemsList = new Dictionary<string, ItemInfo>();
            int currAge = (DateTime.Now.Date-checkItem2.MemberInfo.Birthday.Date).Days/ 365;
            var compareAns = compare.GetResult(age, currAge);
            if (compareAns > 0)
            {
                foreach (var item in checkItem1.GetAllItems().Value)
                {
                    if (!itemsList.ContainsKey(item.name))
                    {
                        itemsList.Add(item.name,item);
                    }
                }
            }

            return itemsList;
        }

        public override bool CheckOneItem(ItemInfo itemInfo, User checkItem2)
        {
            var compareAns = compare.GetResult(age, (checkItem2.MemberInfo.Birthday.Date - DateTime.Now.Date).Days);
            return compareAns > 0;
        }

        public override Result<RuleInfoNode> GetRuleInfo()
        {
            return Result.Ok<RuleInfoNode>(new RuleInfoNodeLeaf(new RuleInfo(RuleType.Age, this.age.ToString(), "", "",
                compare.GetComperatorInfo())));
        }

        
    }
    
}