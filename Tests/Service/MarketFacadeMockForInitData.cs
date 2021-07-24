using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Business;
using eCommerce.Business.Discounts;
using eCommerce.Common;
using eCommerce.Service;
using eCommerce.Statistics;

namespace Tests.Service
{
    public class MarketFacadeMockForInitData : IMarketFacade
    {
        public int RegisteredNumber;
        public int OpenedStores;
        
        public int LoginsNumber;
        public int LogoutsNumber;

        public MarketFacadeMockForInitData()
        {
            Clear();
        }

        public void Clear()
        {
            RegisteredNumber = 0;
            OpenedStores = 0;

            LoginsNumber = 0;
            LogoutsNumber = 0;
        }
        public string Connect()
        {
            return "1";
        }

        public void Disconnect(string token)
        {
        }

        public async Task<Result> Register(string token, MemberInfo memberInfo, string password)
        {
            RegisteredNumber++;
            return Result.Ok();
        }

        public async Task<Result<string>> Login(string guestToken, string username, string password, UserToSystemState role)
        {
            LoginsNumber++;
            return Result.Ok("1");
        }

        public Result<string> Logout(string token)
        {
            LogoutsNumber++;
            return Result.Ok("1");
        }

        public bool IsUserConnected(string token)
        {
            throw new NotImplementedException();
        }

        public Result<UserBasicInfo> GetUserBasicInfo(string token)
        {
            throw new NotImplementedException();
        }

        public Result<IList<PurchaseRecord>> GetPurchaseHistory(string token)
        {
            throw new NotImplementedException();
        }

        public Result AppointCoOwner(string token, string storeId, string appointedUserId)
        {
            throw new NotImplementedException();
        }

        public Result RemoveCoOwner(string token, string storeId, string removedUserId)
        {
            throw new NotImplementedException();
        }

        public Result AppointManager(string token, string storeId, string appointedManagerUserId)
        {
            throw new NotImplementedException();
        }

        public Result RemoveManager(string token, string storeId, string removedUserId)
        {
            throw new NotImplementedException();
        }

        public Result<IList<StorePermission>> GetStorePermission(string token, string storeId)
        {
            throw new NotImplementedException();
        }

        public Result UpdateManagerPermission(string token, string storeId, string managersUserId, IList<StorePermission> permissions)
        {
            throw new NotImplementedException();
        }

        public Result<IList<Tuple<string, IList<StorePermission>>>> GetStoreStaffAndTheirPermissions(string token, string storeId)
        {
            throw new NotImplementedException();
        }

        public Result<IList<PurchaseRecord>> AdminGetPurchaseHistoryUser(string token, string ofUserId)
        {
            throw new NotImplementedException();
        }

        public Result<IList<PurchaseRecord>> AdminGetPurchaseHistoryStore(string token, string storeID)
        {
            throw new NotImplementedException();
        }

        public Result<IEnumerable<IItem>> SearchForItem(string token, string query)
        {
            throw new NotImplementedException();
        }

        public Result<IEnumerable<IItem>> SearchForItemByPriceRange(string token, string query, double @from = 0, double to = Double.MaxValue)
        {
            throw new NotImplementedException();
        }

        public Result<IEnumerable<IItem>> SearchForItemByCategory(string token, string query, string category)
        {
            throw new NotImplementedException();
        }

        public Result<IEnumerable<string>> SearchForStore(string token, string query)
        {
            throw new NotImplementedException();
        }

        public Result<Store> GetStore(string token, string storeId)
        {
            throw new NotImplementedException();
        }

        public Result<IEnumerable<IItem>> GetAllStoreItems(string token, string storeId)
        {
            throw new NotImplementedException();
        }

        public Result<IItem> GetItem(string token, string storeId, string itemId)
        {
            throw new NotImplementedException();
        }

        public Result<List<string>> GetStoreIds(string token)
        {
            throw new NotImplementedException();
        }

        public Result<IList<string>> GetAllManagedStores(string token)
        {
            throw new NotImplementedException();
        }

        public Result AddItemToCart(string token, string itemId, string storeId, int amount)
        {
            throw new NotImplementedException();
        }

        public Result AskToBidOnItem(string token, string productId, string storeId, int amount, double newPrice)
        {
            throw new NotImplementedException();
        }

        public Result EditItemAmountOfCart(string token, string itemId, string storeId, int amount)
        {
            throw new NotImplementedException();
        }

        public Result<ICart> GetCart(string token)
        {
            throw new NotImplementedException();
        }

        public Result<double> GetPurchaseCartPrice(string token)
        {
            throw new NotImplementedException();
        }

        public Result PurchaseCart(string token, PaymentInfo paymentInfo)
        {
            throw new NotImplementedException();
        }

        public Result OpenStore(string token, string storeName)
        {
            if (storeName.Equals("TakenStoreName"))
            {
                return Result.Fail("Store name is taken");
            }
            OpenedStores++;
            return Result.Ok();
        }

        public Result AddNewItemToStore(string token, IItem item)
        {
            throw new NotImplementedException();
        }

        public Result RemoveItemFromStore(string token, string storeId, string productId)
        {
            throw new NotImplementedException();
        }

        public Result EditItemInStore(string token, IItem item)
        {
            throw new NotImplementedException();
        }

        public Result UpdateStock_AddItems(string token, IItem item)
        {
            throw new NotImplementedException();
        }

        public Result UpdateStock_SubtractItems(string token, IItem item)
        {
            throw new NotImplementedException();
        }

        public Result<IList<PurchaseRecord>> GetPurchaseHistoryOfStore(string token, string storeId)
        {
            throw new NotImplementedException();
        }

        public Result AddRuleToStorePolicy(string token, string storeId, RuleInfoNode ruleInfoNode)
        {
            throw new NotImplementedException();
        }

        public Result AddDiscountToStore(string token, string storeId, DiscountInfoNode discountInfoNode)
        {
            throw new NotImplementedException();
        }

        public Result<IList<RuleInfoNode>> GetStorePolicyRules(string token, string storeId)
        {
            throw new NotImplementedException();
        }

        public Result<IList<DiscountInfoNode>> GetStoreDiscounts(string token, string storeId)
        {
            throw new NotImplementedException();
        }

        public Result<List<BidInfo>> GetAllBidsWaitingToApprove(string token, string storeId)
        {
            throw new NotImplementedException();
        }

        public Result ApproveOrDisapproveBid(string token, string storeId, string BidID, bool shouldApprove)
        {
            throw new NotImplementedException();
        }

        public Result<LoginDateStat> AdminGetLoginStats(string token, DateTime date)
        {
            throw new NotImplementedException();
        }

        public Result RemoveOwnerFromStore(string token, string storeId, string appointedUserId)
        {
            throw new NotImplementedException();
        }
    }
}