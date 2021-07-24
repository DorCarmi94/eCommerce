using System;

namespace eCommerce.Business.CombineRules
{
    public class SmallerEquals: Compare
    {
        public override int GetResult(IComparable a, IComparable b)
        {
            return b.CompareTo(a) <= 0 ? 1 : -1;
        }

        public override Comperators GetComperatorInfo()
        {
            return Comperators.SMALLER_EQUALS;
        }
    }
}