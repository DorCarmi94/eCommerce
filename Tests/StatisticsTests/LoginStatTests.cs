using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eCommerce.Common;
using eCommerce.Statistics;
using eCommerce.Statistics.Repositories;
using NUnit.Framework;

namespace Tests.StatisticsTests
{
    [TestFixture]
    public class LoginStatTests
    {
        private IStatisticsService _statisticsService;
        
        public LoginStatTests()
        {
        }

        [SetUp]
        public void SetUp()
        {
            _statisticsService = Statistics.GetInstanceForTests(new InMemoryStatsRepo());
        }

        [Test]
        [Order(1)]
        public void AddLoginsTest() 
        {
            Assert.True(_statisticsService.AddLoggedIn(DateTime.Now, "_Guest1", "guest").IsSuccess);
            Assert.True(_statisticsService.AddLoggedIn(DateTime.Now, "User1", "owner").IsSuccess);
            Assert.True(_statisticsService.AddLoggedIn(DateTime.Now.AddDays(1), "User2", "owner").IsSuccess);
        }
        
        [Test]
        [Order(2)]
        public void GetLoginOfTodayTest()
        {
            Result<LoginDateStat> loginStatsRes = _statisticsService.GetLoginStatsOn(DateTime.Now);
            Assert.True(loginStatsRes.IsSuccess);

            List<Tuple<string, int>> loginStats = loginStatsRes.Value.Stat;
            foreach (var stat in loginStats)
            {
                switch (stat.Item1)
                {
                    case "guest":
                        Assert.AreEqual(stat.Item2, 1);
                        break;
                    case "owner":
                        Assert.AreEqual(stat.Item2, 1);
                        break;
                    default:
                        Assert.Fail("Item doesnt need to exist");
                        break;
                }
            }
        }
        
        [Test]
        [Order(3)]
        public void ReciverTest() 
        {
            Assert.True(_statisticsService.AddLoggedIn(DateTime.Now, "_Guest1", "guest").IsSuccess);
            Assert.True(_statisticsService.AddLoggedIn(DateTime.Now, "User1", "owner").IsSuccess);

            MockReciver mockReciver = new MockReciver();
            _statisticsService.Register(mockReciver);
            
            Assert.True(_statisticsService.AddLoggedIn(DateTime.Now, "User2", "owner").IsSuccess);
            
            Thread.Sleep(TimeSpan.FromSeconds(5));
            Assert.AreEqual(mockReciver.Loggins["owner"], 2);
            Assert.True(mockReciver.NumberOfMessages["owner"] == 1);
        }
    }
}