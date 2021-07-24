using System;
using System.Collections.Generic;
using eCommerce.Business.Discounts;

using eCommerce.Common;

namespace eCommerce.Business.CombineRules
{
    public abstract class Composite
    {
 

        public abstract Dictionary<string,ItemInfo> Check(IBasket checkItem1, User checkItem2);
        
        public abstract bool CheckOneItem(ItemInfo itemInfo, User checkItem2);
        
        

        public abstract bool CheckIfDiscount();

        public abstract Result<double> Get(IBasket basket, User user);
        public abstract Result<double> GetOneItem(ItemInfo itemInfo, User user);


        public Dictionary<string, ItemInfo> CombineDictionaries(Dictionary<string, ItemInfo> dictionary1,
            Dictionary<string, ItemInfo> dictionary2)
        {
            Dictionary<string, ItemInfo> combinedDictionary = new Dictionary<string, ItemInfo>();
            foreach (var itemInfo in dictionary1)
            {
                if (!combinedDictionary.ContainsKey(itemInfo.Key))
                {
                    combinedDictionary.Add(itemInfo.Key,itemInfo.Value);
                }
            }
            foreach (var itemInfo in dictionary2)
            {
                if (!combinedDictionary.ContainsKey(itemInfo.Key))
                {
                    combinedDictionary.Add(itemInfo.Key,itemInfo.Value);
                }
            }

            return combinedDictionary;
        }


        public abstract Result<RuleInfoNode> GetRuleInfo();

        public abstract Result<DiscountInfoNode> GetDisocuntInfo();
    }
}