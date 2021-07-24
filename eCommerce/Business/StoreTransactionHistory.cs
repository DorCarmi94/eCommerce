using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using eCommerce.Common;

namespace eCommerce.Business
{
    public class StoreTransactionHistory
    {
        [Key]
        public string StoreName { get; set; }

        public IList<PurchaseRecord> _history { get; set; }

        public StoreTransactionHistory(Store store)
        {
            this.StoreName = store.StoreName;
            this._history = new List<PurchaseRecord>();
        }

        public Result<PurchaseRecord> AddRecordToHistory(Store store, IBasket basket)
        {
            PurchaseRecord newRecord = new PurchaseRecord(store, basket, DateTime.Now);
            this._history.Add(newRecord);
            return Result.Ok<PurchaseRecord>(newRecord);
        }

        public Result<IList<PurchaseRecord>> GetHistory(User user)
        {
            return Result.Ok(this._history);
        }
        
        public Result<IList<PurchaseRecord>> GetHistory(User user, DateTime dateStart, DateTime dateEnd)
        {
            var lst = this._history.Where(x => x.GetDate() >= dateStart && x.GetDate() <= dateEnd).ToList();
                return Result.Ok<IList<PurchaseRecord>>(lst);
        }
        

        //ef constructor
        public StoreTransactionHistory()
        {}
    }

    
}