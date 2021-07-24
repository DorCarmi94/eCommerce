using System;
using System.Collections.Generic;
using eCommerce.Business.CombineRules;
using eCommerce.Business.DiscountPoliciesCombination;
using eCommerce.Business.Discounts;

using eCommerce.Common;

namespace eCommerce.Business
{
    public class DefaultDiscount: DiscountComposite
    {
        public DefaultDiscount(): base(new TotalAmounts(0,new BiggerEquals()),1)
        {
        }
        
    }
}