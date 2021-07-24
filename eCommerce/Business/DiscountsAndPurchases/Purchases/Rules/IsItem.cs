using System;
using System.Collections.Generic;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.Purchases;
using eCommerce.Common;

namespace eCommerce.Business.PurchaseRules
{
    public class IsItem: CompositeRule
    {
        private string _itemName;

        public IsItem(string itemName)
        {
            this._itemName = itemName;
        }
        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, User checkItem2)
        {
            Dictionary<string, ItemInfo> itemsList = new Dictionary<string, ItemInfo>();
            foreach (var item in checkItem1.GetAllItems().Value)
            {
                if (item.name.Equals(_itemName))
                {
                    if (!itemsList.ContainsKey(item.name))
                    {
                        itemsList.Add(item.name,item);
                        
                    }
                    return itemsList;
                }
            }

            return itemsList;
        }

        public override bool CheckOneItem(ItemInfo itemInfo, User checkItem2)
        {
            return this._itemName.Equals(itemInfo.name);
        }

        public override Result<RuleInfoNode> GetRuleInfo()
        {
            return Result.Ok<RuleInfoNode>(new RuleInfoNodeLeaf(new RuleInfo(RuleType.IsItem, "", "", this._itemName,
                Comperators.EQUALS)));
        }
    }
}