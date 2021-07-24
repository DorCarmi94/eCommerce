using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using eCommerce.Statistics;

namespace Tests.StatisticsTests
{
    public class MockReciver : Reciver
    {
        public Dictionary<string, int> NumberOfMessages { get; set; }

        public Dictionary<string, int> Loggins { get; set; }

        public MockReciver()
        {
            Loggins = new Dictionary<string, int>();
            NumberOfMessages = new Dictionary<string, int>();

        }

        public void ReciveBrodcast(string userType, int number)
        {
            if (!Loggins.ContainsKey(userType))
            {
                Loggins.Add(userType, number);
                NumberOfMessages.Add(userType, 1);
            }
            else
            {
                Loggins[userType] = number;
                NumberOfMessages[userType] = NumberOfMessages[userType] + 1;
            }
        }
    }
}