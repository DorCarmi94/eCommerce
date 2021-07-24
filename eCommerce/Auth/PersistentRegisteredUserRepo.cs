using System;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.Auth.DAL;
using eCommerce.Business;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Auth
{
    public class PersistentRegisteredUserRepo : IRegisteredUserRepo
    {
        private UserContextFactory _contextFactory;
        
        public PersistentRegisteredUserRepo()
        {
            _contextFactory = new UserContextFactory();
        }

        public async Task<bool> Add(AuthUser authUser)
        {
            bool added = false;
            try
            {
                using (var context = _contextFactory.Create())
                {
                    await context.User.AddAsync(authUser);
                    added = await context.SaveChangesAsync() == 1;

                }
            } catch (Exception e)
            {
                if (!CheckConnection())
                {
                    MarketState.GetInstance().SetErrorState("Bad connection to db",this.CheckConnection);
                }
                return false;
            }

            return added;
        }

        public async Task<AuthUser> GetUserOrNull(string username)
        {
            AuthUser authUser = null;
            
                try
                {
                    using (var context = _contextFactory.Create())
                    {
                        authUser = await context.User.SingleAsync(user => user.Username.Equals(username));
                    }
                }
                catch (Exception e)
                {
                    if (!CheckConnection())
                    {
                        MarketState.GetInstance().SetErrorState("Bad connection to db",this.CheckConnection);
                    }
                    return null;
                }
            

            return authUser;
        }

        public bool CheckConnection()
        {
            return _contextFactory.Create().Database.CanConnect();
        }
    }
}