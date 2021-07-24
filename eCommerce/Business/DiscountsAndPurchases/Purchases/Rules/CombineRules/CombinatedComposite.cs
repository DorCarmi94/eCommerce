using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.Purchases;
using eCommerce.Common;

namespace eCommerce.Business.DiscountsAndPurchases.Purchases.Rules.CombineRules
{
    public abstract class CombinatedComposite: Composite
    {
        public override Result<RuleInfoNode> GetRuleInfo()
        {
            var resA = this.GetRuleInfo_A();
            var resB = this.GetRuleInfo_B();
            if (resA.IsFailure)
            {
                return resA;
            }

            if (resB.IsFailure)
            {
                return resB;
            }

            return Result.Ok<RuleInfoNode>(new RuleInfoNodeComposite(resA.Value, resB.Value, this.GetCombination()));
        }

        public override Result<DiscountInfoNode> GetDisocuntInfo()
        {
            var resA = this.GetDiscountInfo_A();
            var resB = this.GetDiscountInfo_B();
            if (resA.IsFailure)
            {
                return resA;
            }

            if (resB.IsFailure)
            {
                return resB;
            }

            return Result.Ok<DiscountInfoNode>(new DiscountInfoCompositeNode(resA.Value, resB.Value, this.GetCombination()));
        }

        public abstract Result<RuleInfoNode> GetRuleInfo_A();
        public abstract Result<RuleInfoNode> GetRuleInfo_B();
        
        public abstract Result<DiscountInfoNode> GetDiscountInfo_A();
        public abstract Result<DiscountInfoNode> GetDiscountInfo_B();

        public abstract Combinations  GetCombination();
    }
}