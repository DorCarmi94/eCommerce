using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using eCommerce.Adapters;
using eCommerce.Common;
using eCommerce.DataLayer;

namespace eCommerce.Business.Repositories
{
    public class PersistenceStoreRepo : AbstractStoreRepo
    {
        private DataFacade _dataFacade;

        public PersistenceStoreRepo(DataFacade dataFacade)
        {
            _dataFacade = dataFacade;
        }

        public override IEnumerable<ItemInfo> SearchForItem(string query)
        {
            var lst = _dataFacade.GetAllItems();
            return lst.Value.Where(x => x._name.ToUpper().Contains(query.ToUpper())).Select(x=>x.ShowItem()).ToList();
        }

        public override IEnumerable<ItemInfo> SearchForItemByPrice(string query, double @from, double to)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ItemInfo> SearchForItemByCategory(string query, string category)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> SearchForStore(string query)
        {
            var lst = _dataFacade.GetAllStores();
            return lst.Where(x => x._storeName.ToUpper().Contains(query.ToUpper())).Select(x=>x.StoreName).ToList();
        }

        public override bool Add([NotNull] Store store)
        {
            return _dataFacade.SaveStore(store).IsSuccess;
        }

        public override Store GetOrNull([NotNull] string storeName)
        {
            Result<Store> storeRes = _dataFacade.ReadStore(storeName);
            if (storeRes.IsFailure)
            {
                return null;
            }

            return storeRes.Value;
        }

        public override void Remove(string id)
        {
            throw new System.NotImplementedException();
        }

        public override void Update(Store data)
        {
            _dataFacade.UpdateStore(data);
        }

        public override void UpdateManager(ManagerAppointment manager)
        {
            _dataFacade.UpdateManager(manager);
        }
    }
}