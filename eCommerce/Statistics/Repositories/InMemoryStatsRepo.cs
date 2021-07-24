using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Statistics.Repositories
{
    
    public class InMemoryStatsRepo : StatsRepo
    {

        private List<LoginStat> _statLogins;
        
        public InMemoryStatsRepo()
        {
            
            _statLogins = new List<LoginStat>();
        }
        
        public Result AddLoginStat(LoginStat stat)
        {
            lock (_statLogins)
            {
                _statLogins.Add(stat);
            }

            return Result.Ok();
        }

        public Result<List<LoginStat>> GetAllLoginStatsFrom(DateTime date)
        {
            List<LoginStat> loginStats = new List<LoginStat>();
            DateTime dateComponent = date.Date;
            lock (_statLogins)
            {
                foreach (var stat in _statLogins)
                {
                    if (stat.DateTime.Date.Equals(dateComponent))
                    {
                        loginStats.Add(stat);
                    }
                }
            }

            return Result.Ok(loginStats);
        }

        public Result<int> GetNumberOfLoginStatsFrom(DateTime date, string userType)
        {
            int number = 0;
            DateTime dateComponent = date.Date;
            lock (_statLogins)
            {
                foreach (var stat in _statLogins)
                {
                    if (stat.DateTime.Date.Equals(dateComponent) && stat.UserType.Equals(userType))
                    {
                        number++;
                    }
                }
            }

            return Result.Ok(number);
        }
    }
}