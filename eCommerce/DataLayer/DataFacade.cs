using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using eCommerce.Business;
using eCommerce.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace eCommerce.DataLayer
{
    public class DataFacade
    {

    private static readonly DataFacade instance = new DataFacade();  
    // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit  

    static DataFacade(){}
    protected DataFacade(){}  
    public static DataFacade Instance  
    {  
        get  
        {  
            return instance;  
        }  
    
    }

    private ECommerceContext db;
    private Queue<Object> EntitiesToRemove = new Queue<object>();
    
        public void init()
        {
            lock (this)
            {
                try
                {
                    db = new ECommerceContext();
                }
                catch (Exception e)
                {
                    if (!CheckConnection())
                    {
                        MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
                    }

                    Console.WriteLine(e);
                }
            }
        }

        public void init(bool test)
        {
            lock (this)
            {
                try
                {
                    if (test)
                        db = new testContext();
                    else
                    {
                        db = new ECommerceContext();
                    }
                }
                catch (Exception e)
                {
                    if (!CheckConnection())
                    {
                        MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
                    }

                    Console.WriteLine(e);
                }
            }
        }
        
        
        public bool CheckConnection()
        {
            if (db == null)
                return true;
            return db.Database.CanConnect();
            
        }

        #region User functions

        public Result SaveUser(User user)
        {
            lock (this)
            {
                try
                {
                    db.Add(user);

                    Console.WriteLine($"Inserting a new User - [{user.Username}]");
                    db.SaveChanges();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    if (!CheckConnection())
                    {
                        MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
                    }

                    return Result.Fail("Unable to Save User");
                }

                return Result.Ok();
            }
        }


        public Result UpdateUser(User user)
        {
            lock (this)
            {
                try
                {
                    // db.Update(user);
                    db.SaveChanges();
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    if (!CheckConnection())
                    {
                        MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
                    }

                    return Result.Fail("Unable to Update User");
                    // add logging here
                }

                return Result.Ok();
            }
        }

        public Result<User> ReadUser(string username)
        {
            lock (this)
            {
                User user = null;

                try
                {
                    Console.WriteLine("fetching saved User");
                    user = db.Users
                        .Include(u => u.MemberInfo)
                        .Include(u => u.StoresFounded)
                        // include inner Owned-Stores entities
                        .Include(u => u.StoresOwned)
                        .ThenInclude(p => p.Value)
                        .ThenInclude(o => o.User)
                        // // include inner Managed-Stores entities
                        .Include(u => u.StoresManaged)
                        .ThenInclude(p => p.Value)
                        .ThenInclude(m => m.User)
                        // // include inner Appointed-Owners entities
                        .Include(u => u.AppointedOwners)
                        .ThenInclude(p => p.ValList)
                        .ThenInclude(o => o.User)
                        // include inner Appointed-Managers entities
                        .Include(u => u.AppointedManagers)
                        .ThenInclude(p => p.ValList)
                        .ThenInclude(m => m.User)
                        //include inner Transaction history
                        .Include(u => u._transHistory)
                        .ThenInclude(h => h._purchases)
                        .ThenInclude(pr => pr.BasketInfo)
                        .ThenInclude(bi => bi.ItemsInBasket)
                        .ThenInclude(i => i._store)
                        .Include(u => u._transHistory)
                        .ThenInclude(h => h._purchases)
                        .SingleOrDefault(u => u.Username == username);

                    if (user == null)
                    {
                        return Result.Fail<User>($"No user called {username}");
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    if (!CheckConnection())
                    {
                        MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
                    }

                    return Result.Fail<User>("Unable to read User");
                }

                // synchronize all fields which arent compatible with EF (all dictionary properties) 
                var cartRes = ReadCart(username);
                user.setState();
                return Result.Ok<User>(user);
            }
        }

        #endregion

        #region Store functions

        public Result SaveStore(Store store)
        {
            lock (this)
            {
                try
                {
                    var storesLst = db.Stores.Where(x => x._storeName.Equals(store._storeName)).ToList();
                    if (storesLst.Count > 0)
                    {
                        return Result.Fail("Store with the same name already exist");
                    }

                    db.Add(store);
                    db.SaveChanges();
                }
                catch (Exception e)
                {

                    if (!CheckConnection())
                    {
                        MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
                    }

                    Console.WriteLine(e);
                    Console.WriteLine(e.InnerException);
                    return Result.Fail($"Unable to Save Store {store.StoreName}");
                }

                return Result.Ok();
            }
        }

        public Result UpdateStore(Store store)
        {
            lock (this)
            {
                try
                {
                    store.OwnersIds = string.Join(";", store._ownersAppointments.Select(o => o.OwnerId));
                    store.ManagersIds = string.Join(";", store._managersAppointments.Select(m =>
                    {
                        m.syncFromDict();
                        return m.ManagerId;
                    }));
                    store.basketsIds = string.Join(";", store.GetBasketsOfMembers().Select(b => b.BasketID));
                    
                    db.SaveChanges();
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    
                    if (!CheckConnection())
                    {
                        MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
                    }

                    return Result.Fail($"Unable to Update Store {store.StoreName}");
                }

                return Result.Ok();
            }
        }

        public Result<List<Item>> GetAllItems()
        {
            lock (this)
            {
                IIncludableQueryable<Item, Store> lst;
                try
                {
                    lst = db.Items
                        .Include(i => i._category)
                        .Include(i => i._belongsToStore);
                }
                catch (Exception e)
                {
                    if (!CheckConnection())
                    {
                        MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
                    }

                    return Result.Fail<List<Item>>("Error getting items from db");
                }

                return Result.Ok(lst.ToList());
            }

        }

        public Result<Store> ReadStore(string storename)
        {
            lock (this)
            {
                Store store = null;
                try
                {
                    store = db.Stores
                        .Where(s => s._storeName == storename)
                        // Include Transaction-history and every sub property
                        .Include(s => s._transactionHistory)
                        .ThenInclude(th => th._history)
                        .ThenInclude(pr => pr.BasketInfo)
                        .ThenInclude(bi => bi.ItemsInBasket)
                        .ThenInclude(i => i._store)
                        .Include(u => u._transactionHistory)
                        .ThenInclude(h => h._history)
                        // //Include Item-Inventory and every sub property
                        // .Include(s => s._inventory)
                        // .ThenInclude( ii => ii._aquiredItems)
                        .Include(s => s._inventory)
                        .ThenInclude(ii => ii._itemsInStore)
                        .ThenInclude(i => i._category)
                        .SingleOrDefault();

                    Console.WriteLine("fetching saved Store");

                    if (store == null)
                    {
                        return Result.Fail<Store>($"No store called {storename}");
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    if (!CheckConnection())
                    {
                        MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
                    }

                    return Result.Fail<Store>("Unable to read Store");
                }

                /*synchronize all fields which arent compatible with EF (all dictionary properties)*/

                var userRes = ReadUser(store._founderName);
                if (userRes.IsFailure)
                    return Result.Fail<Store>("In ReadStore -  Get Founder: " + userRes.Error);
                store._founder = userRes.Value;

                var ownersIds = store.OwnersIds.Split(";", StringSplitOptions.RemoveEmptyEntries);
                store._ownersAppointments = new List<OwnerAppointment>();
                foreach (var ownerId in ownersIds)
                {
                    var res = ReadOwner(ownerId);
                    if (res.IsFailure)
                        return Result.Fail<Store>("In ReadStore -  Get Owner: " + res.Error);
                    store._ownersAppointments.Add(res.Value);
                }

                var managersIds = store.ManagersIds.Split(";", StringSplitOptions.RemoveEmptyEntries);
                store._managersAppointments = new List<ManagerAppointment>();
                foreach (var managerId in managersIds)
                {
                    var res = ReadManager(managerId);
                    if (res.IsFailure)
                        return Result.Fail<Store>("In ReadStore -  Get Manager: " + res.Error);
                    res.Value.syncToDict();
                    store._managersAppointments.Add(res.Value);
                }

                var BasketsIds = store.basketsIds.Split(";", StringSplitOptions.RemoveEmptyEntries);
                var baskets = new List<Basket>();
                foreach (var basketId in BasketsIds)
                {
                    var res = ReadBasket(basketId);
                    if (res.IsFailure)
                        return Result.Fail<Store>("In ReadStore -  Get Basket: " + res.Error);
                    baskets.Add(res.Value);
                }
                store.SetBasketsOfMembers(baskets);
                return Result.Ok<Store>(store);
            }
        }

        public void UpdateManager(ManagerAppointment managerAppointment)
        {
            lock (this)
            {
                try
                {
                    managerAppointment.syncFromDict();
                    db.Entry(managerAppointment).Property(x => x.permissionsString).IsModified = true;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    if (!CheckConnection())
                    {
                        MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
                    }
                }
            }
        }



        #endregion

    private Result<OwnerAppointment> ReadOwner(string ownerId)
    {
        lock (this)
        {
            OwnerAppointment owner = null;
            try
            {
                owner = db.OwnerAppointments
                    .Where(o => o.OwnerId == ownerId)
                    .Include(o => o.User)
                    .SingleOrDefault();
                Console.WriteLine("fetching saved Owner");

                if (owner == null)
                {
                    return Result.Fail<OwnerAppointment>($"No Owner called {ownerId}");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                if (!CheckConnection())
                {
                    MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
                }

                return Result.Fail<OwnerAppointment>("Unable to read Owner");
            }

            return Result.Ok<OwnerAppointment>(owner);
        }
    }
    
    private Result<ManagerAppointment> ReadManager(string managerId)
    {
        lock (this)
        {
            ManagerAppointment manager = null;
            try
            {
                manager = db.ManagerAppointments
                    .Where(m => m.ManagerId == managerId)
                    .Include(m => m.User)
                    .SingleOrDefault();
                Console.WriteLine("fetching saved Manager");

                if (manager == null)
                {
                    return Result.Fail<ManagerAppointment>($"No Manager called {managerId}");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                if (!CheckConnection())
                {
                    MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
                }

                return Result.Fail<ManagerAppointment>("Unable to read Manager");
            }

            //sync permissions!!!
            return Result.Ok<ManagerAppointment>(manager);
        }
    }

    //TODO:: add delete basketS method
    
    public Result<Basket> ReadBasket(string basketId)
    {
        lock (this)
        {
            Basket basket = null;
            try
            {
                basket = db.Baskets
                    .Where(b => b.BasketID == basketId)
                    .Include(b => b._store)
                    .Include(b => b._cart)
                    .Include(b => b._itemsInBasket)
                    .ThenInclude(i => i.theItem)
                    .Include(b => b._itemsInBasket)
                    .ThenInclude(i => i._store)
                    .SingleOrDefault();
                Console.WriteLine("fetching saved Basket");

                if (basket == null)
                {
                    return Result.Fail<Basket>($"No Basket called {basketId}");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                if (!CheckConnection())
                {
                    MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
                }

                return Result.Fail<Basket>("Unable to read Basket");
                // add logging here
            }

            return Result.Ok<Basket>(basket);
        }
    }
    

    public void RemoveEntity(Object entity)
    {
        lock (this)
        {
                EntitiesToRemove.Enqueue(entity);
                Console.WriteLine("Queue removal of " + entity.GetType() + " from DB");
        }
    }


    public bool RemoveLingeringEntities()
    {
        
        try
        {
            while (EntitiesToRemove.Count > 0)
            {
                var entity = EntitiesToRemove.Dequeue();
                db.Remove(entity);
            }

            db.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            if (!CheckConnection())
            {
                MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
            }
            return false;
        }

        return true;
    }

    public void RemovePair<K,V>(Pair<K,V> pair)
    {
        lock (this)
        {
            try
            {
                Console.WriteLine("removing " + pair.Value.GetType() + " from DB");
                db.Remove(pair.Value);
                db.Remove(pair);
                db.SaveChanges();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                if(db!=null)
                    MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
            }
        }
    }

    
    public void RemoveListPair<K,V>(ListPair<K,V> pair)
    {
        lock (this)
        {
            try
            {

                var valQueue = new Queue<V>(pair.ValList);
                while (valQueue.Count > 0)
                {
                    var val = valQueue.Dequeue();
                    db.Remove(val);
                }

                db.Remove(pair);
                Console.WriteLine("removing " + pair.ValList.GetType() + " from DB");
                db.SaveChanges();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                if(db!=null)
                    MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
            }
        }
    }

    #region SQL Transactions

    public IDbContextTransaction BeginTransaction()
    {
        // lock db
        lock (this)
        {
            var transaction = db.Database.BeginTransaction();
            return transaction;
        }
    }
    public void CommitTransaction(IDbContextTransaction transaction)
    {
        // lock db
        lock (this)
        {
            transaction.Commit();
        }
    }
    public void RollbackTransaction(IDbContextTransaction transaction)
    {
        // lock db
        lock (this)
        {
            transaction.Rollback();
        }
    }
    

    #endregion
    
        #region Cart functions
        
        public Result<Cart> ReadCart(string holderId)
        {
            lock (this)
            {
                Cart cart = null;

                try
                {
                    Console.WriteLine("fetching saved Cart");

                    cart = db.Carts
                        .Where(c => c.CardID == holderId)
                        .Include(c => c._baskets)
                        .ThenInclude(bp => bp.Key)
                        .Include(c => c._baskets)
                        .ThenInclude(bp => bp.Value)
                        .ThenInclude(b => b._cart)
                        .Include(c => c._baskets)
                        .ThenInclude(bp => bp.Value)
                        .ThenInclude(b => b._store)
                        .Include(c => c._baskets)
                        .ThenInclude(bp => bp.Value)
                        .ThenInclude(b => b._itemsInBasket)
                        .ThenInclude(i => i.theItem)
                        .Include(c => c._baskets)
                        .ThenInclude(bp => bp.Value)
                        .ThenInclude(b => b._itemsInBasket)
                        .ThenInclude(i => i._store)
                        // .Include(c => c._baskets)
                        // .ThenInclude(bp => bp.Value)
                        // .ThenInclude(b => b._nameToItem)
                        // .ThenInclude(n2i => n2i.Value)
                        .Include(c => c._cartHolder)

                        .SingleOrDefault();

                    if (cart == null)
                    {
                        return Result.Fail<Cart>($"No cart for user called \'{holderId}\'");
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    if (!CheckConnection())
                    {
                        MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
                    }

                    return Result.Fail<Cart>("Unable to read Cart");
                }

                // synchronize all fields which arent compatible with EF (all dictionary properties) 

                return Result.Ok<Cart>(cart);
            }
        }
      
        #endregion
        
        #region Test functions
        
        //ClearTables function is reserved for Testing purposes.  
        public Result ClearTables()
        {
            lock (this)
            {
                try
                {
                    db = new testContext();
                    Console.WriteLine("clearing DB!");
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    if (!CheckConnection())
                    {
                        MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
                    }

                    return Result.Fail("Unable clear Database");
                }

                return Result.Ok();
            }
        }

        //ResetConnection function is reserved for Testing purposes.  
        public Result ResetConnection()
        {
            lock (this)
            {
                try
                {
                    db = new testContext();
                    Console.WriteLine("reset DB!");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    if (!CheckConnection())
                    {
                        MarketState.GetInstance().SetErrorState("Bad connection to db", this.CheckConnection);
                    }

                    return Result.Fail("Unable to reset Database connection");
                }

                return Result.Ok();
            }
        }

        // TODO check if to update market state
        public Result SaveLocalUser(User user)
        {
            lock (this)
            {
                db.Users.Add(user);
                return Result.Ok();
            }
        }

        public Result<User> ReadLocalUser(string username)
        {
            lock (this)
            {
                var user = db.Users.Local.SingleOrDefault(u => u.Username == username);
                if (user == null)
                    return Result.Fail<User>($"Unable to read user {username}");
                return Result.Ok<User>(user);
                    
            }
        }
        #endregion

        public List<Store> GetAllStores()
        {
            try
            {
                return this.db.Stores.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<Store>();
            }
            
        }
    }
}