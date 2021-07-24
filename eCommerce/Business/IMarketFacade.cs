using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Business.Discounts;

using eCommerce.Common;
using eCommerce.Service;
using eCommerce.Statistics;

namespace eCommerce.Business
{
    public interface IMarketFacade
    {
        #region UserManage
        
        // ========== Connection and Authorization ========== //
        
        /// <summary>
        /// Connect a new guest to the system
        /// </summary>
        /// <returns>New auth token</returns>
        public string Connect();
        
        /// <summary>
        /// Disconnect a user from the system
        /// </summary>
        public void Disconnect(string token);

        /// <summary>
        /// Register a new user to the system as a member.
        /// </summary>
        ///<Test>
        ///TestRegisterToSystemSuccess
        ///TestRegisterToSystemFailure
        ///</Test>
        /// <param name="token">The Authorization token</param>
        /// <param name="memberInfoDto">The user information</param>
        /// <param name="password">The user password</param>
        /// <returns>Successful Result if the user has been successfully registered</returns>
        public Task<Result> Register(string token, MemberInfo memberInfo, string password);
        
        /// <summary>
        /// Log in to the system
        /// </summary>
        /// <Test>
        ///TestLoginSuccess
        /// TestLoginFailure
        /// </Test>
        /// <param name="guestToken">The guest Authorization token</param>
        /// <param name="username">The user name</param>
        /// <param name="password">The user password</param>
        /// <param name="role">The user role</param>
        /// <returns>Authorization token</returns>
        public Task<Result<string>> Login(string guestToken ,string username, string password, UserToSystemState role);
        
        /// <summary>
        /// Logout a user form the system.
        /// </summary>
        /// <Test>
        ///TestLogoutSuccess
        ///TestLogoutFailure
        /// </Test>
        /// <param name="token">Authorization token</param>
        /// <returns>New guest Authorization token</returns>
        public Result<string> Logout(string token);
        
        bool IsUserConnected(string token);

        Result<UserBasicInfo> GetUserBasicInfo(string token);

        /// <summary>
        /// Get the purchase history of the user 
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>The purchase history</returns>
        public Result<IList<PurchaseRecord>> GetPurchaseHistory(string token);
        
        /// <summary>
        /// Appoint a user as a coOwner to the store
        /// </summary>
        /// <Test>
        /// TestAppointCoOwnerSuccess
        /// TestAppointCoOwnerFailureInvalid
        /// TestAppointCoOwnerFailureLogic
        /// </Test>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <param name="appointedUserId">The appointment user</param>
        /// <returns>Result of the appointment</returns>
        public Result AppointCoOwner(string token, string storeId, string appointedUserId);

        /// <summary>
        /// Remove owner from store
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <param name="removedUserId">The removed owner user</param>
        /// <returns>Result of the removal</returns>
        public Result RemoveCoOwner(string token, string storeId, string removedUserId);

        
        /// <summary>
        /// Appoint a user as a new manager to the sore 
        /// </summary>
        /// <Test>
        /// TestAppointManagerSuccess
        /// TestAppointManagerFailureInvalid
        /// TestAppointManagerFailureLogic
        /// </Test>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <param name="appointedManagerUserId">The appointment manager</param>
        /// <returns>Result of the appointment</returns>
        public Result AppointManager(string token, string storeId, string appointedManagerUserId);

        /// <summary>
        /// Remove manager from store
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <param name="removedUserId">The removed owner user</param>
        /// <returns>Result of the removal</returns>
        public Result RemoveManager(string token, string storeId, string removedUserId);
        
        /// <summary>
        /// Return all the store permission that the user have
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <returns>All the store permission that the user have</returns>
        public Result<IList<StorePermission>> GetStorePermission(string token, string storeId);

        /// <summary>
        /// Update the manager permission
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <param name="managersUserId">The user id of the manger</param>
        /// <param name="permissions">The updated permission</param>
        /// <returns>Result of the update</returns>
        public Result UpdateManagerPermission(string token, string storeId, string managersUserId, IList<StorePermission> permissions);
        
        /*/// <summary>
        /// Remove the manager permission
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <param name="managersUserId">The user id of the manger</param>
        /// <param name="permissions">The updated permission</param>
        /// <returns>Result of the remove</returns>
        public Result RemoveManagerPermission(string token, string storeId, string managersUserId,
            IList<StorePermission> permissions);*/


        /// <summary>
        /// Get all the staff of the store and their permissions
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The storeId</param>
        /// <returns>List of all the staff and their permissions</returns>
        public Result<IList<Tuple<string, IList<StorePermission>>>> GetStoreStaffAndTheirPermissions(string token,
            string storeId);

