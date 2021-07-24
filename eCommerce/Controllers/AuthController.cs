using System;
using System.Threading.Tasks;
using eCommerce.Common;
using eCommerce.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eCommerce.Controllers
{

    public class LoginInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
    
    public class MemberInfo : Business.MemberInfo
    {
        public string Password { get; set; }

        public MemberInfo(string username, string email, string name, DateTime birthday, string address, string password) : base(username, email, name, birthday, address)
        {
            this.Password = password;
        }
    }
    
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
            _authService = new AuthService();
        }
        
        [HttpGet("[action]")]
        public string Connect()
        {
            string token = _authService.Connect();
            Response.Cookies.Append("_auth", token, new CookieOptions()
            {
                Path = "/",
                Secure = true,
                MaxAge = TimeSpan.FromDays(5),
                Domain = Request.PathBase.Value,
                Expires = DateTimeOffset.Now.AddDays(5),
                HttpOnly = true
            });
            Response.Headers.Add("RedirectTo", "/");
            return token;
        }
        
        [HttpPost("[action]")]
        public async Task<Result<string>> Login([FromBody] LoginInfo loginInfo)
        {
            if (Enum.TryParse<ServiceUserRole>(loginInfo.Role, true, out var serviceRole))
            {
                Result<string> loginRes = await _authService.Login((string) HttpContext.Items["authToken"],
                    loginInfo.Username, loginInfo.Password, serviceRole);
                if (loginRes.IsSuccess)
                {
                    Response.Cookies.Append("_auth", loginRes.Value, new CookieOptions()
                    {
                        Path = "/",
                        Secure = true,
                        MaxAge = TimeSpan.FromDays(5),
                        Domain = Request.PathBase.Value,
                        Expires = DateTimeOffset.Now.AddDays(5),
                        HttpOnly = true
                    });
                    Response.Headers.Add("RedirectTo", "/");
                }
                
                return loginRes;
            }

            return Result.Fail<string>("Invalid role");
        }
        
        [HttpGet("[action]")]
        public Result<string> Logout()
        {
            Result<string> logoutRes = _authService.Logout((string) HttpContext.Items["authToken"]);

            if (logoutRes.IsSuccess)
            {
                Response.Cookies.Append("_auth", logoutRes.Value, new CookieOptions()
                {
                    Path = "/",
                    Secure = true,
                    MaxAge = TimeSpan.FromDays(5),
                    Domain = Request.PathBase.Value,
                    Expires = DateTimeOffset.Now.AddDays(5),
                    HttpOnly = true
                });
                Response.Headers.Add("RedirectTo", "/");
            }
            
            return logoutRes;
        }
        
        [Route("[action]")]
        public async Task<Result> Register([FromBody] MemberInfo memberInfo)
        {
            Result registerRes = await _authService.Register((string) HttpContext.Items["authToken"],
                memberInfo, memberInfo.Password);
            if (registerRes.IsSuccess)
            {
                Response.Headers.Add("RedirectTo", "/");
            }

            return registerRes;
        }
    }
}