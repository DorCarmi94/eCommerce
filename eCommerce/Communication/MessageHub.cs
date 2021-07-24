using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Common;
using eCommerce.Publisher;
using eCommerce.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;

namespace eCommerce.Communication
{
    public class MessageModel
    {
        public string Message { get; set; }

        public MessageModel(string message)
        {
            Message = message;
        }
    }
    
    public class MessageHub : Hub
    {
        private IHubContext<MessageHub> _hubContext = null;
        
        private MainPublisher _mainPublisher;
        private ConnectionRepository _connectionRepository;
        private IAuthService _authService;
        private IUserService _userService;
        public MessageHub(IHubContext<MessageHub> hubContext)
        {
            _hubContext = hubContext;
            _mainPublisher = MainPublisher.Instance;
            _connectionRepository = ConnectionRepository.Instance;
            _authService = new AuthService();
            _userService = new UserService();
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
            //Clients.Client(Context.ConnectionId).SendAsync("ReceiveConnID", Context.ConnectionId);
            
            var userBasicData = _userService.GetUserBasicInfo(authToken).Value;
            var userId = userBasicData.Username;
            _connectionRepository.AddConnection(Context.ConnectionId, userId);

            Console.WriteLine("--> Connection Opened: " + Context.ConnectionId);
            _mainPublisher.Connect(userId);
            return base.OnConnectedAsync();
        }
        
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("Close connection");
            if (_connectionRepository.RemoveConnection(Context.ConnectionId, out var userId) == 0 &&
                userId != null)
            {
                Console.WriteLine("--> Connection Closed: " + Context.ConnectionId);
                _mainPublisher.Disconnect(userId);
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}