using System;
using eCommerce.Business.CombineRules;
using eCommerce.Business.DiscountPoliciesCombination;
using eCommerce.Business.DiscountsAndPurchases.Purchases.Rules.CombineRules;
using eCommerce.Business.Purchases;
using eCommerce.Common;

namespace eCommerce.Business.Discounts
{
    public class DiscountHandler
    {
        public static Result<Composite> HandleDiscount(DiscountInfoNode discountInfoNode)
        {
            return discountInfoNode.handleDiscountInfo();
        }

        public static Result<Composite> Handle(DiscountInfoCompositeNode discountInfoCompositeNode)
        {
            Composite retComp;
            var resA = HandleDiscount(discountInfoCompositeNode.combinationDiscountInfoNodeA);
            if (resA.IsFailure)
            {
                return resA;
            }

            var resB = HandleDiscount(discountInfoCompositeNode.combinationDiscountInfoNodeB);
            if (resB.IsFailure)
            {
                return resB;
            }

            switch (discountInfoCompositeNode._combination)
            {
                case Combinations.AND:
                    retComp = new And(resA.Value, resB.Value);
                    break;
                case Combinations.OR:
                    retComp = new Or(resA.Value, resB.Value);
                    break;
                case Combinations.MAX:
                    retComp = new Max(resA.Value, resB.Value);
                    break;
                case Combinations.MIN:
                    retComp = new Min(resA.Value, resB.Value);
                    break;
                case Combinations.NOT:
                    retComp = new Not(resA.Value);
                    break;
                case Combinations.XOR:
                    retComp = new Xor(resA.Value, resB.Value,RuleHandler.HandleComperator(discountInfoCompositeNode.Comperator),
                        discountInfoCompositeNode.FieldToCompare,discountInfoCompositeNode.ItemInfoToCompare);
                    break;
                case Combinations.PLUS:
                    retComp = new Plus(resA.Value, resB.Value);
                    break;
                default:
                    retComp = new DefaultDiscount();
                    break;
            }

            return Result<Composite>.Ok(retComp);
        }

        public static Result<Composite> Handle(DiscountInfoLeaf discountInfoLeaf)
        {
            Composite retComp;
            var rule = RuleHandler.HandleRule(discountInfoLeaf.theRule);
            if (rule.IsFailure)
            {
                return Result.Fail<Composite>(rule.Error);
            }
            
            DiscountComposite discountComposite;
            if (discountInfoLeaf.theItemsToPerformTheDiscountOn == null)
            {
                discountComposite= new DiscountComposite(rule.Value, discountInfoLeaf.theDiscount);
            }
            else
            {
                var itemToDiscount = RuleHandler.HandleRule(discountInfoLeaf.theItemsToPerformTheDiscountOn);
                discountComposite = new DiscountComposite(rule.Value, discountInfoLeaf.theDiscount,
                    itemToDiscount.Value);
            }

            retComp = discountComposite;
            return Result.Ok(retComp);
        }

        public static Result<DiscountInfoNode> GetDiscountInfo(CombinatedComposite combinatedComposite)
        {
            throw new NotImplementedException();
        }
    }
}