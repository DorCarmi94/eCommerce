using eCommerce.Statistics.DAL;
using Microsoft.EntityFrameworkCore;

namespace Tests.StatisticsTests
{
    public class TestsStatsDbContext: StatsContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=localhost;DataBase=statsTests;Trusted_Connection=True;");
        }
    }
}