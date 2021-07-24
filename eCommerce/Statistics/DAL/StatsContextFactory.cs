using eCommerce.Auth.DAL;

namespace eCommerce.Statistics.DAL
{
    public class StatsContextFactory
    {
        public virtual StatsContext Create()
        {
            return new StatsContext();
        }
    }
}