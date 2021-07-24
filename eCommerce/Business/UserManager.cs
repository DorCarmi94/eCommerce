using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using eCommerce.Auth;

using eCommerce.Common;
using eCommerce.Statistics;
using NLog;

namespace eCommerce.Business
{
    public class UserManager
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private IUserAuth _auth;
        
        // token to user
        private ConcurrentDictionary<string, User> _connectedUsers;
        private ConcurrentDictionary<string, bool> _connectedUsersName;
        private IRepository<User> _registeredUsersRepo;

        private ConcurrentIdGenerator _concurrentIdGenerator;
        private IStatisticsService _statisticsService;


        public UserManager(IUserAuth auth, IRepository<User> registeredUsersRepo)
        {
            _auth = auth;
            _connectedUsers = new ConcurrentDictionary<string, User>();
            _connectedUsersName = new ConcurrentDictionary<string, bool>();
            // TODO get the initialze id value from DB
            _concurrentIdGenerator = new ConcurrentIdGenerator(0);
            _registeredUsersRepo = registeredUsersRepo;
            _statisticsService = Statistics.Statistics.GetInstance();
        }

        public string Connect()
        {
            string guestUsername = GenerateGuestUsername();
            string token = _auth.GenerateToken(guestUsername);

            User newUser = CreateGuestUser(guestUsername);
            _connectedUsers.TryAdd(token, newUser);

            DateTime date = DateTime.Now;
            _logger.Info($"New guest: {guestUsername}");
            if (_statisticsService.AddLoggedIn(date, guestUsername, newUser.GetUserCategory()).IsFailure)
            {
                _logger.Warn($"Didnt add to stats {guestUsername} at {date} as guest");
            }
            return token;
        }
        
        public void Disconnect(string token)
        {
            if (!_auth.IsValidToken(token))
            {
                _logger.Warn($"Invalid token {token}");
                if (token != null)
                {
                    _connectedUsers.TryRemove(token, out var tuser);
                }

                return;
            }

            if (_connectedUsers.TryGetValue(token, out var user)
                && user.GetState() == Guest.State)
            {
                _connectedUsers.TryRemove(token, out user);
                _logger.Info($"Guest: {user?.Username} disconnected");

            }
        }

        public async Task<Result> Register(string token, MemberInfo memberInfo, string password)
        {
            if (!_auth.IsValidToken(token))
            {
                _logger.Warn($"Invalid token {token}");
                if (token != null)
                {
                    _connectedUsers.TryRemove(token, out var tuser);
                }

                return Result.Fail("Invalid token");
            }

            if (!_connectedUsers.TryGetValue(token, out var user))
            {
                return Result.Fail("User need to be connected or logged in");
            }
            
            Result validMemberInfoRes = IsValidMemberInfo(memberInfo);
            if (validMemberInfoRes.IsFailure)
            {
                return validMemberInfoRes;
            }

            Result authRegistrationRes = await RegisterAtAuthorization(memberInfo.Username, password);
            if (authRegistrationRes.IsFailure)
            {
                return authRegistrationRes;
            }
            
            User newUser = new User(Member.State, memberInfo.Clone());
            if (!_registeredUsersRepo.Add(newUser))
            {
                // TODO maybe remove the user form userAuth
                _logger.Error($"User {memberInfo.Username} was able to register at Auth but already exists in " +
                    "the registered user repository");
                return Result.Fail("User already exists");
            }

            _logger.Info($"User {memberInfo.Username} was registered");
            return Result.Ok();
        }

        public void AddAdmin(MemberInfo adminInfo, string password)
        {
            User user = new User(Admin.State, adminInfo);
            RegisterAtAuthorization(adminInfo.Username, password).Wait();
            _registeredUsersRepo.Add(user);
        }
        
        private Task<Result> RegisterAtAuthorization(string username, string password)
        {
            return _auth.Register(username, password);
        }
        
        public async Task<Result<string>> Login(string guestToken, string username, string password, UserToSystemState role)
        {
            if (!_auth.IsValidToken(guestToken))
            {
                _logger.Warn($"Invalid token {guestToken}");
                if (guestToken != null)
                {
                    _connectedUsers.TryRemove(guestToken, out var tUser);
                }

                return Result.Fail<string>("Invalid token");
            }

            if (!_connectedUsers.TryGetValue(guestToken, out var guestUser) || guestUser.GetState() != Guest.State)
            {
                return Result.Fail<string>("Not connected or not guest");
            }

            Result authLoginRes = await _auth.Authenticate(username, password);
            if (authLoginRes.IsFailure)
            {
                return Result.Fail<string>(authLoginRes.Error);
            }

            if (_connectedUsersName.ContainsKey(username))
            {
                return Result.Fail<string>("User is already logged in");
            }
            
            User user = _registeredUsersRepo.GetOrNull(username);
            if (user == null)
            {
                _logger.Error($"User {username} is registered in auth, but not in usermanger");
                return Result.Fail<string>("Invalid username or password");
            }

            if (role == Admin.State & user.IsAdmin().IsFailure)
            {
                return Result.Fail<string>("User is not a admin");
            }

            if (!_connectedUsers.TryRemove(guestToken, out var tUser1))
            {
                return Result.Fail<string>("Guest not connected");
            }
            
            string loginToken = _auth.GenerateToken(username);

            if (!_connectedUsers.TryAdd(loginToken, user) || !_connectedUsersName.TryAdd(username, true))
            {
                _logger.Error($"UserAuth created duplicate toekn(already in connected userses dictionry)");
                return Result.Fail<string>("Error");
            }
            
            DateTime date = DateTime.Now;
            string category = user.GetUserCategory();
            if (_statisticsService.AddLoggedIn(date, username, category).IsFailure)
            {
                _logger.Warn($"Didnt add to stats {username} at {date} as {category}");
            }
            
            _logger.Info($"User {user.Username} logged in. Token {loginToken}");
            return Result.Ok(loginToken);
        }
        
