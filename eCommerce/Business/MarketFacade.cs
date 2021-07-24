using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.Auth;
using eCommerce.Business.Discounts;
using eCommerce.Business.Repositories;

using eCommerce.Common;
using eCommerce.DataLayer;
using eCommerce.Service;
using eCommerce.Statistics;
using NLog;

namespace eCommerce.Business
{
    // TODO should be singleton
    // TODO check authException if we should throw them
    public class MarketFacade : IMarketFacade
    {
        private static MarketFacade _instance = new MarketFacade();

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private AbstractStoreRepo _storeRepo;
        private UserManager _userManager;

        private MarketFacade()
        {
            
        }
        
        private MarketFacade(IUserAuth userAuth,
            IRepository<User> registeredUsersRepo,
            AbstractStoreRepo storeRepo)
        {
            Init(userAuth, registeredUsersRepo, storeRepo);
        }

        public static MarketFacade GetInstance()
        {
            return _instance;
        }

        public static MarketFacade CreateInstanceForTests(IUserAuth userAuth,
            IRepository<User> registeredUsersRepo,
            AbstractStoreRepo storeRepo)
        {
            return new MarketFacade(userAuth, registeredUsersRepo, storeRepo);
        }

        /// <summary>
        /// Initialize the market, must be called before using this class
        /// </summary>
        public void Init(IUserAuth userAuth,
            IRepository<User> registeredUsersRepo,
            AbstractStoreRepo storeRepo)
        {
            _storeRepo = storeRepo;
            _userManager = new UserManager(userAuth, registeredUsersRepo);
            CreateMainAdmin();
        }

        public void CreateMainAdmin()
        {
            _userManager.CreateMainAdmin();
        }


        #region UserManage
        // <CNAME>Connect</CNAME>
        public string Connect()
        {
            return _userManager.Connect();
        }

        // <CNAME>Disconnect</CNAME>
        public void Disconnect(string token)
        {
            _userManager.Disconnect(token);
        }

        // <CNAME>Register</CNAME>
        public Task<Result> Register(string token, MemberInfo memberInfo, string password)
        {
            return _userManager.Register(token, memberInfo, password);
        }

        // <CNAME>Login</CNAME>
        public Task<Result<string>> Login(string guestToken, string username, string password, UserToSystemState role)
        {
            return _userManager.Login(guestToken, username, password, role);
        }

        // <CNAME>Logout</CNAME>
        public Result<string> Logout(string token)
        {
            return _userManager.Logout(token);
        }

        public bool IsUserConnected(string token)
        {
            return _userManager.IsUserConnected(token);
        }

        public Result<UserBasicInfo> GetUserBasicInfo(string token)
        {
            Result<User> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<UserBasicInfo>(userRes.Error);
            }
            User user = userRes.Value;

            UserBasicInfo userBasicInfo = new UserBasicInfo(user.Username, true,
                UserSystemStateToUserRole(user.GetState()));
            if (userBasicInfo.UserRole == UserRole.Guest)
            {
                userBasicInfo.IsLoggedIn = false;
            }

            return Result.Ok(userBasicInfo);
        }

        //<CNAME>PersonalPurchaseHistory</CNAME>
        public Result<IList<PurchaseRecord>> GetPurchaseHistory(string token)
        {
            Result<User> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<IList<PurchaseRecord>>(userRes.Error);
            }
            User user = userRes.Value;

            var result = user.GetUserPurchaseHistory();

            if (result.IsFailure)
            {
                _logger.Error($"Error for user {user.Username} in getting the Purchase history");
                return Result.Fail<IList<PurchaseRecord>>(result.Error);
            }

