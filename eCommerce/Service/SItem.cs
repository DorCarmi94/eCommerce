using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace eCommerce.Service
{
    public class SItem : IItem
    {
        public string ItemName { get; set; }
        public string StoreName { get; set; }
        public int Amount { get; set; }
        
        public string Category { get; set; }
        public List<string> KeyWords { get; set; }
        public double PricePerUnit { get; set; }

        public SItem(string itemName, string storeName, int amount,
            string category, List<string> keyWords, double pricePerUnit)
        {
            ItemName = itemName;
            StoreName = storeName;
            Amount = amount;
            Category = category;
            KeyWords = keyWords;
            PricePerUnit = pricePerUnit;
        }
    }
}