using System;
using System.Collections.Generic;
using eCommerce.Common;
using eCommerce.Statistics;
using eCommerce.Statistics.Repositories;
using NUnit.Framework;

namespace Tests.StatisticsTests
{
    [TestFixture]
    public class PersistenceRepoTests
    {
        private TestsPersistenceRepoExtender _repo;
        public PersistenceRepoTests()
        {
            _repo = new TestsPersistenceRepoExtender();
            _repo.CleanDB();
        }

        [Test]
        [Order(1)]
        public void AddLoginsTest()
        {
            Assert.True(_repo.AddLoginStat(new LoginStat(DateTime.Now, "_Guest1", "guest")).IsSuccess);
            Assert.True(_repo.AddLoginStat(new LoginStat(DateTime.Now, "User1", "owner")).IsSuccess);
            Assert.True(_repo.AddLoginStat(new LoginStat(DateTime.Now.AddDays(1), "User2", "owner")).IsSuccess);
        }
        
        [Test]
        [Order(2)]
        public void GetLoginOfTodayTest()
        {
            Result<List<LoginStat>> loginStatsRes = _repo.GetAllLoginStatsFrom(DateTime.Now);
            Assert.True(loginStatsRes.IsSuccess);

            foreach (var stat in loginStatsRes.Value)
            {
                switch (stat.Username)
                {
                    case "_Guest1":
                        break;
                    case "User1":
                        break;
                    default:
                        Assert.Fail("Item doesnt need to exist");
                        break;
                }
            }
        }
        
        [Test]
        [Order(3)]
        public void NumberOfLoginTodayTest()
        {
            Result<int> loginStatsGuestRes = _repo.GetNumberOfLoginStatsFrom(DateTime.Now, "guest");
            Result<int> loginStatsOwnerTodayRes = _repo.GetNumberOfLoginStatsFrom(DateTime.Now, "owner");
            Result<int> loginStatsOwnerNextDayRes = _repo.GetNumberOfLoginStatsFrom(DateTime.Now, "owner");
            
            Assert.True(loginStatsGuestRes.IsSuccess);
            Assert.True(loginStatsOwnerTodayRes.IsSuccess);
            Assert.True(loginStatsOwnerNextDayRes.IsSuccess);

            Assert.AreEqual(1, loginStatsGuestRes.Value);
            Assert.AreEqual(1, loginStatsOwnerTodayRes.Value);
            Assert.AreEqual(1, loginStatsOwnerNextDayRes.Value);
        }
    }
}