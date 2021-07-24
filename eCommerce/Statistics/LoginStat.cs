using System;

namespace eCommerce.Statistics
{
    public class LoginStat
    {
        public DateTime DateTime { get; set; }
        public string Username { get; set; }
        public string UserType { get; set; }

        public LoginStat(DateTime dateTime, string username, string userType)
        {
            DateTime = dateTime;
            Username = username;
            UserType = userType;
        }
    }
}