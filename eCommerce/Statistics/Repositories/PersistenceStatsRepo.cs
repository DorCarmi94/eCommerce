using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Business;
using eCommerce.Common;
using eCommerce.Statistics.DAL;

namespace eCommerce.Statistics.Repositories
{
    public class PersistenceStatsRepo : StatsRepo
    {
        protected readonly StatsContextFactory _contextFactory;

        public PersistenceStatsRepo()
        {
            _contextFactory = new StatsContextFactory();
        }
        
        public PersistenceStatsRepo(StatsContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public Result AddLoginStat(LoginStat stat)
        {
            try
            {
                using (var context = _contextFactory.Create())
                {
                    context.Add(stat);
                    context.SaveChanges();
                }
            } catch (Exception e)
            {
                if (!CheckConnection())
                {
                    MarketState.GetInstance().SetErrorState("Bad connection to db",this.CheckConnection);
                }
                return Result.Fail("Error saving data");
            }

            return Result.Ok();
        }

        public Result<List<LoginStat>> GetAllLoginStatsFrom(DateTime date)
        {
            try
            {
                using (var context = _contextFactory.Create())
                {
                    return Result.Ok(context.Login.Where(ls => ls.DateTime.Date.Equals(date.Date)).ToList());
                }
            }
            catch (Exception e)
            {
                if (!CheckConnection())
                {
                    MarketState.GetInstance().SetErrorState("Bad connection to db",this.CheckConnection);
                }
                return Result.Fail<List<LoginStat>>("Error saving data");
            }
        }

        public Result<int> GetNumberOfLoginStatsFrom(DateTime date, string userTyp)
        {
            try
            {
                using (var context = _contextFactory.Create())
                {

                    return Result.Ok(context.Login.Count(ls => ls.DateTime.Date.Equals(date.Date) &&
                                                               ls.UserType.Equals(userTyp)));
                }
            }
            catch (Exception e)
            {
                if (!CheckConnection())
                {
                    MarketState.GetInstance().SetErrorState("Bad connection to db",this.CheckConnection);
                }
                return Result.Fail<int>("Error saving data");
            }
        }
        
        public bool CheckConnection()
        {
            return _contextFactory.Create().Database.CanConnect();
        }
    }
}