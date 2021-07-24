using System.Collections.Generic;
using eCommerce.Business.CombineRules;
using eCommerce.Common;

namespace eCommerce.Business.Discounts
{
    public class DiscountInfoLeaf : DiscountInfoNode
    {
        public double theDiscount;
        public RuleInfoNode theRule;
        public RuleInfoNode theItemsToPerformTheDiscountOn;

        public DiscountInfoLeaf(double discount, RuleInfoNode rule)
        {
            this.theDiscount = discount;
            this.theRule = rule;
            this.theItemsToPerformTheDiscountOn = null;
        }
        
        public DiscountInfoLeaf(double discount, RuleInfoNode rule, RuleInfoNode theItemsToPerformTheDiscountOn)
        {
            this.theDiscount = discount;
            this.theRule = rule;
            this.theItemsToPerformTheDiscountOn = theItemsToPerformTheDiscountOn;
        }
        
        

        public override bool hasKids()
        {
            return false;
        }

        public override Result<Composite> handleDiscountInfo()
        {
            return DiscountHandler.Handle(this);
        }
    }
}