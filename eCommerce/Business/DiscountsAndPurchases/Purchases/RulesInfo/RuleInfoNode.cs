using System.Data;
using eCommerce.Business.CombineRules;

using eCommerce.Common;

namespace eCommerce.Business.Discounts
{
    public abstract class RuleInfoNode 
    {
        public abstract bool hasKids();

        public abstract Result<Composite> handleRuleInfo();
        
    }
}