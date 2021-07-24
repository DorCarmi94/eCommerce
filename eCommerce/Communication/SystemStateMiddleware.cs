using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eCommerce.Controllers;
using eCommerce.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace eCommerce.Communication
{
    public class SystemStateMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISystemService _systemService;
        
        
        public SystemStateMiddleware(RequestDelegate next,
            IActionDescriptorCollectionProvider provider)
        {
            _next = next;
            _systemService = new SystemService();
            
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            string errMessage;
            byte[] buffer;
            if (_systemService.GetErrMessageIfValidSystem(out errMessage))
            {
                context.Response.StatusCode = 503;
                context.Response.ContentType = "text/html; charset=utf-8";
                buffer = Encoding.UTF8.GetBytes($"<h1>Server error</h1><p>{errMessage}</p>");
                context.Response.ContentLength = buffer.Length;
                
                using (var stream = context.Response.Body)
                {
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                    await stream.FlushAsync();
                }  
                
                return;
            }
            
            await _next(context);
        }
    }
}