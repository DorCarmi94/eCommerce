using eCommerce.Business.CombineRules;
using eCommerce.Business.DiscountsAndPurchases.Purchases.RulesInfo;

namespace eCommerce.Business.Discounts
{
    public class RuleInfo
    {
        public RuleType RuleType;
        public string WhatIsTheRuleFor;
        public string WhatKind;
        public string WhichItems;
        public Comperators Comperators;
        

        public RuleInfo(RuleType ruleType, string whatIsTheRuleFor, string whatKind, string whichItems, Comperators comperators)
        {
            RuleType = ruleType;
            WhatIsTheRuleFor = whatIsTheRuleFor;
            WhatKind = whatKind;
            WhichItems = whichItems;
            Comperators = comperators;
        }
        public RuleInfo(RuleType ruleType, string whatIsTheRuleFor, string whatKind, string whichItems)
        {
            RuleType = ruleType;
            WhatIsTheRuleFor = whatIsTheRuleFor;
            WhatKind = whatKind;
            WhichItems = whichItems;
        }
    }
}