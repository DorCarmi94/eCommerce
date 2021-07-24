using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using eCommerce.Adapters;
using eCommerce.Common;

namespace eCommerce.Business.Repositories
{
    public class InMemoryStoreRepo : AbstractStoreRepo
    {
        private ConcurrentDictionary<string, Store> _stores;

        public InMemoryStoreRepo()
        {
            _stores = new ConcurrentDictionary<string, Store>();
        }

        public override bool Add([NotNull] Store store)
        {
            return _stores.TryAdd(store.GetStoreName().ToUpper(), store);
        }

        public override Store GetOrNull([NotNull] string storeName)
        {
            Store store = null;
            _stores.TryGetValue(storeName.ToUpper(), out store);
            return store;
        }

        public override void Remove(string id)
        {
            
        }

        public override void Update(Store data)
        {
            
        }

        public override void UpdateManager(ManagerAppointment manager)
        {
            
        }

        public override IEnumerable<ItemInfo> SearchForItem(string query)
        {

            SortedList<int, ItemInfo> list = new SortedList<int, ItemInfo>();
            
            foreach (var store in _stores.Values)
            {
                foreach (var item in store.GetAllItems().Where(x=>x._name.ToUpper().Contains(query.ToUpper())))
                {
                    list.Add(EditDistance(query, item._name), item.ShowItem());
                }
            }
            
            return list.Values;
        }
        
        public override IEnumerable<ItemInfo> SearchForItemByPrice(string query, double from, double to)
        {
            IList<ItemInfo> queryMatches = new List<ItemInfo>();
            foreach (var store in _stores.Values)
            {
                foreach (var item in store.SearchItemWithPriceFilter(query, (int)from, (int)to))
                {
                    queryMatches.Add(item.ShowItem());
                }
            }

            return queryMatches;
        }

        public override IEnumerable<ItemInfo> SearchForItemByCategory(string query, string category)
        {
            IList<ItemInfo> queryMatches = new List<ItemInfo>();
            foreach (var store in _stores.Values)
            {
                foreach (var item in store.SearchItemWithCategoryFilter(query, category))
                {
                    queryMatches.Add(item.ShowItem());
                }
            }

            return queryMatches;
        }

        public override IEnumerable<string> SearchForStore(string query)
        { 
            List<string> storeNames = _stores.Keys.ToList();
            storeNames.Sort(EditDistance);
            var filtered=storeNames.FindAll(x => x.ToUpper().Contains(query.ToUpper()));
            return filtered;
        }
    }
}