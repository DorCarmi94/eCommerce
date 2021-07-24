using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eCommerce;
using eCommerce.Auth;
using eCommerce.Common;

namespace Tests.Business.UserManagerTests
{
    /// <summary>
    /// <para>Implementation of concurrent.</para>
    /// <para>
    /// Assume Token is valid <br/>
    /// </para>
    /// </summary>
    public class UserAuthMock : IUserAuth
    {

        // token to name
        private ConcurrentDictionary<string, string> _connectedGuests;
        // username to auth
        private ConcurrentDictionary<string, AuthData> _registeredUsers;
        //name to password
        private ConcurrentDictionary<string, string> _passwords;

        private ConcurrentIdGenerator _idGenerator;
        
        public UserAuthMock()
        {
            _connectedGuests = new ConcurrentDictionary<string, string>();
            _registeredUsers = new ConcurrentDictionary<string, AuthData>();
            _passwords = new ConcurrentDictionary<string, string>();
            _idGenerator = new ConcurrentIdGenerator(0);
        }

        public void Init(AppConfig config)
        {
        }

        public async Task<Result> Register(string username, string password)
        {
            if (!_registeredUsers.TryAdd(username, new AuthData(username)))
            {
                return Result.Fail<string>("Username already exists");
            }

            _passwords.TryAdd(username, password);
            return Result.Ok();
        }

        public async Task<Result> Authenticate(string username, string password)
        {
            if (!(_registeredUsers.ContainsKey(username) && _passwords.TryGetValue(username, out var userPassword)))
            {
                return Result.Fail("Invalid username or password");
            }

            if (!userPassword.Equals(password))
            {
                return Result.Fail("Invalid username or password");
            }

            return Result.Ok();
        }

        public string GenerateToken(string username)
        {
            return $"{username}*{_idGenerator.MoveNext().ToString()}";
        }

        public bool IsValidToken(string token)
        {
            return GetData(token).IsSuccess;
        }
        

        public Result<AuthData> GetData(string token)
        {
            return Result.Ok(new AuthData(token.Split("*")[0]));
        }
    }
}