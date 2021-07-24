using System.IO;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.Purchases;

namespace eCommerce.Service.StorePolicies
{
    public class SDiscountNode
    {
        public SDiscountNodeType Type { get; set; }

        //leaf
        public SRuleNode Rule { get; set; }
        public double Discount { get; set; }
        
        // composite
        public SDiscountNode DiscountA { get; set; }
        public SDiscountNode DiscountB { get; set; }
        public Combinations Combination { get; set; }

        public DiscountInfoNode ParseToDiscountInfoNode()
        {
            switch (Type)
            {
                case SDiscountNodeType.Leaf:
                {
                    var ruleInfoNode = Rule.ParseToRuleInfoNude();
                    return new DiscountInfoLeaf(Discount, ruleInfoNode);
                }
                case SDiscountNodeType.Composite:
                {
                    var discountA = DiscountA.ParseToDiscountInfoNode();
                    var discountB = DiscountB.ParseToDiscountInfoNode();
                    return new DiscountInfoCompositeNode(discountA, discountB, Combination);
                }
                default:
                    throw new InvalidDataException();
            }
        }
    }
}