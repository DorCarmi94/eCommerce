using System;
using System.Collections.Concurrent;
using eCommerce.Publisher;
using Microsoft.AspNetCore.SignalR;

namespace eCommerce.Communication
{
    public class MessageConnectionHandler : UserObserver
    {
        private IHubContext<MessageHub> _hubContext = null;
        private MainPublisher _mainPublisher;
        private ConnectionRepository _connectionRepository;


        public MessageConnectionHandler(IHubContext<MessageHub> hubContext)
        {
            _hubContext = hubContext;
            _mainPublisher = MainPublisher.Instance;
            _mainPublisher.Register(this);
            _connectionRepository = ConnectionRepository.Instance;
        }

        public async void Notify(string userId, ConcurrentQueue<string> messages)
        {
            if (!_connectionRepository.GetConnections(userId, out var connectionsIds))
            {
                return;
            }

            Console.WriteLine($"Notify user {userId}");
            while (!messages.IsEmpty)
            {
                if (messages.TryDequeue(out var message))
                {
                    foreach (var connectionId in connectionsIds)
                    {
                        try
                        {
                            await _hubContext.Clients.Client(connectionId).SendAsync("message", new MessageModel(message));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
            }
        }
    }
}