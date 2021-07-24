using eCommerce.Business.CombineRules;
using eCommerce.Common;

namespace eCommerce.Business.Discounts
{
    public abstract class DiscountInfoNode
    {
        public abstract bool hasKids();

        public abstract Result<Composite> handleDiscountInfo();
    }
}