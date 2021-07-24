using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Runtime.InteropServices;

namespace eCommerce.Communication
{
    public class ConnectionRepository
    {
        public static ConnectionRepository Instance = new ConnectionRepository();
        
        private ConcurrentDictionary<string, IList<string>> _userToConnection;
        private ConcurrentDictionary<string, string> _connectionToUser;

        public ConnectionRepository()
        {
            _userToConnection = new ConcurrentDictionary<string, IList<string>>();
            _connectionToUser = new ConcurrentDictionary<string, string>();
        }
        
        public bool GetConnections(string userId, out IList<string> connectionIds)
        {
            return _userToConnection.TryGetValue(userId, out connectionIds);
        }

        public void AddConnection(string contextId, string userId)
        {
            if (!_userToConnection.TryRemove(userId, out var connectionList))
            {
                connectionList = new List<string>();
            }

            connectionList.Add(contextId);
            _userToConnection.TryAdd(userId, connectionList);
            _connectionToUser.TryAdd(contextId, userId);
        }


        /// <summary>
        /// Remove user connection by connection context id
        /// </summary>
        /// <param name="contextId">Connection context</param>
        /// <param name="userId">user id</param>
        /// <returns>Return the number of connection left open for the user</returns>
        public int RemoveConnection(string contextId, out string userId)
        {
            if (_connectionToUser.TryRemove(contextId, out userId) &
                _userToConnection.TryRemove(userId, out var connectionList))
            {
                connectionList.Remove(contextId);
                _userToConnection.TryAdd(userId, connectionList);
            }

            return connectionList.Count;
        }
    }
}