using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace eCommerce.Service
{
    public interface IItem
    {
        public string ItemName { get; }
        public string StoreName { get; }
        public int Amount { get; }
        public string Category { get; }
        public List<string> KeyWords { get; }
        public double PricePerUnit { get; }

    }
}