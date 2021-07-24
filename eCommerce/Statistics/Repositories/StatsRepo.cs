using System;
using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Statistics.Repositories
{
    public interface StatsRepo
    {
        
        /// <summary>
        /// Add login stat record
        /// </summary>
        /// <param name="stat">The login stat record</param>
        /// <returns>Result if it was added</returns>
        public Result AddLoginStat(LoginStat stat);
        
        public Result<List<LoginStat>> GetAllLoginStatsFrom(DateTime date);
        
        public Result<int> GetNumberOfLoginStatsFrom(DateTime date, string userTyp);
    }
}