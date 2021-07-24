using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.Purchases;

using eCommerce.Common;

namespace eCommerce.Business
{
    public class Categories: CompositeRule
    {
        private string _category;
        public Categories(string category)
        {
            this._category = category;
        }
        
        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, User checkItem2)
        {
            bool ans = false;
            Dictionary<string, ItemInfo> itemsList = new Dictionary<string, ItemInfo>();
            foreach (var item in checkItem1.GetAllItems().Value)
            {
                if (item.category.Equals(_category))
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
            if (itemInfo.category.Equals(_category))
            {
                return true;
            }
            

            return false;
        }

        public override Result<RuleInfoNode> GetRuleInfo()
        {
            return Result.Ok<RuleInfoNode>(new RuleInfoNodeLeaf(new RuleInfo(RuleType.Category, this._category, "", "",
                Comperators.EQUALS)));
        }
        
    }
}