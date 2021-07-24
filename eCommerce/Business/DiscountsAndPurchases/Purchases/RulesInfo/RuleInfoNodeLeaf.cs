using System.Collections.Generic;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Common;

namespace eCommerce.Business.Purchases
{
    public class RuleInfoNodeLeaf: RuleInfoNode
    {
        public RuleInfo rule;

        public RuleInfoNodeLeaf(RuleInfo rule)
        {
            this.rule = rule;
        }
        
        public override bool hasKids()
        {
            throw new System.NotImplementedException();
        }

        public override Result<Composite> handleRuleInfo()
        {
            return RuleHandler.Handle(this);
        }
        
    }
}