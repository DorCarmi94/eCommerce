using System;
using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Common;
using eCommerce.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eCommerce.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
            _userService = new UserService();
        }

        [HttpGet("[action]")]
        public Result<List<string>> GetALlStoreIds()
        {
            return _userService.GetAllStoreIds((string) HttpContext.Items["authToken"]);
        }
        
        [HttpGet("[action]")]
        public Result<IList<string>> ALlManagedStoreIds()
        {
            return _userService.GetAllManagedStoreIds((string) HttpContext.Items["authToken"]);
        }
        
        [HttpGet("[action]")]
        public Result<UserBasicInfo> GetUserBasicInfo()
        {
            return _userService.GetUserBasicInfo((string) HttpContext.Items["authToken"]);
        }
        
        [HttpGet("purchaseHistory")]
        public Result<SPurchaseHistory> GePurchaseHistory()
        {
            return _userService.GetPurchaseHistory((string) HttpContext.Items["authToken"]);
        }
        
        [HttpGet("{userId}/userHistory")]
        public Result<SPurchaseHistory> GePurchaseHistory(string userId)
        {
            return _userService.AdminGetPurchaseHistoryUser((string) HttpContext.Items["authToken"],
                userId);
        }
    }
}