using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using eCommerce.Common;

namespace eCommerce.Auth
{
    public class InMemoryRegisteredUserRepo : IRegisteredUserRepo
    {
        private ConcurrentDictionary<string, AuthUser> _dictionary;

        public InMemoryRegisteredUserRepo()
        {
            _dictionary = new ConcurrentDictionary<string, AuthUser>();
        }
        
        public async Task<bool> Add(AuthUser authUser)
        {
            return _dictionary.TryAdd(authUser.Username, authUser);
        }
        
        public async Task<AuthUser> GetUserOrNull(string username)
        {
            AuthUser authUser;
            if (!_dictionary.TryGetValue(username, out authUser))
            {
                authUser = null;
            }
            return authUser;
        }
    }
}