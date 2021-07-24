using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class UserTransactionHistory
    {
        [Key]
        public string Id { get; set; }
        public virtual List<PurchaseRecord> _purchases { get; set; }
        private Object transLock;

        
        public UserTransactionHistory(string username)
        {
            Id = username;
            _purchases = new List <PurchaseRecord>();
            transLock = new Object();
        }
        public UserTransactionHistory()
        {
            transLock = new Object();
        }
        
        public Result EnterRecordToHistory(PurchaseRecord record)
        {
            lock (transLock)
            {
                _purchases.Add(record);
                return Result.Ok();
            }
        }
        public Result<IList<PurchaseRecord>> GetUserHistory()
        {
            lock (transLock)
            {
                return Result.Ok<IList<PurchaseRecord>>(new List<PurchaseRecord>(_purchases));
            }
        }

    }
}