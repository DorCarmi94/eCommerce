using System;
using System.Collections.Generic;
using eCommerce.Auth;
using eCommerce.Business;
using eCommerce.Business.Repositories;
using eCommerce.Common;
using eCommerce.Statistics;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace eCommerce.Service
{
    public class UserService : IUserService
    {
        private IMarketFacade _marketFacade;
        
        internal UserService(IMarketFacade marketFacade)
        {
            _marketFacade = marketFacade;
        }
        
        public UserService()
        {
            _marketFacade = MarketFacade.GetInstance();
        }

        public static UserService CreateUserServiceForTests(IMarketFacade marketFacade)
        {
            return new UserService(marketFacade);
        }

        public Result<UserBasicInfo> GetUserBasicInfo(string token)
        {
            return _marketFacade.GetUserBasicInfo(token);
        }

        public Result<SPurchaseHistory> GetPurchaseHistory(string token)
        {
            Result<IList<PurchaseRecord>> purchaseHistoryRes = _marketFacade.GetPurchaseHistory(token);
            if (purchaseHistoryRes.IsFailure)
            {
                return Result.Fail<SPurchaseHistory>(purchaseHistoryRes.Error);
            }

            return Result.Ok(new SPurchaseHistory(purchaseHistoryRes.Value));
        }

        public Result AppointCoOwner(string token, string storeId, string appointedUserId)
        {
            return _marketFacade.AppointCoOwner(token, storeId, appointedUserId);
        }

        public Result RemoveCoOwner(string token, string storeId, string removedUserId)
        {
            return _marketFacade.RemoveCoOwner(token, storeId, removedUserId);
        }

        public Result AppointManager(string token, string storeId, string appointedManagerUserId)
        {
            return _marketFacade.AppointManager(token, storeId, appointedManagerUserId);
        }

        public Result RemoveManager(string token, string storeId, string removedUserId)
        {
            return _marketFacade.RemoveManager(token, storeId, removedUserId);
        }

        public Result UpdateManagerPermission(string token, string storeId, string managersUserId, IList<StorePermission> permissions)
        {
            return _marketFacade.UpdateManagerPermission(token, storeId, managersUserId, permissions);
        }

        public Result<SPurchaseHistory> AdminGetPurchaseHistoryUser(string token, string ofUserId)
        {
            Result<IList<PurchaseRecord>> purchaseRecordRes = _marketFacade.AdminGetPurchaseHistoryUser(token, ofUserId);
            if (purchaseRecordRes.IsFailure)
            {
                return Result.Fail<SPurchaseHistory>(purchaseRecordRes.Error);
            }

            return Result.Ok(new SPurchaseHistory(purchaseRecordRes.Value));
        }

        public Result<SPurchaseHistory> AdminGetPurchaseHistoryStore(string token, string storeId)
        {
            Result<IList<PurchaseRecord>> purchaseRecordRes = _marketFacade.AdminGetPurchaseHistoryStore(token, storeId);
            if (purchaseRecordRes.IsFailure)
            {
                return Result.Fail<SPurchaseHistory>(purchaseRecordRes.Error);
            }

            return Result.Ok(new SPurchaseHistory(purchaseRecordRes.Value));
        }

        public Result<List<string>> GetAllStoreIds(string token)
        {
            return _marketFacade.GetStoreIds(token);
        }
        
        public Result<IList<string>> GetAllManagedStoreIds(string token)
        {
            return _marketFacade.GetAllManagedStores(token);
        }

        public Result<LoginDateStat> AdminGetLoginStats(string token, DateTime date)
        {
            return _marketFacade.AdminGetLoginStats(token, date);
        }
    }
}