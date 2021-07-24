using System.Collections.Generic;
using System.Globalization;
using eCommerce.Business.CombineRules;
using eCommerce.Business.DiscountsAndPurchases.Purchases.RulesInfo;
using eCommerce.Common;

namespace eCommerce.Business.Discounts
{
    public class DiscountInfoCompositeNode: DiscountInfoNode
    {
        public DiscountInfoNode combinationDiscountInfoNodeA;
        public DiscountInfoNode combinationDiscountInfoNodeB;
        public Combinations _combination;
        
        //Xor
        public Comperators Comperator;
        public FieldToCompare FieldToCompare;
        public ItemInfo ItemInfoToCompare;

        public DiscountInfoCompositeNode(DiscountInfoNode nodeA,DiscountInfoNode nodeB, Combinations combination)
        {
            this.combinationDiscountInfoNodeA = nodeA;
            this.combinationDiscountInfoNodeB = nodeB;
            this._combination = combination;
        }
        
        public DiscountInfoCompositeNode(DiscountInfoNode nodeA,DiscountInfoNode nodeB,
            Comperators Comperator,FieldToCompare FieldToCompare,ItemInfo itemInfoToCompare)
        {
            this.combinationDiscountInfoNodeA = nodeA;
            this.combinationDiscountInfoNodeB = nodeB;
            this._combination = Combinations.XOR;
            this.Comperator = Comperator;
            this.FieldToCompare = FieldToCompare;
            this.ItemInfoToCompare = itemInfoToCompare;
        }
        
        public DiscountInfoCompositeNode(DiscountInfoNode nodeA,DiscountInfoNode nodeB,
            Comperators Comperator,FieldToCompare FieldToCompare)
        {
            this.combinationDiscountInfoNodeA = nodeA;
            this.combinationDiscountInfoNodeB = nodeB;
            this._combination = Combinations.XOR;
            this.Comperator = Comperator;
            this.FieldToCompare = FieldToCompare;
        }


        public override bool hasKids()
        {
            return true;
        }

        public override Result<Composite> handleDiscountInfo()
        {
            return DiscountHandler.Handle(this);
        }
    }
}