        public Result<string> Logout(string token)
        {

            if (!_auth.IsValidToken(token))
            {
                _logger.Warn($"Invalid token {token}");
                if (token != null)
                {
                    _connectedUsers.TryRemove(token, out var tUser);
                }

                return Result.Fail<string>("Invalid token");
            }
            
            if (!(_connectedUsers.TryGetValue(token, out var user) && user.GetState() != Guest.State))
            {
                return Result.Fail<string>("Guest cant logout");
            }

            if (!_connectedUsers.TryRemove(token, out var tUser1) || 
                !_connectedUsersName.TryRemove(user.Username, out var tbool))
            {
                _logger.Error($"User logout error");
            }
            else
            {
                _logger.Info($"User {tUser1.Username} logout");
            }
            
            return Result.Ok(Connect());
        }
        
        public bool IsUserConnected(string token)
        {
            if (!_auth.IsValidToken(token))
            {
                _logger.Info($"Invalid use of token {token}");
                if (token != null)
                {
                    _connectedUsers.TryRemove(token, out var tUser);
                }

                return false;
            }

            return _connectedUsers.TryGetValue(token, out var user);
        }

        public Result<User> GetUserIfConnectedOrLoggedIn(string token)
        {
            if (!_auth.IsValidToken(token))
            {
                _logger.Info($"Invalid use of token {token}");
                if (token != null)
                {
                    _connectedUsers.TryRemove(token, out var tUser);
                }

                return Result.Fail<User>("Invalid token");
            }

            if (!_connectedUsers.TryGetValue(token, out var user))
            {
                _logger.Info($"Usage of old token {token}");
                return Result.Fail<User>("User not connected or logged in");
            }

            return Result.Ok(user);
        }
        
        public Result<User> GetUserLoggedIn(string token)
        {
            if (!_auth.IsValidToken(token))
            {
                _logger.Info($"Invalid use of token {token}");
                if (token != null)
                {
                    _connectedUsers.TryRemove(token, out var tUser);
                }

                return Result.Fail<User>("Invalid token");
            }

            if (!_connectedUsers.TryGetValue(token, out var user))
            {
                _logger.Info($"Usage of old token {token}");
                return Result.Fail<User>("User not logged in");
            }

            if (user.GetState() == Guest.State)
            {
                return Result.Fail<User>("This is a guest user");
            }
            
            return Result.Ok(user);
        }
        
        /// <summary>
        /// Get user
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>The user</returns>
        public Result<User> GetUser(string username)
        {
            User user = _registeredUsersRepo.GetOrNull(username);
            if (user == null)
            {
                return Result.Fail<User>("User doesn't exists");
            }

            return Result.Ok(user);
        }

        private User CreateGuestUser(string guestName)
        {
            return new User(guestName);
        }
        
        /// <summary>
        /// Check if the member information is valid
        /// </summary>
        /// <returns>Result of the check</returns>
        private Result IsValidMemberInfo(MemberInfo memberInfo)
        {
            Result fullDataRes = memberInfo.IsBasicDataFull();
            if (fullDataRes.IsFailure)
            {
                return fullDataRes;
            }

            if (!IsValidUsername(memberInfo.Username))
            {
                return Result.Fail("Invalid username address");
            }

            if (!RegexUtils.IsValidEmail(memberInfo.Email))
            {
                return Result.Fail("Invalid email address");
            }

            return Result.Ok();
        }

        private bool IsValidUsername(string username)
        {
            return Regex.IsMatch(username,
            "^[a-zA-z][a-zA-z0-9]*$");
        }

        private long GetAndIncrementGuestId()
        {
            return _concurrentIdGenerator.MoveNext();
        }
        
        private string GenerateGuestUsername()
        {
            return $"_Guest{GetAndIncrementGuestId():D}";
        }

        public void CreateMainAdmin()
        {
            AppConfig config = AppConfig.GetInstance();
            string adminUsername = config.GetData("AdminCreationInfo:Username");
            string adminPassword = config.GetData("AdminCreationInfo:Password");
            string adminEmail = config.GetData("AdminCreationInfo:Email");

            if (adminUsername == null | adminPassword == null | adminEmail == null)
            {
                adminUsername = "_Admin";
                adminPassword = "_Admin";
                adminEmail = "_Admin@eCommernce.com";
            }

            if (_auth.Authenticate(adminUsername, adminPassword).Result.IsFailure)
            {
                MemberInfo adminInfo = new MemberInfo(
                    adminUsername,
                    adminEmail,
                    "TheAdmin",
                    DateTime.Now,
                    null);
                AddAdmin(adminInfo, adminPassword);
            }
        }

        public void UpdateUser(User user)
        {
            _registeredUsersRepo.Update(user);
        }
    }
}