            _logger.Info($"User {user.Username} request purchase history");
            return result;
        }
        
         //<CNAME>AppointCoOwner</CNAME>
        public Result AppointCoOwner(string token, string storeId, string appointedUserId)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;
            
            _logger.Info($"AppointCoOwner({user.Username}, {store.GetStoreName()}, {appointedUserId})");
            
            Result<User> appointedUserRes = _userManager.GetUser(appointedUserId);
            if (appointedUserRes.IsFailure)
            {
                return appointedUserRes;
            }
            User appointedUser = appointedUserRes.Value;

            Result appointmentRes = user.AppointUserToOwner(store, appointedUser);
            if (appointmentRes.IsSuccess)
            {
                _userManager.UpdateUser(user);
                _userManager.UpdateUser(appointedUser);
                _storeRepo.Update(store);
            }

            return appointmentRes;
        }

        //<CNAME>RemoveCoOwner</CNAME>
        public Result RemoveCoOwner(string token, string storeId, string removedUserId)
        {
            return this.RemoveOwnerFromStore(token, storeId, removedUserId);
        }

        //<CNAME>AppointManager</CNAME>
        public Result AppointManager(string token, string storeId, string appointedManagerUserId)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;
            
            _logger.Info($"AppointManager({user.Username}, {store.GetStoreName()}, {appointedManagerUserId})");
            
            Result<User> appointedUserRes = _userManager.GetUser(appointedManagerUserId);
            if (appointedUserRes.IsFailure)
            {
                return appointedUserRes;
            }
            User appointedUser = appointedUserRes.Value;
            
            Result appointmentRes = user.AppointUserToManager(store, appointedUser);
            if (appointmentRes.IsSuccess)
            {
                _userManager.UpdateUser(user);
                _userManager.UpdateUser(appointedUser);
                _storeRepo.Update(store);
            }
            

            return appointmentRes;
        }

        public Result RemoveManager(string token, string storeId, string removedUserId)
        {
            throw new NotImplementedException();
        }

        public Result<IList<StorePermission>> GetStorePermission(string token, string storeId)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return  Result.Fail<IList<StorePermission>>(userAndStoreRes.Error);
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;

            return store.GetPermissions(user);
        }

        //<CNAME>UpdateManagerPermission</CNAME>
        public Result UpdateManagerPermission(string token, string storeId, string managersUserId, IList<StorePermission> permissions)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;
            
            _logger.Info($"UpdateManagerPermission({user.Username}, {store.GetStoreName()}, {managersUserId}, {permissions})");

            Result<User> mangerUserRes = _userManager.GetUser(managersUserId);
            if (mangerUserRes.IsFailure)
            {
                return mangerUserRes;
            }
            User managerUser = mangerUserRes.Value;
            Result updateRes = user.UpdatePermissionsToManager(store, managerUser, permissions);
            if (updateRes.IsSuccess)
            {
                _storeRepo.UpdateManager(managerUser.StoresManaged.KeyToValue(store.StoreName));
                _storeRepo.Update(store);
                _userManager.UpdateUser(managerUser);
            }

            return updateRes;
        }
        
        //<CNAME>GetStoreStaff</CNAME>
        public Result<IList<Tuple<string, IList<StorePermission>>>> GetStoreStaffAndTheirPermissions(string token,
            string storeId)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<IList<Tuple<string, IList<StorePermission>>>>(userAndStoreRes.Error);
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;

            _logger.Info($"GetStoreStaffAndTheirPermissions({user.Username}, {store.GetStoreName()})");

            return store.GetStoreStaffAndTheirPermissions(user);
        }
        //<CNAME>AdminGetAllUserHistory</CNAME>
        public Result<IList<PurchaseRecord>> AdminGetPurchaseHistoryUser(string token, string ofUserId)
        {
            var userAndStoreRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
             if (userAndStoreRes.IsFailure)
             {
                 return Result.Fail<IList<PurchaseRecord>>(userAndStoreRes.Error);
             }
             
             User user = userAndStoreRes.Value;
             
             _logger.Info($"AdminGetPurchaseHistoryUser({user.Username}, {ofUserId})");

             var ofUser=_userManager.GetUser(ofUserId);
             if (ofUser.IsFailure)
             {
                 return Result.Fail<IList<PurchaseRecord>>(ofUser.Error);
             }

             return user.GetUserPurchaseHistory(ofUser.Value);
        }
         
         //<CNAME>AdminGetStoreHistory</CNAME>
         public Result<IList<PurchaseRecord>> AdminGetPurchaseHistoryStore(string token, string storeId)
         {
             Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
             if (userAndStoreRes.IsFailure)
             {
                 return Result.Fail<IList<PurchaseRecord>>(userAndStoreRes.Error);
             }
             User user = userAndStoreRes.Value.Item1;
             Store store = userAndStoreRes.Value.Item2;

             _logger.Info($"AdminGetPurchaseHistoryStore({user.Username}, {store.GetStoreName()})");

             return user.GetStorePurchaseHistory(store);
         }
        #endregion

        #region ItemsAndStores
        //<CNAME>SearchForProducts</CNAME>
        public Result<IEnumerable<IItem>> SearchForItem(string token, string query)
        {
            Result<User> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<IEnumerable<IItem>>(userRes.Error);
            }

            _logger.Info($"SearchForItem({userRes.Value.Username}, {query})");
            
            return Result.Ok<IEnumerable<IItem>>(_storeRepo.SearchForItem(query));
        }

        public Result<IEnumerable<IItem>> SearchForItemByPriceRange(string token, string query, double @from = 0, double to = Double.MaxValue)
        {
            Result<User> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<IEnumerable<IItem>>(userRes.Error);
            }

            if (to < from)
            {
                to = from;
            }
            
            _logger.Info($"SearchForItemByPriceRange({userRes.Value.Username}, {query}, {from}, {to})");

            return Result.Ok<IEnumerable<IItem>>(_storeRepo.SearchForItemByPrice(query, from, to));
        }

        public Result<IEnumerable<IItem>> SearchForItemByCategory(string token, string query, string category)
        {
            Result<User> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<IEnumerable<IItem>>(userRes.Error);
            }

            _logger.Info($"SearchForItemByCategory({userRes.Value.Username}, {query}, {category})");

            return Result.Ok<IEnumerable<IItem>>(_storeRepo.SearchForItemByCategory(query, category));
        }

        //<CNAME>SearchForStore</CNAME>
        public Result<IEnumerable<string>> SearchForStore(string token, string query)
        {
            Result<User> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<IEnumerable<string>>(userRes.Error);
            }
            
            _logger.Info($"SearchForStore({userRes.Value.Username}, {query})");

            return Result.Ok(_storeRepo.SearchForStore(query));
        }
        
        public Result<Store> GetStore(string token, string storeId)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<Store>(userAndStoreRes.Error);
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;

            _logger.Info($"GetStore({user.Username}, {storeId})");

            return Result.Ok(store);
        } 
        public Result<IEnumerable<IItem>> GetAllStoreItems(string token, string storeId)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<IEnumerable<IItem>>(userAndStoreRes.Error);
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;

            IList<IItem> storeItems = new List<IItem>();
            foreach (var item in store.GetAllItems())
            {
                storeItems.Add(item.ShowItem());
            }
            
            _logger.Info($"GetAllStoreItems({user.Username}, {storeId})");

            return Result.Ok<IEnumerable<IItem>>(storeItems);
            
        }
        
        public Result<IItem> GetItem(string token, string storeId, string itemId)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<IItem>(userAndStoreRes.Error);
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;

            Result<Item> itemRes = store.GetItem(itemId);
            if (itemRes.IsFailure)
            {
                return Result.Fail<IItem>(itemRes.Error);
            }
            
            _logger.Info($"GetItemInfoAfterBidApprove({user.Username}, {storeId}, {itemId})");

            return Result.Ok<IItem>(itemRes.Value.ShowItem());
        }

        public Result<List<string>> GetStoreIds(string token)
        {
            Result<User> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<List<string>>(userRes.Error);
            }
            User user = userRes.Value;

            return user.GetStoreIds();
        }

        public Result<IList<string>> GetAllManagedStores(string token)
        {
            Result<User> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<IList<string>>(userRes.Error);
            }
            User user = userRes.Value;

            return user.GetManagedStoreIds();
        }

        #endregion

        #region UserBuyingFromStores
        
        //<CNAME>AddItemToCart</CNAME>
        public Result AddItemToCart(string token, string productId, string storeId, int amount)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;

            _logger.Info($"AddItemToCart({user.Username}, {productId}, {storeId}, {amount})");

            Result<Item> itemRes = store.GetItem(productId);
            if (itemRes.IsFailure)
            {
                return itemRes;
            }
            var newItemInfo = itemRes.Value.ShowItem();
            newItemInfo.amount = amount;

            Result addRes = user.AddItemToCart(newItemInfo);
            if (addRes.IsSuccess)
            {
                //TODO check if need to update store
                _userManager.UpdateUser(user);
                _storeRepo.Update(store);
            }

            return addRes;
        }

        //<CNAME>AddBidToItem</CNAME>
        public Result AskToBidOnItem(string token, string productId, string storeId, int amount, double newPrice)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;

            _logger.Info($"AddItemToCart({user.Username}, {productId}, {storeId}, {amount})");

            Result<Item> itemRes = store.GetItem(productId);
            if (itemRes.IsFailure)
            {
                return itemRes;
            }
            var newItemInfo = itemRes.Value.ShowItem();
            //newItemInfo.AssignStoreToItem(store);
            return store.AskToBidOnItem(user, newItemInfo, newPrice, amount);
        }

        //<CNAME>EditCart</CNAME>  
        public Result EditItemAmountOfCart(string token, string itemId, string storeId, int amount)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;
            
            _logger.Info($"EditItemAmountOfCart({user.Username}, {itemId}, {storeId}, {amount})");

            Result<Item> itemRes = store.GetItem(itemId);
            if (itemRes.IsFailure)
            {
                return itemRes;
            }
            var editedItemInfo = itemRes.Value.ShowItem();
            editedItemInfo.amount = amount;
            Result cartRes = user.EditCart(editedItemInfo);

            if (cartRes.IsSuccess)
            {
                _userManager.UpdateUser(user);
            }
            
            return cartRes;
        }
        
        //<CNAME>GetCart</CNAME>
        public Result<ICart> GetCart(string token)
        {
            Result<User> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<ICart>(userRes.Error);
            }
            User user = userRes.Value;

            _logger.Info($"GetCart({user.Username})");
            var resCart=user.GetCartInfo();
            if (resCart.IsFailure)
            {
                return Result.Fail<ICart>(resCart.Error);
            }
            return new Result<ICart>(resCart.Value, resCart.IsSuccess, resCart.Error);
        }
        
        //<CNAME>GetCartPrice</CNAME>
        public Result<double> GetPurchaseCartPrice(string token)
        {
            Result<User> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<double>(userRes.Error);
            }
            User user = userRes.Value;

            _logger.Info($"GetPurchaseCartPrice({user.Username})");

            Result<Cart> cartRes = user.GetCartInfo();
            if (cartRes.IsFailure)
            {
                return Result.Fail<double>(cartRes.Error);
            }

            ICart cart = cartRes.Value;
            return cart.CalculatePricesForCart();
        }

        //<CNAME>BuyWholeCart</CNAME>
        public Result PurchaseCart(string token, PaymentInfo paymentInfo)
        {
            Result<User> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<double>(userRes.Error);
            }
            User user = userRes.Value;

            _logger.Info($"PurchaseCart({user.Username} {paymentInfo})");
            
            Result<Cart> cartRes = user.GetCartInfo();
            if (cartRes.IsFailure)
            {
                return Result.Fail<double>(cartRes.Error);
            }

            ICart cart = cartRes.Value;

            var storesToUpdate = cart.GetBaskets().Select(basket => basket._store );
            
            Result purchaseRes = cart.BuyWholeCart(user,paymentInfo);
            if (purchaseRes.IsFailure)
            {
                return purchaseRes;
            }
            _userManager.UpdateUser(user);
            
            foreach (var store in storesToUpdate)
            {
                _storeRepo.Update(store);
            }
            
            return Result.Ok();
        }
        
        #endregion

        #region StoreManage
        //<CNAME>OpenStore</CNAME>
        public Result OpenStore(string token, string storeName)
        {
            // TODO check with user and store
            Result<User> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail(userRes.Error);
            }
            User user = userRes.Value;
            
            _logger.Info($"OpenStore({user.Username})");
            
            Store newStore = new Store(storeName, user);
            if (!_storeRepo.Add(newStore))
            {
                return Result.Fail("Store name taken");
            }

            if (user.OpenStore(newStore).IsFailure)
            {
                return Result.Fail("Error opening store");
            }

            // TODO check save order
            
            _userManager.UpdateUser(user);
            _storeRepo.Update(newStore);
            return Result.Ok();
        }
        
        //<CNAME>ItemsToStore</CNAME>
        public Result AddNewItemToStore(string token, IItem item)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, item.StoreName);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;

            _logger.Info($"AddNewItemToStore({user.Username} ,{item})");

            Result addItemRes = store.AddItemToStore(DtoUtils.ItemDtoToProductInfo(item), user);
            if (addItemRes.IsSuccess)
            {
                _storeRepo.Update(store);
            }

            return addItemRes;
        }
        
        //<CNAME>ItemsInStore</CNAME>
        public Result RemoveItemFromStore(string token, string storeId, string itemId)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;

            _logger.Info($"RemoveItemFromStore({user.Username} ,{storeId}, {itemId})");

            Result removeItemRes =  store.RemoveItemToStore(itemId, user);
            if (removeItemRes.IsSuccess)
            {
                _storeRepo.Update(store);
            }

            return removeItemRes;
        }

        //<CNAME>EditItemInStore</CNAME>
        public Result EditItemInStore(string token, IItem item)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, item.StoreName);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;
            
            _logger.Info($"EditItemInStore({user.Username} , {item})");

            //public ItemInfo(int amount, string name, string storeName, string category,int pricePerUnit, List<string> keyWords,Item theItem)
            store.EditItemToStore(
                new ItemInfo(item.Amount, item.ItemName, item.StoreName, item.Category, item.KeyWords.ToList(),
                    (int) item.PricePerUnit), user);
            
            Result editItemRes = store.EditItemToStore(DtoUtils.ItemDtoToProductInfo(item), user);
            if (editItemRes.IsSuccess)
            {
                _storeRepo.Update(store);
            }

            return editItemRes;
        }

        //<CNAME>UpdateStockAdd</CNAME>
        public Result UpdateStock_AddItems(string token, IItem item)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, item.StoreName);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;

            return store.UpdateStock_AddItems(DtoUtils.ItemDtoToProductInfo(item), user);
        }
        
        //<CNAME>UpdateStockSub</CNAME>
        public Result UpdateStock_SubtractItems(string token, IItem item)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, item.StoreName);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;

            return store.UpdateStock_SubtractItems(DtoUtils.ItemDtoToProductInfo(item), user);
        }
        
        //<CNAME>GetStoreHistory</CNAME>
        public Result<IList<PurchaseRecord>> GetPurchaseHistoryOfStore(string token, string storeId)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<IList<PurchaseRecord>>(userAndStoreRes.Error);
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;

            _logger.Info($"GetPurchaseHistoryOfStore({user.Username} , {storeId})");

            Result<IList<PurchaseRecord>> purchaseHistoryRes = store.GetPurchaseHistory(user);
            if (purchaseHistoryRes.IsFailure)
            {
                return Result.Fail<IList<PurchaseRecord>>(purchaseHistoryRes.Error);
            }

            return Result.Ok<IList<PurchaseRecord>>((IList<PurchaseRecord>) purchaseHistoryRes.Value);
        }

        //<CNAME>AddRoleToStorePolicy</CNAME>
        public Result AddRuleToStorePolicy(string token, string storeId, RuleInfoNode ruleInfoNode)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<IList<PurchaseRecord>>(userAndStoreRes.Error);
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;

            _logger.Info($"AddRuleToStorePolicy({user.Username} , {storeId})");

            var res=store.AddRuleToStorePolicy(user,ruleInfoNode);
            if (res.IsFailure)
            {
                return Result.Fail<IList<PurchaseRecord>>(res.Error);
            }

            return Result.Ok();
        }

        //<CNAME>AddDiscountToStore</CNAME>
        public Result AddDiscountToStore(string token, string storeId, DiscountInfoNode discountInfoNode)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<IList<PurchaseRecord>>(userAndStoreRes.Error);
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;

            _logger.Info($"AddDiscountToStore({user.Username} , {storeId})");

            var res=store.AddDiscountToStore(user,discountInfoNode);
            if (res.IsFailure)
            {
                return Result.Fail(res.Error);
            }

            return Result.Ok();
        }

        //<CNAME>GetStorePolicy</CNAME>
        public Result<IList<RuleInfoNode>> GetStorePolicyRules(string token, string storeId)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<IList<RuleInfoNode>>(userAndStoreRes.Error);
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;

            _logger.Info($"GetStorePolicyRules({user.Username} , {storeId})");

            var res=store.GetStorePolicy(user);
            if (res.IsFailure)
            {
                return Result.Fail<IList<RuleInfoNode>>(res.Error);
            }

            return res;
        }

        //<CNAME>GetStoreDiscounts</CNAME>
        public Result<IList<DiscountInfoNode>> GetStoreDiscounts(string token, string storeId)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<IList<DiscountInfoNode>>(userAndStoreRes.Error);
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;

            _logger.Info($"GetStorePolicyRules({user.Username} , {storeId})");

            var res=store.GetStoreDiscounts(user);
            if (res.IsFailure)
            {
                return Result.Fail<IList<DiscountInfoNode>>(res.Error);
            }

            return res;
        }

        public Result<LoginDateStat> AdminGetLoginStats(string token, DateTime date)
        {
            Result<User> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<LoginDateStat>(userRes.Error);
            }
            User user = userRes.Value;

            return user.GetLoginStats(date);
        }

        public Result RemoveOwnerFromStore(string token, string storeId, string appointedUserId)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;
            
            _logger.Info($"AppointCoOwner({user.Username}, {store.GetStoreName()}, {appointedUserId})");
            
            Result<User> appointedUserRes = _userManager.GetUser(appointedUserId);
            if (appointedUserRes.IsFailure)
            {
                return appointedUserRes;
            }
            User appointedUser = appointedUserRes.Value;

            Result removalRes = user.RemoveOwnerFromStore(store, appointedUser);
            if (removalRes.IsSuccess)
            {
                _userManager.UpdateUser(user);
                _userManager.UpdateUser(appointedUser);
                _storeRepo.Update(store);
            }

            return removalRes;
        }

        public Result<List<BidInfo>> GetAllBidsWaitingToApprove(string token, string storeId)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<List<BidInfo>>(userAndStoreRes.Error);
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;
            
            _logger.Info($"Get all bids waiting to approve ({user.Username}, {store.GetStoreName()})");
            return store.GetAllMyWaitingBids(user);
        }
        
        public Result ApproveOrDisapproveBid(string token, string storeId, string BidID,bool shouldApprove)
        {
            Result<Tuple<User, Store>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<List<BidInfo>>(userAndStoreRes.Error);
            }
            User user = userAndStoreRes.Value.Item1;
            Store store = userAndStoreRes.Value.Item2;
            
            _logger.Info($"Approve={shouldApprove} for bid {BidID} of owner and store({user.Username}, {store.GetStoreName()})");
            return store.ApproveOrDissaproveBid(user, BidID, shouldApprove);
        }

        #endregion

        private Result<Tuple<User, Store>> GetUserAndStore(string token, string storeId)
        {
            Result<User> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<Tuple<User, Store>>(userRes.Error);
            }
            User user = userRes.Value;
            
            _logger.Info($"GetUserAndStore({user.Username} , {storeId})");

            Store store = _storeRepo.GetOrNull(storeId);
            if (store == null)
            {
                return Result.Fail<Tuple<User, Store>>("Store doesn't exist");
            }

            return Result.Ok(new Tuple<User, Store>(user, store));
        }
        
        private UserRole UserSystemStateToUserRole(UserToSystemState userToSystemState)
        {
            if (userToSystemState.Equals(Guest.State))
            {
                return UserRole.Guest;
            } 
            
            if (userToSystemState.Equals(Member.State))
            {
                return UserRole.Member;
            }
            
            return UserRole.Admin;
            
        }
    }
}