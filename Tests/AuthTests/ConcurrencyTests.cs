using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Auth;
using eCommerce.Common;
using NUnit.Framework;

namespace Tests.AuthTests
{
    [TestFixture]
    public class ConcurrencyTests
    {

        private IUserAuth _auth; 
        
        [SetUp]
        public void Setup()
        {
            _auth = UserAuth.CreateInstanceForTests(new InMemoryRegisteredUserRepo(), "ThisIsAKeyForTests");
        }

        [Test]
        public async Task ConcurrentGenerateTokenTest()
        {
            ConcurrentIdGenerator idGenerator = new ConcurrentIdGenerator(0);
            const int numberOfTasks = 10;
            string[] tokens = await TaskTestUtils.CreateAndRunTasks(
                () => _auth.GenerateToken(idGenerator.MoveNext().ToString()),
                numberOfTasks);
            
            ISet<string> usernames = new HashSet<string>();
            
            for (var i = 0; i < numberOfTasks; i++)
            {
                Result<AuthData> authData = _auth.GetData(tokens[i]);
                Assert.True(authData.IsSuccess, 
                    $"The guest is connect therefore the token should be valid\nError: {authData.Error}");
                usernames.Add(authData.Value.Username);
                Console.WriteLine($"Created name: {authData.Value.Username}");
            }
            
            Assert.AreEqual(numberOfTasks,
                usernames.Count,
                $"There are duplicate usernames");
        }
        
        [Test, Repeat(2)]
        public async Task ConcurrentRegisterSameUserTest()
        {
            const int numberOfTasks = 5;
            Task<Result>[] registerTasks = new Task<Result>[numberOfTasks];
            for (int i = 0; i < numberOfTasks; i++)
            {
                registerTasks[i] = _auth.Register("user1", "password1");
            }

            Result[] registerRes = await Task.WhenAll<Result>(registerTasks);

            int registeredSuccessfully = 0;
            foreach (var res in registerRes)
            {
                if (res.IsSuccess)
                {
                    registeredSuccessfully++;
                }
            }
            
            Assert.AreEqual(1,
                registeredSuccessfully,
                $"Only one task should has been able to register but {registeredSuccessfully} succeeded");
        }
    }
}