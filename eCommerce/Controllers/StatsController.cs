using System;
using System.Threading.Tasks;
using eCommerce.Common;
using eCommerce.Service;
using eCommerce.Statistics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eCommerce.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly ILogger<StatsController> _logger;
        private readonly IUserService _userService;

        public StatsController(ILogger<StatsController> logger)
        {
            _logger = logger;
            _userService = new UserService();
        }
        
        [HttpGet("[action]")]
        public Result<LoginDateStat> LoginStats(DateTime date)
        {
            return _userService.AdminGetLoginStats((string) HttpContext.Items["authToken"],
                date);
        }
    }
}