using System;
using eCommerce.Business;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Auth.DAL
{
    public class UserContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            AppConfig config = AppConfig.GetInstance();
            string connectionString = config.GetData("AuthDBConnectionString");
            if (connectionString == null)
            {
                config.ThrowErrorOfData("AuthDBConnectionString", "missing");
            }
            
            options.UseSqlServer(connectionString);
        }

        // DB sets
        public DbSet<AuthUser> User { get; set; }
    }
}