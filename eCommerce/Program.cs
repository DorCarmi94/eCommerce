using System;
using eCommerce.Adapters;
using eCommerce.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace eCommerce
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ISystemService systemService = new SystemService();
            if (systemService.InitSystem(args))
            {
                systemService.Start(args);
            }
        }
    }
}