        /// <summary>
        /// Get the history purchase of a user
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <param name="ofUserId">The user id</param>
        /// <returns>The history purchase</returns>
        public Result<IList<PurchaseRecord>> AdminGetPurchaseHistoryUser(string token, string ofUserId);
        
        /// <summary>
        /// Get the history purchase of a store
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <returns>The history purchase</returns>
        public Result<IList<PurchaseRecord>> AdminGetPurchaseHistoryStore(string token, string storeID);
        
        
        #endregion
        
        #region ItemsAndStores

        /// <summary>
        /// Search for item
        /// </summary>
        /// <param name="query">The item query the search</param>
        /// <param name="token">Authorization token</param>
        /// <returns>List of match products</returns>
        public Result<IEnumerable<IItem>> SearchForItem(string token, string query);
        
        /// <summary>
        /// Search for item by price range
        /// </summary>
        /// <param name="query">The item query the search</param>
        /// <param name="token">Authorization token</param>
        /// <param name="from">From price</param>
        /// <param name="to">To price</param>
        /// <returns>List of match products</returns>
        public Result<IEnumerable<IItem>> SearchForItemByPriceRange(string token, string query, double from = 0.0, double to = Double.MaxValue);
        
        /// <summary>
        /// Search for item by category
        /// </summary>
        /// <param name="query">The item query the search</param>
        /// <param name="token">Authorization token</param>
        /// <param name="category">Search category</param>
        /// <returns>List of match products</returns>
        public Result<IEnumerable<IItem>> SearchForItemByCategory(string token, string query, string category);

        /// <summary>
        /// Search for store
        /// </summary>
        /// <param name="query">The store query the search</param>
        /// <param name="token">Authorization token</param>
        /// <returns>List of match stores</returns>
        public Result<IEnumerable<string>> SearchForStore(string token, string query);
        
        /// <summary>
        /// Get all the store information
        /// </summary>
        /// <param name="token">The Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <returns>The store information</returns>
        public Result<Store> GetStore(string token, string storeId);
        
        /// <summary>
        /// Get all the store items
        /// </summary>
        /// <param name="token">The Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <returns>The store items</returns>
        public Result<IEnumerable<IItem>> GetAllStoreItems(string token, string storeId);
        
        /// <summary>
        /// Get the info of an item
        /// </summary>
        /// <param name="token">The Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <param name="itemId">The item id</param>
        /// <returns>The item information</returns>
        public Result<IItem> GetItem(string token, string storeId, string itemId);
        
        /// <summary>
        /// Get all the store ids of the user owns
        /// </summary>
        /// <param name="token">The Authorization token</param>
        /// <returns>All the owned store ids</returns>
        public Result<List<string>> GetStoreIds(string token);

        /// <summary>
        /// Return all stores id that the user manage
        /// </summary>
        /// <param name="token">The Authorization token</param>
        /// <returns>All stores id that the user manage</returns>
        public Result<IList<string>> GetAllManagedStores(string token);
        
        #endregion
        
        // ========== Store ========== //
        
        #region UserBuyingFromStores
        
        /// <summary>
        /// Adding Item to user cart
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="itemId">The item id</param>
        /// <param name="storeId">The store id</param>
        /// <param name="amount">The amount of the item</param>
        /// <returns>Result of the request</returns>
        public Result AddItemToCart(string token, string itemId, string storeId, int amount);
        /// <summary>
        /// Ask To Bid On Item
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="productId">The item id</param>
        /// <param name="storeId">The store id</param>
        /// <param name="amount">The amount of the item</param>
        /// <param name="newPrice">New price to offer</param>
        /// <returns>Result of the request</returns>
        public Result AskToBidOnItem(string token, string productId, string storeId, int amount, double newPrice);
        
        /// <summary>
        /// Change the amount of the item in the cart
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="itemId">The item id</param>
        /// <param name="storeId">The store id</param>
        /// <param name="amount">The amount of the item</param>
        /// <returns>Result of the request</returns>
        public Result EditItemAmountOfCart(string token, string itemId, string storeId, int amount);

        /// <summary>
        /// Get the cart of the user
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>The user cart</returns>
        public Result<ICart> GetCart(string token);

        /// <summary>
        /// Return the total price of the cart(after discounts)
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>The total price of the cart</returns>
        public Result<double> GetPurchaseCartPrice(string token);
        
        /// <summary>
        /// Purchase the user cart
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>The result the purchase</returns>
        public Result PurchaseCart(string token, PaymentInfo paymentInfo);
        
