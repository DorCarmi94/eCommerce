using System.Collections.Concurrent;
using eCommerce.Common;

namespace eCommerce.Business.Repositories
{
    public class InMemoryRegisteredUsersRepository : IRepository<User>
    {

        private ConcurrentDictionary<string, User> _users;

        public InMemoryRegisteredUsersRepository()
        {
            _users = new ConcurrentDictionary<string, User>();
        }
        
        public bool Add(User user)
        {
            return _users.TryAdd(user.Username, user);
        }

        public User GetOrNull(string id)
        {
            if (!_users.TryGetValue(id, out var data))
            {
                return null;
            }

            return data;
        }

        
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Remove(string id)
        {
            throw new System.NotImplementedException();
        }

        public void Update(User data)
        {
            
        }
    }
}