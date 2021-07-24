using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Common;
using eCommerce.Publisher;
using eCommerce.Service;
using eCommerce.Statistics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;

namespace eCommerce.Communication
{
    public class StatsMessageModel
    {
        public string UserType { get; set; }
        
        public int Amount { get; set; }

        public StatsMessageModel(string userType, int amount)
        {
            UserType = userType;
            Amount = amount;
        }
    }
    
    public class StatsHub : Hub
    {
        private IHubContext<MessageHub> _hubContext = null;
        
        private IStatisticsService _statistics;
        private IAuthService _authService;
        public StatsHub(IHubContext<MessageHub> hubContext)
        {
            _hubContext = hubContext;
            _statistics = Statistics.Statistics.GetInstance();
            _authService = new AuthService();
        }

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext == null)
            {
                // close connection
                return base.OnDisconnectedAsync(null);
            }

            var authToken = httpContext.Request.Cookies["_auth"];
            if (!_authService.IsUserConnected(authToken))
            {
                return base.OnDisconnectedAsync(null);
            }

            Console.WriteLine("--> Stats Connection Opened: " + Context.ConnectionId);
            return base.OnConnectedAsync();
        }
        
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("Stats close connection");
            return base.OnDisconnectedAsync(exception);
        }
    }
}