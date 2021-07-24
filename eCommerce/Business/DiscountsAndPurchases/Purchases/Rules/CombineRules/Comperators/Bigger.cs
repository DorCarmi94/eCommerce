using System;

namespace eCommerce.Business.CombineRules
{
    public class Bigger : Compare
    {
        public override int GetResult(IComparable a, IComparable b)
        {
            var cop = a.CompareTo(b);
            return b.CompareTo(a) > 0 ? 1 : -1;
        }

        public override Comperators GetComperatorInfo()
        {
            return Comperators.BIGGER;
        }
    }
}