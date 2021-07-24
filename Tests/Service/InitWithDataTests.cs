using System.IO;
using eCommerce;
using eCommerce.Business;

using eCommerce.Service;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Tests.Service
{
    [TestFixture]
    public class InitWithDataTests
    {
        private InitSystemWithData _initSystemWithData;
        private MarketFacadeMockForInitData _marketFacade;
        private IAuthService _authService;
        private IUserService _userService;
        private InStoreService _storeService;

        public InitWithDataTests()
        {
            _marketFacade = new MarketFacadeMockForInitData();
            _authService = AuthService.CreateUserServiceForTests(_marketFacade);
            _userService = UserService.CreateUserServiceForTests(_marketFacade);
            _storeService = InStoreService.CreateUserServiceForTests(_marketFacade);
            _initSystemWithData = new InitSystemWithData(_authService, _userService, _storeService);

        }
        [SetUp]
        public void Setup()
        {
            _marketFacade.Clear();
        }
        
        [Test]
        [Order(1)]
        public void NotExistingInitFileTest()
        {
            Assert.False(_initSystemWithData.Init(""));
        }
        
        [Test]
        [Order(2)]
        public void EmptyInitFileTest()
        {
            Assert.True(_initSystemWithData.Init("..\\..\\..\\Service\\empty.json"));
        }
        
        [Test]
        [Order(3)]
        public void RegisterInitFileTest()
        {
            Assert.True(_initSystemWithData.Init("..\\..\\..\\Service\\simpleInit.json"));
            Assert.AreEqual(1, _marketFacade.RegisteredNumber);
        }
        
        [Test]
        public void OpenStoreTest()
        {
            Assert.True(_initSystemWithData.Init("..\\..\\..\\Service\\openStoreInit.json"));
            
            Assert.AreEqual(1, _marketFacade.OpenedStores);
            
            Assert.True(_marketFacade.LoginsNumber > 0 && _marketFacade.LogoutsNumber > 0, "Need to login and logout in member action");
            Assert.AreEqual(_marketFacade.LoginsNumber, _marketFacade.LogoutsNumber);

        }
    }
}