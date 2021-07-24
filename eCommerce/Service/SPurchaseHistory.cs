using System.Collections.Generic;
using eCommerce.Business;

namespace eCommerce.Service
{
    public class SPurchaseHistory
    {
        public IList<SPurchaseRecord> Records { get; }

        public SPurchaseHistory(IList<SPurchaseRecord> records)
        {
            Records = records;
        }
        
        internal SPurchaseHistory(IList<PurchaseRecord> records)
        {
            Records = new List<SPurchaseRecord>();
            foreach (var record in records)
            {
                Records.Add(new SPurchaseRecord(record, record.Username));
            }
        }
    }
}