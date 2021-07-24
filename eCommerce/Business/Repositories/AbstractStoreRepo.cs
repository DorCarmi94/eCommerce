using System;
using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business.Repositories
{
    public abstract class AbstractStoreRepo : IRepository<Store>
    {
        public abstract IEnumerable<ItemInfo> SearchForItem(string query);

        public abstract IEnumerable<ItemInfo> SearchForItemByPrice(string query, double from, double to);

        public abstract IEnumerable<ItemInfo> SearchForItemByCategory(string query, string category);

        public abstract IEnumerable<string> SearchForStore(string query);
        

        /// <summary>
        /// How match chars we need to add or replace to src to get to traget
        /// </summary>
        /// <param name="src">The source</param>
        /// <param name="target">The target string</param>
        /// <returns>Edit distance</returns>
        public int EditDistance(string src, string target) {
            int n = src.Length;
            int m = target.Length;
            int[,] d = new int[n + 1, m + 1];
            if (n == 0) {
                return m;
            }
            if (m == 0) {
                return n;
            }
            for (int i = 0; i <= n; d[i, 0] = i++)
                ;
            for (int j = 0; j <= m; d[0, j] = j++)
                ;
            for (int i = 1; i <= n; i++) {
                for (int j = 1; j <= m; j++) {
                    int editDistance = (target[j - 1] == src[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + editDistance);
                }
            }
            return d[n, m];
        }

        public abstract bool Add(Store data);

        public abstract Store GetOrNull(string id);

        public abstract void Remove(string id);

        public abstract void Update(Store data);

        public abstract void UpdateManager(ManagerAppointment manager);
    }
}