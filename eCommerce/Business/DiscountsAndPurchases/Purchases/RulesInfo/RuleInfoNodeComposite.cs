using System.Globalization;
using eCommerce.Business.CombineRules;
using eCommerce.Business.DiscountPoliciesCombination;
using eCommerce.Business.Discounts;
using eCommerce.Business.DiscountsAndPurchases.Purchases.RulesInfo;
using eCommerce.Common;

namespace eCommerce.Business.Purchases
{
    public class RuleInfoNodeComposite : RuleInfoNode
    {
        public RuleInfoNode ruleA;
        public RuleInfoNode ruleB;
        public Combinations combination;
        
        //Xor
        public Comperators Comperator;
        public FieldToCompare FieldToCompare;
        public ItemInfo ItemInfoToCompare;
        public RuleInfoNodeComposite(RuleInfoNode ruleA, RuleInfoNode ruleB, Combinations combinations)
        {
            this.ruleA = ruleA;
            this.ruleB = ruleB;
            this.combination = combinations;
        }
        
        public RuleInfoNodeComposite(RuleInfoNode ruleA, RuleInfoNode ruleB,
            Comperators Comperator,FieldToCompare FieldToCompare,ItemInfo itemInfoToCompare)
        {
            this.ruleA = ruleA;
            this.ruleB = ruleB;
            this.combination = Combinations.XOR;
            this.Comperator = Comperator;
            this.FieldToCompare = FieldToCompare;
            this.ItemInfoToCompare = itemInfoToCompare;
        }
        
        public RuleInfoNodeComposite(RuleInfoNode ruleA, RuleInfoNode ruleB,
            Comperators Comperator,FieldToCompare FieldToCompare)
        {
            this.ruleA = ruleA;
            this.ruleB = ruleB;
            this.combination = Combinations.XOR;
            this.Comperator = Comperator;
            this.FieldToCompare = FieldToCompare;
            this.ItemInfoToCompare = null;
        }
        public override bool hasKids()
        {
            return true;
        }

        public override Result<Composite> handleRuleInfo()
        {
            return RuleHandler.Handle(this);
        }

        
    }
}