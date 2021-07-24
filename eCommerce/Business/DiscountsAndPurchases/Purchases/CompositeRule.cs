using System.Collections.Generic;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Common;

namespace eCommerce.Business
{
    public abstract class CompositeRule : Composite
    {

        public override bool CheckIfDiscount()
        {
            return false;
        }

        public override Result<double> Get(IBasket basket, User user)
        {
            return Result.Fail<double>("Not a discount, only a rule");
        }

        public override Result<double> GetOneItem(ItemInfo itemInfo, User user)
        {
            return Result.Fail<double>("Not a discount");
        }
        public override Result<DiscountInfoNode> GetDisocuntInfo()
        {
            return Result.Fail<DiscountInfoNode>("Not a discount");
        }
    }
}