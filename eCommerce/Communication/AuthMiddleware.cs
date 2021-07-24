using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.Controllers;
using eCommerce.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace eCommerce.Communication
{
    public class AuthMiddleware
    {
        private const string AUTH_COOKIE = "_auth";
        
        private readonly RequestDelegate _next;
        private readonly IAuthService _authService;
        
        
        public AuthMiddleware(RequestDelegate next,
            IActionDescriptorCollectionProvider provider)
        {
            _next = next;
            _authService = new AuthService();
            
             var controllers = provider.ActionDescriptors.Items.Where(
                 ad => ad.AttributeRouteInfo != null).Select(
                 ad => ad.AttributeRouteInfo.Template
             ).ToList();
             
            Console.WriteLine("Controllers:");
            foreach (var controller in controllers)
            {
                if (controller != null)
                {
                    Console.WriteLine(controller);
                }
            }
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            var authCookie = context.Request.Cookies[AUTH_COOKIE];
            var path = context.Request.Path.Value;
            
            if ((authCookie == null && path != null) || !_authService.IsUserConnected(authCookie))
            {
                string token = _authService.Connect();
                context.Response.Cookies.Append("_auth", token, new CookieOptions()
                {
                    Path = "/",
                    Secure = true,
                    MaxAge = TimeSpan.FromDays(5),
                    Domain = context.Request.PathBase.Value,
                    Expires = DateTimeOffset.Now.AddDays(5),
                    HttpOnly = true
                });
                authCookie = token;
            }

            context.Items["authToken"] = authCookie;
            await _next(context);
        }
    }
}