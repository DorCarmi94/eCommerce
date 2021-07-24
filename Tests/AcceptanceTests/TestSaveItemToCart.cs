using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using eCommerce.Auth;
using eCommerce.Business;
using eCommerce.Business.Repositories;
using eCommerce.Common;
using eCommerce.Service;
using NUnit.Framework;

namespace Tests.AcceptanceTests
{
    /// <summary>
    /// <UC>
    /// Save items in a shopping cart
    /// </UC>
    /// <Req>
    /// 2.7
    /// </Req>
    /// </summary>
    [TestFixture]
    [Order(16)]
    public class TestSaveItemToCart
    {
        private IAuthService _auth;
        private INStoreService _inStore;
        private ICartService _cart;
        private IUserService _user;
        private string store = "halfords";
        
        
        private MemberInfo Gedalia;
        private string goodPassword;
        private MemberInfo Guy;
        private MemberInfo Raviv;
        private MemberInfo Rinat;
        private IItem pumpkin;
        private IItem pomela;
        private string yossi = "Yossi1192";
        
        
        [SetUpAttribute]
        public async Task SetUp()
        {
            InMemoryRegisteredUserRepo RP = new InMemoryRegisteredUserRepo();
            UserAuth UA = UserAuth.CreateInstanceForTests(RP, "ThisKeyIsForTests");
            InMemoryStoreRepo SR = new InMemoryStoreRepo();
            IRepository<User> UR = new InMemoryRegisteredUsersRepository();
            IMarketFacade marketFacade = MarketFacade.CreateInstanceForTests(UA,UR, SR);

            _auth = AuthService.CreateUserServiceForTests(marketFacade);
            _inStore = InStoreService.CreateUserServiceForTests(marketFacade);
            _cart = CartService.CreateUserServiceForTests(marketFacade);
            _user = UserService.CreateUserServiceForTests(marketFacade);
            MemberInfo yossi = new MemberInfo("Yossi1192", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("singerFrog", "shiran@gmail.com", "Shiran Moris",
                DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            MemberInfo lior = new MemberInfo("Barov","lior@gmail.com", "Lior Lee", 
                DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
            
            Gedalia = new MemberInfo("Gedalia","lior@gmail.com", "Lior Lee", 
                DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
            
            Guy = new MemberInfo("Guy","lior@gmail.com", "Lior Lee", 
                DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
            
            Raviv = new MemberInfo("Raviv","lior@gmail.com", "Lior Lee", 
                DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
            
            Rinat = new MemberInfo("Rinat","lior@gmail.com", "Lior Lee", 
                DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
            
            
            string token = _auth.Connect();
            await _auth.Register(token, yossi, "qwerty123");
            await _auth.Register(token, shiran, "130452abc");
            await _auth.Register(token, lior, "987654321");

            goodPassword = "987654321";
            
            await _auth.Register(token, Gedalia, goodPassword);
            await _auth.Register(token, Guy, goodPassword);
            await _auth.Register(token, Raviv, goodPassword);
            await _auth.Register(token, Rinat, goodPassword);
            
            
            Result<string> yossiLogInResult = await _auth.Login(token, this.yossi, "qwerty123", ServiceUserRole.Member);
            IItem product = new SItem("Tara milk", store, 10, "dairy",
                new List<string>{"dairy", "milk", "Tara"}, (double)5.4);
            
            pumpkin = new SItem("Pumpkin", store, 200, "fruit",
                new List<string>{"fruit"}, (double)19.5);
            
            pomela = new SItem("Pomela", store, 200, "fruit",
                new List<string>{"fruit"}, (double)19.5);
            
            _inStore.OpenStore(yossiLogInResult.Value, store);
            _inStore.AddNewItemToStore(yossiLogInResult.Value, product);
            _inStore.AddNewItemToStore(yossiLogInResult.Value, pumpkin);
            _inStore.AddNewItemToStore(yossiLogInResult.Value, pomela);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TearDownAttribute]
        public void Teardown()
        {
            _auth = null;
            _inStore = null;
            _cart = null;
        }
        
        //TODO:Check
        [TestCase("Tara milk", "halfords", 3)]
        [TestCase("Tara milk", "halfords", 5)]
        [Test]
        public async Task TestAddItemToCartSuccess(string itemId, string storeName, int amount)
        { 
            string token = _auth.Connect();
            Result<string> login = await _auth.Login(token, "singerFrog", "130452abc", ServiceUserRole.Member);
            Assert.True(login.IsSuccess, login.Error);
            Result result = _cart.AddItemToCart(login.Value, itemId, storeName, amount);
            Assert.True(result.IsSuccess, "failed to add  " + " from " + storeName + " to cart: " + result.Error);
            token = _auth.Logout(login.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("Tnuva cream cheese", "halfords", 3)]
        [TestCase("Tara milk", "halfords", -3)]
        [TestCase("Tara milk", "halfords", 12)]
        [Test] 
        public async Task TestAddItemToCartFailure(string itemId, string storeName, int amount)
        { 
            string token = _auth.Connect();
            Result<string> login = await _auth.Login(token, "singerFrog", "130452abc", ServiceUserRole.Member);
            Result result = _cart.AddItemToCart(login.Value, itemId, storeName, amount);
            Assert.True(result.IsFailure, "action was suppose to fail!");
            token = _auth.Logout(login.Value).Value;
            _auth.Disconnect(token);
        }
        
        [Test]
        public async Task TestBidSuccess()
        { 
            string token = _auth.Connect();
            string yossiPass = "qwerty123";
            int newPrice = 10;
            Result<string> loginGedalia = await _auth.Login(token, Gedalia.Username, goodPassword, ServiceUserRole.Member);
            Assert.True(loginGedalia.IsSuccess,loginGedalia.Error);
            var bidRes=_inStore.AskToBidOnItem(loginGedalia.Value, pumpkin.ItemName, store, 30, newPrice);
            Assert.True(bidRes.IsSuccess,bidRes.Error);

            string tokenyossi = _auth.Connect();
            var resLoginYossi = await _auth.Login(tokenyossi, yossi, "qwerty123", ServiceUserRole.Member);
            Assert.True(resLoginYossi.IsSuccess,resLoginYossi.Error);

            var resBidsInfos = _inStore.GetAllBidsWaitingToApprove(resLoginYossi.Value, store);
            Assert.True(resBidsInfos.IsSuccess,resBidsInfos.Error);
            Assert.True(resBidsInfos.Value.Count>0);

            var resApproveBid =
                _inStore.ApproveOrDisapproveBid(resLoginYossi.Value, store, resBidsInfos.Value[0].BidID, true);
            
            Assert.True(resApproveBid.IsSuccess);

            var cartRes=_cart.GetCart(loginGedalia.Value).Value;
            var basketRes=cartRes.Baskets.FirstOrDefault(x => x.StoreId.Equals(store));
            Assert.NotNull(basketRes);

            var itemInBasket = basketRes.Items.FirstOrDefault(x => x.ItemName.Equals(this.pumpkin.ItemName));
            Assert.NotNull(itemInBasket);
            
            Assert.AreEqual(newPrice,itemInBasket.PricePerUnit);
            
            token = _auth.Logout(loginGedalia.Value).Value;
            _auth.Disconnect(token);
            
            tokenyossi = _auth.Logout(resLoginYossi.Value).Value;
            _auth.Disconnect(tokenyossi);
        }
        
        [Test]
        public async Task TestBidMultipleApprove()
        { 
            string token = _auth.Connect();
            string yossiPass = "qwerty123";
            int newPrice = 10;
            

            string tokenyossi = _auth.Connect();
            var resLoginYossi = await _auth.Login(tokenyossi, yossi, "qwerty123", ServiceUserRole.Member);
            Assert.True(resLoginYossi.IsSuccess,resLoginYossi.Error);
            var resAppointGuy=_user.AppointCoOwner(resLoginYossi.Value, store, Guy.Username);
            var resAppointRaviv=_user.AppointCoOwner(resLoginYossi.Value, store, Raviv.Username);
            var resAppointRinat=_user.AppointCoOwner(resLoginYossi.Value, store, Rinat.Username);
            
            Assert.True(resAppointGuy.IsSuccess,resAppointGuy.Error);
            Assert.True(resAppointRaviv.IsSuccess,resAppointRaviv.Error);
            Assert.True(resAppointRinat.IsSuccess,resAppointRinat.Error);
            
            
            Result<string> loginGedalia = await _auth.Login(token, Gedalia.Username, goodPassword, ServiceUserRole.Member);
            Assert.True(loginGedalia.IsSuccess,loginGedalia.Error);
            var bidRes=_inStore.AskToBidOnItem(loginGedalia.Value, pomela.ItemName, store, 30, newPrice);
            Assert.True(bidRes.IsSuccess,bidRes.Error);
            
            string tokenguy = _auth.Connect();
            var resLoginGuy = await _auth.Login(tokenguy, this.Guy.Username, goodPassword, ServiceUserRole.Member);
            Assert.True(resLoginGuy.IsSuccess,resLoginGuy.Error);
            
            
            string tokenRaviv = _auth.Connect();
            var resLoginRaviv = await _auth.Login(tokenRaviv, this.Raviv.Username, goodPassword, ServiceUserRole.Member);
            Assert.True(resLoginRaviv.IsSuccess,resLoginRaviv.Error);
            
            
            string tokenRinat = _auth.Connect();
            var resLoginRinat = await _auth.Login(tokenRinat, this.Rinat.Username, goodPassword, ServiceUserRole.Member);
            Assert.True(resLoginRinat.IsSuccess,resLoginRinat.Error);
            
            
            //----for each owner- 01

            var resBidsInfos = _inStore.GetAllBidsWaitingToApprove(resLoginYossi.Value, store);
            Assert.True(resBidsInfos.IsSuccess,resBidsInfos.Error);
            Assert.True(resBidsInfos.Value.Count>0);

            var resApproveBid =
                _inStore.ApproveOrDisapproveBid(resLoginYossi.Value, store, resBidsInfos.Value[0].BidID, true);
            
            Assert.True(resApproveBid.IsSuccess);
            
            var cartRes=_cart.GetCart(loginGedalia.Value).Value;
            var basketRes=cartRes.Baskets.FirstOrDefault(x => x.StoreId.Equals(store));

            if (basketRes != null)
            {
                var itemInBasket = basketRes.Items.FirstOrDefault(x => x.ItemName.Equals(this.pomela.ItemName));
                Assert.Null(itemInBasket);
            }

            //----for each owner
            
            //----for each owner - 02

            resBidsInfos = _inStore.GetAllBidsWaitingToApprove(resLoginGuy.Value, store);
            Assert.True(resBidsInfos.IsSuccess,resBidsInfos.Error);
            Assert.True(resBidsInfos.Value.Count>0);

            resApproveBid =
                _inStore.ApproveOrDisapproveBid(resLoginGuy.Value, store, resBidsInfos.Value[0].BidID, true);
            
            Assert.True(resApproveBid.IsSuccess);
            
            cartRes=_cart.GetCart(loginGedalia.Value).Value;
            basketRes=cartRes.Baskets.FirstOrDefault(x => x.StoreId.Equals(store));

            if (basketRes != null)
            {
                var itemInBasket = basketRes.Items.FirstOrDefault(x => x.ItemName.Equals(this.pomela.ItemName));
                Assert.Null(itemInBasket);
            }

            //----for each owner
            
            //----for each owner - 03

            resBidsInfos = _inStore.GetAllBidsWaitingToApprove(resLoginRaviv.Value, store);
            Assert.True(resBidsInfos.IsSuccess,resBidsInfos.Error);
            Assert.True(resBidsInfos.Value.Count>0);

            resApproveBid =
                _inStore.ApproveOrDisapproveBid(resLoginRaviv.Value, store, resBidsInfos.Value[0].BidID, true);
            
            Assert.True(resApproveBid.IsSuccess);
            
            cartRes=_cart.GetCart(loginGedalia.Value).Value;
            basketRes=cartRes.Baskets.FirstOrDefault(x => x.StoreId.Equals(store));

            if (basketRes != null)
            {
                var itemInBasket = basketRes.Items.FirstOrDefault(x => x.ItemName.Equals(this.pomela.ItemName));
                Assert.Null(itemInBasket);
            }

            //----for each owner
            
            //----for each owner - 04

            resBidsInfos = _inStore.GetAllBidsWaitingToApprove(resLoginRinat.Value, store);
            Assert.True(resBidsInfos.IsSuccess,resBidsInfos.Error);
            Assert.True(resBidsInfos.Value.Count>0);

            resApproveBid =
                _inStore.ApproveOrDisapproveBid(resLoginRinat.Value, store, resBidsInfos.Value[0].BidID, true);
            
            Assert.True(resApproveBid.IsSuccess);
            
            cartRes=_cart.GetCart(loginGedalia.Value).Value;
            basketRes=cartRes.Baskets.FirstOrDefault(x => x.StoreId.Equals(store));
            

            cartRes=_cart.GetCart(loginGedalia.Value).Value;
            basketRes=cartRes.Baskets.FirstOrDefault(x => x.StoreId.Equals(store));
            Assert.NotNull(basketRes);
            
            var itemInBasketAfterAl = basketRes.Items.FirstOrDefault(x => x.ItemName.Equals(this.pomela.ItemName));
            Assert.NotNull(itemInBasketAfterAl);
            
            Assert.AreEqual(newPrice,itemInBasketAfterAl.PricePerUnit);
            
            token = _auth.Logout(loginGedalia.Value).Value;
            _auth.Disconnect(token);
            
            token = _auth.Logout(resLoginGuy.Value).Value;
            _auth.Disconnect(token);
            
            token = _auth.Logout(resLoginRaviv.Value).Value;
            _auth.Disconnect(token);
            
            token = _auth.Logout(resLoginRinat.Value).Value;
            _auth.Disconnect(token);
            
            tokenyossi = _auth.Logout(resLoginYossi.Value).Value;
            _auth.Disconnect(tokenyossi);
        }
        
        
    }
}