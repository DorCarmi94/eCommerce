using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.Purchases;

using eCommerce.Common;

namespace eCommerce.Business
{
    public class Amounts: CompositeRule
    {
        private string _item;
        private int _amount;
        private Compare _compare;
        
        public Amounts(string item,int amount,Compare compare)
        {
            _item = item;
            _amount = amount;
            _compare = compare;
        }

        private Dictionary<string, ItemInfo> CheckForSpecificItem(IBasket basket)
        {
            bool ans = false;
            Dictionary<string, ItemInfo> itemsList = new Dictionary<string, ItemInfo>();
            foreach (var item in basket.GetAllItems().Value)
            {
                if (item.name.Equals(_item) 
                    && _compare.GetResult(_amount,item.amount)>0)
                    {
                        if (!itemsList.ContainsKey(item.name))
                        {
                            itemsList.Add(item.name,item);
                        }
                    }
            }

            return itemsList;
        }
        
        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, User checkItem2)
        {
            var lst=CheckForSpecificItem(checkItem1);
            return lst;
        }

        public override bool CheckOneItem(ItemInfo item, User checkItem2)
        {
            if (item.name.Equals(_item) 
                && _compare.GetResult(_amount,item.amount)>0)
            {
                return true;
            }

            return false;
        }

        public override Result<RuleInfoNode> GetRuleInfo()
        {
            return Result.Ok<RuleInfoNode>(new RuleInfoNodeLeaf(new RuleInfo(RuleType.Amount, this._amount.ToString(), "", this._item,
                this._compare.GetComperatorInfo())));
        }
        
    }
}