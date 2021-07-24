using System;
using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Common;
using eCommerce.Service;
using eCommerce.Service.StorePolicies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eCommerce.Controllers
{

    public class JSONStorePermissions
    {
        public List<StorePermission> StorePermissions { get; set; }
    }
    
    public class StoreAndItemId
    {
        public string StoreId { get; set; }
        public string ItemId { get; set; }
    }
    
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        private readonly ILogger<StoreController> _logger;
        private readonly INStoreService _inStoreService;
        private readonly UserService _userService;

        public StoreController(ILogger<StoreController> logger)
        {
            _logger = logger;
            _inStoreService = new InStoreService();
            _userService = new UserService();
        }


        // the route is /Store/OpenStore  
        //removes "Controller" from the class name and add the name of the function as an endpoint 

        [HttpPost("[action]")]
        public Result OpenStore(string storeId)
        {
            return _inStoreService.OpenStore((string) HttpContext.Items["authToken"],
                storeId);
        }

        [HttpPost("[action]")]
        public Result AddItem([FromBody] SItem item)
        {
            return _inStoreService.AddNewItemToStore((string) HttpContext.Items["authToken"],
                item);
        }
        
        [HttpPost("[action]")]
        public Result RemoveItem([FromBody] StoreAndItemId data)
        {
            return _inStoreService.RemoveItemFromStore((string) HttpContext.Items["authToken"],
                data.StoreId, data.ItemId);
        }

        [HttpPost("[action]")]
        public Result EditItem([FromBody] SItem item)
        {
            return _inStoreService.EditItemInStore((string) HttpContext.Items["authToken"],
                item);
        }
        
        [HttpGet("{storeId}/{itemId}")]
        public Result<IItem> GetItem(string storeId, string itemId)
        {
            return _inStoreService.GetItem((string) HttpContext.Items["authToken"],
                storeId, itemId);
        }
        
        [HttpGet("[action]/{storeId}")]
        public Result<IEnumerable<IItem>> GetAllItems(string storeId)
        {
            return _inStoreService.GetAllStoreItems((string) HttpContext.Items["authToken"],
                    storeId);
        }
        
        [HttpGet("[action]")]
        public Result<IEnumerable<IItem>> Search(string query)
        {
            return _inStoreService.SearchForItem((string) HttpContext.Items["authToken"],
                query);
        }
        
        [HttpGet("[action]")]
        public Result<IEnumerable<string>> SearchStore(string query)
        {
            return _inStoreService.SearchForStore((string) HttpContext.Items["authToken"],
                query);
        }
        
        [HttpGet("{storeId}/history")]
        public Result<SPurchaseHistory> GetStoreHistory(string storeId)
        {
            return _inStoreService.GetPurchaseHistoryOfStore((string) HttpContext.Items["authToken"],
                storeId);
        }
        
        [HttpGet("{storeId}/storeHistory")]
        public Result<SPurchaseHistory> AdminGetStoreHistory(string storeId)
        {
            return _userService.AdminGetPurchaseHistoryStore((string) HttpContext.Items["authToken"],
                storeId);
        }
        
        // ========== Store staff ========== //

        [HttpGet("[action]/{storeId}")]
        public Result<IList<StorePermission>> StorePermissionForUser(string storeId)
        {
            return _inStoreService.GetStorePermission((string) HttpContext.Items["authToken"],
                storeId);
        }
        
        [HttpGet("{storeId}/staff")]
        public Result<IList<StaffPermission>> GetStoreStaffPermissions(string storeId)
        {
            return _inStoreService.GetStoreStaffAndTheirPermissions((string) HttpContext.Items["authToken"],
                storeId);
        }
        
        [HttpPost("{storeId}/staff")]
        public Result AppointStaff(string storeId, string role, string userId)
        {
            string token = (string) HttpContext.Items["authToken"];
            Result appointRes;

            switch (role)
            {
                case "owner":
                    appointRes = _userService.AppointCoOwner(token, storeId, userId); 
                    break;
                case "manager":
                    appointRes = _userService.AppointManager(token, storeId, userId);
                    break;
                default:
                    appointRes = Result.Fail("Invalid staff role");
                    break;
            }

            return appointRes;
        }
        
        [HttpDelete("{storeId}/staff")]
        public Result RemoveStaff(string storeId, string role, string userId)
        {
            string token = (string) HttpContext.Items["authToken"];
            Result removedRes;

            switch (role)
            {
                case "owner":
                    removedRes = _userService.RemoveCoOwner(token, storeId, userId); 
                    break;
                case "manager":
                    removedRes = _userService.RemoveManager(token, storeId, userId);
                    break;
                default:
                    removedRes = Result.Fail("Invalid staff role");
                    break;
            }

            return removedRes;
        }
        
        [HttpPut("{storeId}/staff")]
        public Result UpdateManagerPermissions(string storeId, string role, 
            string userId, [FromBody] JSONStorePermissions storePermissions)
        {
            string token = (string) HttpContext.Items["authToken"];
            Result updateRes;

            switch (role)
            {
                case "manager":
                    updateRes = _userService.UpdateManagerPermission(token, storeId, userId, storePermissions.StorePermissions);
                    break;
                default:
                    updateRes = Result.Fail("Invalid staff role");
                    break;
            }

            return updateRes;
        }
        
        // ========== Store policy ========== //

        [HttpPost("{storeId}/policy")]
        public Result AddPolicy(string storeId, [FromBody] SRuleNode ruleNode)
        {
            return _inStoreService.AddRuleToStorePolicy((string) HttpContext.Items["authToken"],
                storeId, ruleNode);
        }
        
        [HttpPost("{storeId}/discount")]
        public Result AddDiscount(string storeId, [FromBody] SDiscountNode discountNode)
        {
            return _inStoreService.AddDiscountToStore((string) HttpContext.Items["authToken"],
                storeId, discountNode);
        }
        
        // ========== Store bids ========== //
        
        [HttpPost("{storeId}/askToBid")]
        public Result AskToBidOnItem(string storeId, string itemId, int amount, int newPrice )
        {
            return _inStoreService.AskToBidOnItem((string) HttpContext.Items["authToken"],itemId
                , storeId, amount, newPrice);
        }
        
        [HttpGet("{storeId}/getAllBids")]
        public Result<List<BidInfo>> GetAllBidsWaitingToApprove(string storeId)
        {
            Result<List<BidInfo>> output = _inStoreService.GetAllBidsWaitingToApprove((string) HttpContext.Items["authToken"],
                storeId);
            return output;
        }
        
        [HttpPost("{storeId}/approveOrDisapproveBid")]
        public Result ApproveOrDisapproveBid(string storeId, string bidID, bool shouldApprove )
        {
            Console.WriteLine("storeID "+storeId+"\nbidID: "+bidID+"\nshouldApprove: "+shouldApprove);
            return _inStoreService.ApproveOrDisapproveBid((string) HttpContext.Items["authToken"],storeId,bidID,shouldApprove);
        }
        

    }
    
    
    
}