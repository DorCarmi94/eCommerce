using System;
using eCommerce.Common;

namespace eCommerce.Statistics
{
    public interface IStatisticsService : Brodcaster
    {
        
        /// <summary>
        /// Init the Auth, must be called before use.
        /// </summary>
        /// <param name="config"></param>
        public void Init(AppConfig config);
        
        /// <summary>
        /// Add a logged of a user to the records
        /// </summary>
        /// <param name="dateTime">The date and time user logged in</param>
        /// <param name="username">The username</param>
        /// <param name="userType">The user type</param>
        public Result AddLoggedIn(DateTime dateTime, string username, string userType);
        
        /// <summary>
        /// Return the login stats on that date
        /// </summary>
        /// <param name="date">The required date</param>
        /// <returns>The states for that date</returns>
        public Result<LoginDateStat> GetLoginStatsOn(DateTime date);
        
        
    }
}