        #endregion

        #region StoreManage
        
        /// <summary>
        /// Open a new store for the user.
        /// The name need to be unique
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeName">The store name</param>
        /// <param name="item">The start product of a sotre</param>
        /// <returns>Result of the request</returns>
        public Result OpenStore(string token, string storeName);
        
        /// <summary>
        /// Add new item to the sore
        /// </summary>
        /// <Test>
        /// TestAddNewItemToStoreSuccess
        /// TestAddNewItemToStoreFailureInput
        /// TestAddNewItemToStoreFailureAccess
        /// </Test>
        /// <param name="token">Authorization token</param>
        /// <param name="item">The new item</param>
        /// <returns>Result of the item addition</returns>
        public Result AddNewItemToStore(string token,  IItem item);
        
        /// <summary>
        /// Remove item from store
        /// </summary>
        /// <Test>
        /// TestRemoveProductFromStoreSuccess
        /// TestRemoveProductFromStoreFailureInvalid
        /// TestRemoveProductFromStoreFailureLogic
        /// </Test>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The sore id</param>
        /// <param name="productId">The product id</param>
        /// <returns>Result of the product removal</returns>
        public Result RemoveItemFromStore(string token, string storeId, string productId);
        
        /// <summary>
        /// Edit the item
        /// </summary>
        /// <Test>
        /// TestEditItemInStoreSuccess
        /// TestEditItemInStoreFailureInvalid
        /// TestEditItemInStoreFailureLogic
        /// </Test>
        /// <param name="token">Authorization token</param>
        /// <param name="item">The new item</param>
        /// <returns>Result of the edit</returns>
        public Result EditItemInStore(string token, IItem item);
        
        /// <summary>
        /// Add items amount
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="item">The item</param>
        /// <returns>Result of updating the amount (adding)</returns>
        public Result UpdateStock_AddItems(string token, IItem item);
        
        /// <summary>
        /// Add items amount
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="item">The item</param>
        /// <returns>Result of updating the amount (subtract)</returns>
        public Result UpdateStock_SubtractItems(string token, IItem item);

        /// <summary>
        /// Return all the purchase history of a store
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The storeId</param>
        /// <returns>List of the purchase history in a store</returns>
        public Result<IList<PurchaseRecord>> GetPurchaseHistoryOfStore(string token, string storeId);
        
        /// <summary>
        /// Adding a rule to the store to modify what can or cannot happen.
        /// The rule can be simple or a combination of number of rules combined with logical combination functions (and,or..)
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The storeId</param>
        /// <param name="ruleInfoNode">The rule: Leaf (simple) or Composite (Combined)</param>
        public Result AddRuleToStorePolicy(string token, string storeId, RuleInfoNode ruleInfoNode);
        
        
        /// <summary>
        /// Adding a discount to the store
        /// The discount can be simple or a combination of number of discounts combined with logical or math combination functions (and,or, max,min..)
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The storeId</param>
        /// <param name="discountInfoNode">The discount: Leaf (simple) or Composite (Combined)</param>
        public Result AddDiscountToStore(string token, string storeId, DiscountInfoNode discountInfoNode);
        
        /// <summary>
        /// Returns all the rules in store's policy
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The storeId</param>
        /// <returns>List of the all rules in store</returns>
        public Result<IList<RuleInfoNode>> GetStorePolicyRules(string token, string storeId);
        
        /// <summary>
        /// Return all the discounts in store
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The storeId</param>
        /// <returns>List of all discounts in store</returns>
        public Result<IList<DiscountInfoNode>> GetStoreDiscounts(string token, string storeId);

        /// <summary>
        /// Return all the bids waiting for owner to approve
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The storeId</param>
        /// <returns>List of all bids infos of bids waiting for the user (owner) to approve or not</returns>
        public Result<List<BidInfo>> GetAllBidsWaitingToApprove(string token, string storeId);
        
        /// <summary>
        /// Approves or disapproves the bid
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The storeId</param>
        /// <param name="BidID">The bid id to approve or not</param>
        /// <param name="shouldApprove">true to approve the bid and false to disapprove</param>
        /// <returns>Result for if the process succeeded or not</returns>
        public Result ApproveOrDisapproveBid(string token, string storeId, string BidID, bool shouldApprove);
        
        #endregion

        /// <summary>
        /// Get the login stats
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="date">Date of the stats</param>
        /// <returns>The stats</returns>
        Result<LoginDateStat> AdminGetLoginStats(string token, DateTime date);
    }
}