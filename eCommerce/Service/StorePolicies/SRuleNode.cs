using System;
using System.IO;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.Purchases;

namespace eCommerce.Service.StorePolicies
{
    public class SRuleNode
    {
        public SRuleNodeType Type { get; set; }
        
        // leaf
        public SRuleInfo RuleInfo { get; set; }
        
        // composite
        public SRuleNode RuleA { get; set; }
        public SRuleNode RuleB { get; set; }
        public Combinations Combination { get; set; }
        
        public SRuleNode()
        {
        }
        
        public SRuleNode(SRuleNodeType type, SRuleInfo ruleInfo)
        {
            Type = type;
            RuleInfo = ruleInfo;
        }

        public SRuleNode(SRuleNodeType type,SRuleNode ruleA, SRuleNode ruleB, Combinations combination)
        {
            Type = type;
            RuleA = ruleA;
            RuleB = ruleB;
            Combination = combination;
        }

        public RuleInfoNode ParseToRuleInfoNude()
        {
            switch (Type)
            {
                case SRuleNodeType.Leaf:
                {
                    return new RuleInfoNodeLeaf(new RuleInfo(
                        RuleInfo.RuleType,
                        RuleInfo.WhatIsTheRuleFor,
                        RuleInfo.Kind,
                        RuleInfo.ItemId,
                        RuleInfo.Comperator
                    ));
                }
                case SRuleNodeType.Composite:
                {
                    RuleInfoNode ruleInfoNodeA = RuleA.ParseToRuleInfoNude();
                    RuleInfoNode ruleInfoNodeB = RuleB.ParseToRuleInfoNude();
                    return new RuleInfoNodeComposite(ruleInfoNodeA, ruleInfoNodeB, Combination);
                }
                default:
                    throw new InvalidDataException();
            }
        }
    }
}