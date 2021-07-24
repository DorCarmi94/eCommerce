using System;
using System.Collections.Generic;
using eCommerce.Common;
using eCommerce.Statistics.Repositories;

namespace eCommerce.Statistics
{
    public class Statistics : IStatisticsService
    {
        private static Statistics _instance = new Statistics();
        
        private StatsRepo _statsRepo;
        private List<Reciver> _recivers;

        private Statistics()
        {
            _statsRepo = new InMemoryStatsRepo();
            _recivers = new List<Reciver>();
        }
        
        private Statistics(StatsRepo statsRepo)
        {
            _statsRepo = statsRepo;
            _recivers = new List<Reciver>();
        }

        public static Statistics GetInstance()
        {
            return _instance;
        }
        
        public static Statistics GetInstanceForTests(StatsRepo statsRepo)
        {
            return new Statistics(statsRepo);
        }

        public void Init(AppConfig config)
        {
            StatsRepo statsRepo = null;
            
            string memoryAs = config.GetData("Memory");
            switch (memoryAs)
            {
                case "InMemory":
                {
                    statsRepo = new InMemoryStatsRepo();
                    break;
                }
                case "Persistence":
                {
                    statsRepo = new PersistenceStatsRepo();
                    break;
                }
                case null:
                {
                    config.ThrowErrorOfData("Memory", "missing");
                    break;
                }
                default:
                {
                    config.ThrowErrorOfData("Memory", "invalid");
                    break;
                }
            }

            _statsRepo = statsRepo;
        }

        public Result AddLoggedIn(DateTime dateTime, string username, string userType)
        {
            //dateTime = dateTime.Date;
            Result res = _statsRepo.AddLoginStat(new LoginStat(dateTime, username, userType));
            if (res.IsSuccess && dateTime.Date.Equals(DateTime.Now.Date))
            {
                Result<int> resNumber = _statsRepo.GetNumberOfLoginStatsFrom(dateTime.Date, userType);
                if (resNumber.IsSuccess)
                {
                    NotifyAll(userType, resNumber.Value);
                }
            }

            return res;
        }

        public Result<LoginDateStat> GetLoginStatsOn(DateTime date)
        {
            Result<List<LoginStat>> statsRes = _statsRepo.GetAllLoginStatsFrom(date);
            if (statsRes.IsFailure)
            {
                return Result.Fail<LoginDateStat>(statsRes.Error);
            }

            Dictionary<string, int> loginPerType = new Dictionary<string, int>();
            foreach (var loginStat in statsRes.Value)
            {
                if (!loginPerType.ContainsKey(loginStat.UserType))
                {
                    loginPerType[loginStat.UserType] = 1;
                    continue;
                }
                loginPerType[loginStat.UserType] = loginPerType[loginStat.UserType] + 1;
            }

            List<Tuple<string, int>> loginStats = new List<Tuple<string, int>>();
            foreach (var key in loginPerType.Keys)
            {
                loginStats.Add(new Tuple<string, int>(key, loginPerType[key]));
            }

            return Result.Ok(new LoginDateStat(loginStats));
        }

        public void Register(Reciver reciver)
        {
            _recivers.Add(reciver);
        }

        public void UnRegister(Reciver reciver)
        {
            _recivers.Remove(reciver);
        }

        public void NotifyAll(string userType, int number)
        {
            foreach (var reciver in _recivers)
            {
                reciver.ReciveBrodcast(userType, number);
            }
        }
    }
}