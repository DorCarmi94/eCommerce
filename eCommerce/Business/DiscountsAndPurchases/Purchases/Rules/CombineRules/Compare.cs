using System;
using eCommerce.Business.CombineRules;

namespace eCommerce.Business
{
    public abstract class Compare
    {
        /// <summary>
        /// Compares two items and returns int indicates the result
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>return 1 if a is ? from b, 0 if equals and -1 if a is not ? from b</returns>
        public abstract int GetResult(IComparable a, IComparable b);

        public abstract Comperators GetComperatorInfo();
    }
    
    
}