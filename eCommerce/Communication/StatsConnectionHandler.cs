using System;
using System.Collections.Concurrent;
using eCommerce.Publisher;
using eCommerce.Statistics;
using Microsoft.AspNetCore.SignalR;

namespace eCommerce.Communication
{
    public class StatsConnectionHandler : Reciver
    {
        private IHubContext<StatsHub> _hubContext = null;
        private IStatisticsService _statistics;


        public StatsConnectionHandler(IHubContext<StatsHub> hubContext)
        {
            _hubContext = hubContext;
            _statistics = Statistics.Statistics.GetInstance();
            _statistics.Register(this);
        }

        public void ReciveBrodcast(string userType, int number)
        {
            _hubContext.Clients.All.SendAsync("LoginUpdate", new StatsMessageModel(userType, number));
        }
    }
}