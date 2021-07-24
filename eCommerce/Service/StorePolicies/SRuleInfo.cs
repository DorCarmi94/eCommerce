using eCommerce.Business;
using eCommerce.Business.CombineRules;

namespace eCommerce.Service.StorePolicies
{
    public class SRuleInfo
    {
        public RuleType RuleType { get; set; }
        public string WhatIsTheRuleFor { get; set; }
        public string Kind { get; set; }
        public string ItemId { get; set; }
        public Comperators Comperator { get; set; }

        public SRuleInfo()
        {
        }
        
        public SRuleInfo(RuleType ruleType, string whatIsTheRuleFor, string kind, string itemId, Comperators comperator)
        {
            RuleType = ruleType;
            WhatIsTheRuleFor = whatIsTheRuleFor;
            Kind = kind;
            ItemId = itemId;
            Comperator = comperator;
        }
        
        public SRuleInfo(RuleType ruleType, string whatIsTheRuleFor, string kind, string itemId)
        {
            RuleType = ruleType;
            WhatIsTheRuleFor = whatIsTheRuleFor;
            Kind = kind;
            ItemId = itemId;
        }
    }
}