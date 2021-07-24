using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.Auth;
using eCommerce.Common;
using NUnit.Framework;

namespace Tests.AuthTests
{
    
    [TestFixture]
    public class UserAuthTests
    {
        private IUserAuth _userAuth;
        private IList<TUserData> _registeredUsers;
        private IList<TUserData> _userData;

        [SetUp]
        public void Setup()
        {
            _userAuth = UserAuth.CreateInstanceForTests(new InMemoryRegisteredUserRepo(), "ThisIsAKeyForTests");
            _registeredUsers = new List<TUserData>();
            _userData = new List<TUserData>
            {
                new TUserData("User1", "User1"),
                new TUserData("TheGreatStore", "ThisIsTheGreatestStorePassword")
            };
        }

        [Test]
        public void GenerateTokenForGuestTest()
        {
            string guestUsername = "_guest";
            for (int i = 0; i < 3; i++)
            {
                string generatedName = $"{guestUsername}{i}";
                string token = _userAuth.GenerateToken(generatedName);

                Result<AuthData> authDataRes = _userAuth.GetData(token);
                Assert.True(authDataRes.IsSuccess,
                    "Generated token from Connect is not valid");

                AuthData authData = authDataRes.Value;
                Assert.AreEqual(generatedName,
                    authData.Username,
                    "Token generated for the guest doesnt not have the matching role");

                Console.WriteLine($"Created guest name {authData.Username}");
            }
        }

        [Test, Order(1)]
        public async Task RegisterValidUsersTest()
        {
            foreach (var user in _userData)
            {
                Result registerRes = await _userAuth.Register(user.Username, user.Password);
                Assert.True(registerRes.IsSuccess,
                    $"User {user.Username} wasn't registered, Result Message: {registerRes.Error}");
                _registeredUsers.Add(user);

                Result authRes = await _userAuth.Authenticate(user.Username, user.Password);
                Assert.True(authRes.IsSuccess,
                    $"The user have been successfully register but the Auth class say it doesnt:\n" +
                    $"Error: {authRes.Error}");
            }
        }

        [Test, Order(2)]
        public async Task TryRegistersRegisteredUsersTest()
        {
            TUserData user = _userData[0];
            Result registerRes = await _userAuth.Register(user.Username, user.Password);
            Assert.True(registerRes.IsSuccess,
                $"User {user.Username} wasn't registered, Result Message: {registerRes.Error}");

            Result reRegisterRes = await _userAuth.Register(user.Username, user.Password);
            Assert.True(reRegisterRes.IsFailure,
                $"User {user.Username} already registered but he registered again\nResult Message: {registerRes.Error}");
        }
        
        [Test]
        public async Task RegisterInvalidUsersTest()
        {
            IList<TUserData> userData = new List<TUserData>
            {
                new TUserData(null, "User1"),
                new TUserData("TheGreatStore", null),
                new TUserData(null, null)
            };

            foreach (var user in userData)
            {
                Result registerRes = await _userAuth.Register(user.Username, user.Password);
                Assert.True(registerRes.IsFailure,
                    $"User {user.Username} with password {user.Password} was able to be registered\nResult Message: {registerRes.Error}");
            }
        }
    }
}