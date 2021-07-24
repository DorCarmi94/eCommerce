using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using eCommerce.Business.Repositories;
using eCommerce.Common;
using eCommerce.Publisher;
using eCommerce.DataLayer;
using eCommerce.Statistics;

namespace eCommerce.Business
{
    public class User
    {
        private bool _isRegistered;
        private UserToSystemState _systemState;
        private MemberInfo _memberInfo { get; set; }
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Username { get; set; }

        private Object dataLock;
        
        //MemberData:
        private LDict<string, bool> _storesFounded;
        private LDict<string, OwnerAppointment> _storesOwned;
        private LDict<string, ManagerAppointment> _storesManaged;
        private LLDict<string, OwnerAppointment> _appointedOwners;
        private LLDict<string, ManagerAppointment> _appointedManagers;
        
        public MemberInfo MemberInfo {get => _memberInfo; set =>_memberInfo = value; }
        public UserTransactionHistory _transHistory { get; set; }
        public Cart _myCart { get; private set; }
        public string _cartId { get; set; }
        

        //constructors

        public User(string Username)
        {
            this.Username = Username;
            _memberInfo = new MemberInfo(Username, null,null,DateTime.Now, null);
            _systemState = Guest.State;
            _myCart = new Cart(this);
            _cartId = _myCart.CardID;
            _isRegistered = false;
            dataLock = new Object();
            _role = _systemState.GetRole();
        }
        public User(MemberInfo memberInfo)
        {
            _systemState = Member.State;
            InitMember(memberInfo);
        } 
        
        public User(UserToSystemState state, MemberInfo memberInfo)
        {
            _systemState = state;
            InitMember(memberInfo);
        }

        private void InitMember(MemberInfo memberInfo)
        {
            _isRegistered = true;
            _memberInfo = memberInfo;
            Username = memberInfo.Username;
            dataLock = new Object();
            _role = _systemState.GetRole();
            _myCart = new Cart(this);
            _cartId = _myCart.CardID;
            _storesFounded = new LDict<string, bool>();
            _storesOwned = new LDict<string, OwnerAppointment>();
            _storesManaged = new LDict<string, ManagerAppointment>();
            _appointedOwners = new LLDict<string, OwnerAppointment>();
            _appointedManagers = new LLDict<string, ManagerAppointment>();
            _transHistory = new UserTransactionHistory(Username);
        }

        #region User Facacde Interface
        public virtual Result Login(UserToSystemState systemState, MemberData memberData)
        {
            if (systemState == null)
            {
                return Result.Fail("Invalid State for user " + Username);
            }

            if (memberData == null)
            {
                return Result.Fail("Invalid memberData for user " + Username);
            }

            return _systemState.Login(this,systemState, memberData);
        }


        public virtual Result Logout(string toGuestName)
        {
            return _systemState.Logout(this,toGuestName);
        }

        public virtual Result<bool> IsRegistered()
        {
            return Result.Ok(_isRegistered);
        }

        /// <TEST> UserTest.TestOpenStore </TEST>
        /// <UC> 'Open a Store' </UC>
        /// <REQ> 3.2 </REQ>
        /// <summary>
        ///  receives an Store to open. makes this User a founder and an owner. 
        /// </summary>
        /// <param name="store"></param>
        /// <returns>Result, OK/Fail. </returns>
        public virtual Result OpenStore(Store store)
        {
            return _systemState.OpenStore(this, store);
        }
        
        public virtual Result<List<string>> GetStoreIds()
        {
            return _systemState.GetStoreIds(this);
        }
        
        public virtual Result<IList<string>> GetManagedStoreIds()
        {
            return _systemState.GetManagedStoreIds(this);
        }

        /// <TEST>  </TEST>
        /// <UC> 'Add product to basket' </UC>
        /// <REQ> 2.7 </REQ>
        /// <summary>
        ///  adds the given items to the user's cart. 
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Result, OK/Fail. </returns>
        public virtual Result AddItemToCart(ItemInfo item)
        {
            return _myCart.AddItemToCart(this, item);
        }

        /// <TEST>  </TEST>
        /// <UC> 'View shopping cart' </UC>
        /// <REQ> 2.8 </REQ>
        /// <summary>
        ///  gets the information from the cart. 
        /// </summary>
        /// <returns>Result, ICart. </returns>
        public virtual Result<Cart> GetCartInfo()
        {
            return Result.Ok<Cart>(_myCart);
        }

        /// <TEST>  </TEST>
        /// <UC> 'Edit shopping cart' </UC>
        /// <REQ> 2.8 </REQ>
        /// <summary>
        ///  changes the given items in user's cart. 
        /// </summary>
        /// <param name="info"></param>
        /// <returns>Result, OK/Fail . </returns>
        public virtual Result EditCart(ItemInfo info)
        {
            return _myCart.EditCartItem(this, info);
        }

        public virtual Result BuyWholeCart(PaymentInfo paymentInfo)
        {
            return this._myCart.BuyWholeCart(this, paymentInfo);
        }
      
        /// <TEST> UserTest.TestAppointUserToOwner </TEST>
        /// <UC> 'Nominate member to be store owner' </UC>
        /// <REQ> 4.5 </REQ>
        /// <summary>
        ///  make the user an owner of the given store. 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="store"></param>
        /// <returns>Result, OK/Fail. </returns>
        public virtual Result AppointUserToOwner(Store store, User user)
        {
            return _systemState.AppointUserToOwner(this, store, user);
        }

        /// <TEST> UserTest.TestAppointUserToManager </TEST>
        /// <UC> 'Nominate member to be store manager' </UC>
        /// <REQ> 4.3 </REQ>
        /// <summary>
        ///  make the user a manager of the given store. 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="store"></param>
        /// <returns>Result, OK/Fail. </returns>
        public virtual Result AppointUserToManager(Store store, User user)
        {
            return _systemState.AppointUserToManager(this, store, user);
        }
        
        /// <TEST> UserTest.TestRemoveOwnerFromStore </TEST>
        /// <UC> 'Remove member from store ownership' </UC> //@TODO_Sharon : make sure use case documentation matches this
        /// <REQ> 4.4 </REQ>
        /// <summary>
        ///  remove the user from being an owner of the given store. 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="store"></param>
        /// <returns>Result, OK/Fail. </returns>
        public virtual Result RemoveOwnerFromStore(Store store, User user)
        {
            return _systemState.RemoveOwnerFromStore(this, store, user);
        }
        
        
        
        /// <TEST> UserTest.TestUpdatePermissionsToManager </TEST>
        /// <UC> 'Change management permission for sub-manger' </UC>
        /// <REQ> 4.6 </REQ>
        /// <summary>
        ///  change manager's permissions in the given store. 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="store"></param>
        /// <param name="permissions"></param>
        /// <returns>Result, OK/Fail. </returns>
        public virtual Result UpdatePermissionsToManager(Store store, User user, IList<StorePermission> permissions)
        {
            return _systemState.UpdatePermissionsToManager(this, store, user, permissions);
        }

        /* @Deprecated */
        // public virtual Result RemovePermissionsToManager(Store store, User user, StorePermission permission)
        // {
        //     return _systemState.RemovePermissionsToManager(this, store, user, permission);
        // }
        //
        
        /// <TEST> UserTest.TestUserPurchaseHistory </TEST>
        /// <UC> 'Review purchase history' </UC>
        /// <REQ> 3.7 </REQ>
        /// <summary>
        ///  returns all purchase-records of the user. 
        /// </summary>
        public virtual Result<IList<PurchaseRecord>> GetUserPurchaseHistory()
        {
            return _systemState.GetUserPurchaseHistory(this);
        }
        
        /// <TEST> UserTest.TestAdminGetHistory </TEST>
        /// <UC> 'Admin requests for user history' </UC>
        /// <REQ> 6.4 </REQ>
        /// <summary>
        ///  returns all purchase-records of the requested user. 
        /// </summary>
        public virtual Result<IList<PurchaseRecord>> GetUserPurchaseHistory(User otherUser)
        {
            return _systemState.GetUserPurchaseHistory(this, otherUser);
        }
        
        /// <TEST> UserTest.TestGetStoreHistory </TEST>
        /// <UC> 'Member requests for purchase history for the store' </UC>
        /// <REQ> 4.11 </REQ>
        /// <summary>
        ///  returns all purchase-records of the requested store. 
        /// </summary>
        public virtual Result<IList<PurchaseRecord>> GetStorePurchaseHistory(Store store)
        {
            return _systemState.GetStorePurchaseHistory(this, store);
        }


        //User to User
        public virtual Result<OwnerAppointment> MakeOwner(Store store)
        {
            return _systemState.MakeOwner(this, store);
        }

        public virtual Result<ManagerAppointment> MakeManager(Store store)
        {
            return _systemState.MakeManager(this, store);
        }

        public virtual Result<OwnerAppointment> RemoveOwner(Store store)
        {
            return _systemState.RemoveOwner(this, store);
        }
        
        public virtual Result<ManagerAppointment> RemoveManager(Store store)
        {
            return _systemState.RemoveManager(this, store);
        }

        
        public void FreeCart()
        {
            this._myCart.Free();
        }

        public virtual Result AnnexStakeholders(Store store, IList<OwnerAppointment> owners, IList<ManagerAppointment> managers)
        {
            return _systemState.AnnexStakeholders(this,store,owners,managers);
        }
        

        public virtual UserToSystemState GetState()
        {
            return _systemState;
        }

        public string GetRole()
        {
            return _systemState.GetRole();
        }

        public virtual Result PublishMessage(string message)
        {
            MainPublisher publisher = MainPublisher.Instance;
            if (publisher == null)
                return Result.Fail("user can not access publisher");
            //@TODO_sharon:: find out whether 'userID' or 'Username' sould be passed
            publisher.AddMessageToUser(MemberInfo.Id, message);
            return Result.Ok();
        }

        //InBusiness

        /// <TEST> UserTest.TestHasPermissions </TEST>
        /// <UC> 'Change management permission for sub-manger' </UC>
        /// <REQ> 5.1 </REQ>
        /// <summary>
        ///  checks if user has the required permission. 
        /// </summary>
        public virtual Result HasPermission(Store store, StorePermission storePermission)
        {
            return _systemState.HasPermission(this, store, storePermission);
        }

        /// <TEST> UserTest.TestUserPurchaseHistory </TEST>
        /// <UC> 'Review purchase history' </UC>
        /// <REQ> 3.7 </REQ>
        /// <summary>
        ///  add records to member's history. 
        /// </summary>
        public virtual Result EnterRecordToHistory(PurchaseRecord record)
        {
            return _systemState.EnterRecordToHistory(this, record);
        }

        public virtual Result IsAdmin()
        {
            if (_role.Equals(Admin.State.GetRole()))
                return Result.Ok();
            return Result.Fail("Error deciding user Role");
        }

        public virtual string GetUserCategory()
        {
            if (_systemState.GetRole().Equals(Guest.State.GetRole()))
                return "Guest";
            if (_systemState.GetRole().Equals(Admin.State.GetRole()))
                return "Admin";
            string category = "Member";
            bool manager = StoresManaged.Count > 0;
            bool owner = StoresOwned.Count > 0;
            if (!owner & manager)
                category = "Manager";
            else if (owner)
                category = "Owner";
            else
                category = "Member";
            return category;
        }

        #endregion User Facade Interface


    #region Admin Functions
    
    public virtual Result<LoginDateStat> GetLoginStats(DateTime date)
    {
        return _systemState.GetLoginStats(date);
    }
    
    #endregion Admin Functions
    
    
    #region Guest Functions
        public virtual Result Login(Guest guest, UserToSystemState systemState, MemberData memberData)
        {
            throw new NotImplementedException();

            // Result res = memberData.Cart.MergeCarts(_myCart);
            // if (res.IsFailure)
            // {
            //     return res;
            // }

            _systemState = systemState;
            // _myData = memberData;
            // _myCart = _myData.Cart;
            // _userName = _myData.Username;
            _isRegistered = true;
            return Result.Ok();
        }
   
    #endregion Guest Functions
    
    
    #region Member Functions
        public virtual Result Logout(Member member, string toGuestName)
        {
            _systemState = Guest.State;
            throw new NotImplementedException();
            // _myData = null;
            // _userName = toGuestName;
            _myCart = new Cart(this);
            _isRegistered = false;
            return Result.Ok();
        }

        public virtual Result OpenStore(Member member, Store store)
        {
            // adds store to both Owned-By and Founded-By
            OwnerAppointment owner = new OwnerAppointment(this, store.StoreName);

            bool res =_storesFounded.TryAdd(Username,store.StoreName,store._storeName,true) 
                      && _storesOwned.TryAdd(Username,store.StoreName,store._storeName,owner);
            if (res)
            {
                return store.AppointNewOwner(this,owner);
            }
            return Result.Fail("Unable to open store");
        }

        public virtual Result AppointUserToOwner(Member member, Store store, User otherUser)
        {
            if (!_storesOwned.ContainsKey(store.StoreName))
            {
                return Result.Fail("user \'"+Username+"\' is not an owner of the given store.");
            }

            Result<OwnerAppointment> res = otherUser.MakeOwner(store);
            if (res.IsFailure)
            {
                return res;
            }

            OwnerAppointment newOwner = res.Value;
            // acquire user-data lock
            lock (dataLock)
            {
                if (!_appointedOwners.ContainsKey(store.StoreName))
                {
                    IList<OwnerAppointment> ownerList = new List<OwnerAppointment>();
                    ownerList.Add(newOwner);
                    if (!_appointedOwners.TryAdd(store.StoreName, ownerList))
                        return Result.Fail("unable to add other-user as an Appointed-Owner");
                }
                else
                {
                    _appointedOwners.KeyToValue(store.StoreName).Add(newOwner);
                }
            }//release lock
            return store.AppointNewOwner(this,newOwner);
        }

        public virtual Result AppointUserToManager(Member member, Store store, User otherUser)
        {
            if (!_storesOwned.ContainsKey(store.StoreName)){
                return Result.Fail("user \'"+Username+"\' is not an owner of the given store.");
            }
            Result<ManagerAppointment> res = otherUser.MakeManager(store);
            if (res.IsFailure){
                return res;
            }

            ManagerAppointment newManager = res.Value;
            // acquire user-data lock
            lock (dataLock)
            {
                if (!_appointedManagers.ContainsKey(store.StoreName))
                {
                    IList<ManagerAppointment> managerList = new List<ManagerAppointment>();
                    managerList.Add(newManager);
                    if (!_appointedManagers.TryAdd(store._storeName,managerList))
                        return Result.Fail("unable to add other-user as an Appointed-Manager");
                }
                else
                {
                    _appointedManagers.KeyToValue(store.StoreName).Add(newManager);
                }
            }//release lock
            return store.AppointNewManager(this,newManager);
        }

        public virtual Result<OwnerAppointment> MakeOwner(Member member, Store store)
        {
            OwnerAppointment newOwner = new OwnerAppointment(this,store.StoreName);
            if (_storesOwned.TryAdd(Username,store.StoreName,store._storeName, newOwner))
            {
                return Result.Ok<OwnerAppointment>(newOwner);
            }
            return Result.Fail<OwnerAppointment>("unable to add user \'"+Username+"\' as store owner");
        }

        public virtual Result<ManagerAppointment> MakeManager(Member member, Store store)
        {
            ManagerAppointment newManager = new ManagerAppointment(this, store.StoreName);
            if (!_storesOwned.ContainsKey(store.StoreName) 
                && _storesManaged.TryAdd(Username,store.StoreName,store._storeName, newManager))
            {
                return Result.Ok<ManagerAppointment>(newManager);
            }
            return Result.Fail<ManagerAppointment>("unable to add user \'"+Username+"\' as store Manager");
        }
        
        public  virtual Result RemoveOwnerFromStore(Member member, Store store,User otherUser)
        {
            lock (this)
            {
                if (!_storesOwned.ContainsKey(store.StoreName))
                {
                    return Result.Fail("user \'" + Username + "\' is not an owner of the given store.");
                }

                if ((!_appointedOwners.ContainsKey(store.StoreName)) || FindCoOwner(store, otherUser).IsFailure)
                {
                    return Result.Fail("user \'" + Username + "\' did not appoint the owner of the given store \'" +
                                       otherUser.Username + "\'.");
                }

                var res = otherUser.RemoveOwner(store);
                if (res.IsFailure)
                {
                    return res;
                }
                if (_appointedOwners.ContainsKey(store.StoreName))
                {
                    _appointedOwners.RemoveFromList(store.StoreName,res.Value);
                    var resFromStore=store.RemoveOwnerFromStore(this, res.Value.User, res.Value);
                    // DataFacade.Instance.RemoveEntity(res.Value);
                    return resFromStore;
                }
            }

            return Result.Fail("Something went wrong");
        }
        
        
        public virtual Result RemoveManagerFromStore(Member member, Store store,User otherUser)
        {
            if (!_storesOwned.ContainsKey(store.StoreName)){
                return Result.Fail("user \'"+Username+"\' is not a manager of the given store.");
            }
            if ((!_appointedManagers.ContainsKey(store.StoreName)) || FindCoManager(store,otherUser).IsFailure){
                return Result.Fail("user \'"+Username+"\' did not appoint the manager of the given store \'"+otherUser.Username+"\'.");
            }
            
            //TODO: Check with sharon-> why not exist RemoveManager method
            var res = otherUser.RemoveManager(store);
            if (res.IsFailure)
            {
                return res;
            }

            if (_appointedManagers.ContainsKey(store.StoreName))
            {
                
                _appointedManagers.RemoveFromList(store.StoreName,res.Value);
                var resFromStore=store.RemoveManagerFromStore(this, res.Value.User, res.Value);
                // DataFacade.Instance.RemoveEntity(res.Value);
                return resFromStore;
            }
            
    
            return Result.Ok();
        }

        public virtual Result AnnexStakeholders(Member member,Store store, IList<OwnerAppointment> owners, IList<ManagerAppointment> managers)
        {
            if (!_storesFounded.ContainsKey(store.StoreName))
            {
                return Result.Fail("user ["+Username+"] is not a founder of the store ["+store.GetStoreName()+"]");;
            }

            lock (dataLock)
            {
                if (owners != null)
                {
                    foreach (var owner in owners)
                    {
                     
                        _appointedOwners.KeyToValue(store.StoreName).Add(owner);   
                    }
                }
                if (managers != null && _appointedManagers.ContainsKey(store.StoreName))
                {
                    foreach (var manager in managers)
                    {
                     
                        _appointedManagers.KeyToValue(store.StoreName).Add(manager);   
                    }
                }
            }
            return Result.Ok();
        }

        private Result<ManagerAppointment> FindCoManager(Store store, User otherUser)
        {
            ManagerAppointment manager = null;
            if (_appointedManagers.ContainsKey(store.StoreName))
            {
                lock (dataLock)
                {
                    manager = _appointedManagers.KeyToValue(store.StoreName).FirstOrDefault((ma) => ma.User == otherUser);
                }
            }

            if (manager == null)
                return Result.Fail<ManagerAppointment>("user\'"+Username+"\' did not appoint the given manager +\'"+otherUser.Username+"\'");
            return Result.Ok<ManagerAppointment>(manager);
        }

        private Result<OwnerAppointment> FindCoOwner(Store store, User otherUser)
        {
            OwnerAppointment owner = null;
            if (_appointedOwners.ContainsKey(store.StoreName))
            {
                lock (dataLock)
                {
                    owner = _appointedOwners.KeyToValue(store.StoreName).FirstOrDefault((ma) => ma.User == otherUser);
                }
            }

            if (owner == null)
                return Result.Fail<OwnerAppointment>("user\'"+Username+"\' did not appoint the given owner +\'"+otherUser.Username+"\'");
            return Result.Ok<OwnerAppointment>(owner);
        }

        public virtual Result<OwnerAppointment> RemoveOwner(Member member, Store store)
        {
            if (!_storesOwned.ContainsKey(store.StoreName))
            {
                return Result.Fail<OwnerAppointment>("user ["+Username+"] is not an owner of the store ["+store.GetStoreName()+"]");
            }
            OwnerAppointment own;
            IList<OwnerAppointment> coowners;
            IList<ManagerAppointment> comanagers;
            
            
            string failMessage = "";
            
            
            if (_appointedOwners.ContainsKey(store.StoreName) 
                && _appointedOwners.KeyToValue(store.StoreName)!=null)
            {
                var firedList = new List<OwnerAppointment>(_appointedOwners.KeyToValue(store.StoreName));
                foreach (var owner in firedList)
                {
                    var res = RemoveOwnerFromStore(member, store, owner.User);
                    if(res.IsFailure)
                        failMessage= failMessage+";\n"+res.Error;
                }
            }


            if (_appointedManagers.ContainsKey(store.StoreName) 
                && _appointedManagers.KeyToValue(store.StoreName)!=null)
            {
                var firedList = new List<ManagerAppointment>(_appointedManagers.KeyToValue(store.StoreName));
                foreach (var manager in firedList)
                {
                    
                     var res = RemoveManagerFromStore(member, store, manager.User);
                     if(res.IsFailure)
                         failMessage= failMessage+";\n"+res.Error;
                }
            }

            own = _storesOwned.KeyToValue(store.StoreName);
            _storesOwned.Remove(store.StoreName);
            coowners = _appointedOwners.KeyToValue( store.StoreName);
            _appointedOwners.Remove(store.StoreName);
            comanagers = _appointedManagers.KeyToValue(store.StoreName);
            _appointedManagers.Remove(store.StoreName);
            if(failMessage != "")
                return Result.Fail<OwnerAppointment>(failMessage);
            return Result.Ok(own);
        }
        
        
        public virtual Result<ManagerAppointment> RemoveManager(Member member, Store store)
        {
            if (!_storesManaged.ContainsKey(store.StoreName))
            {
                return Result.Fail<ManagerAppointment>("user ["+Username+"] is not a manager of the store ["+store.GetStoreName()+"]");
            }
            ManagerAppointment mng;
            IList<ManagerAppointment> comanagers;
            
            
            
            string failMessage = "";
            
            
            // if (_appointedManagers.ContainsKey(store.StoreName) 
            //     && _appointedManagers.KeyToValue(store.StoreName)!=null)
            // {
            //     var firedList = new List<ManagerAppointment>(_appointedManagers.KeyToValue(store.StoreName));
            //     foreach (var mngr in firedList)
            //     {
            //         var res = RemoveManagerFromStore(member, store, mngr.User);
            //         if(res.IsFailure)
            //             failMessage= failMessage+";\n"+res.Error;
            //     }
            // }

            mng = _storesManaged.KeyToValue(store.StoreName);
            _storesManaged.Remove(store.StoreName);
            //comanagers = _appointedManagers.KeyToValue(store.StoreName);
            //_appointedManagers.Remove(store.StoreName);
            
            if(failMessage != "")
                return Result.Fail<ManagerAppointment>(failMessage);
            return Result.Ok(mng);
        }


        public virtual Result AddPermissionsToManager(Member member, Store store, User otherUser, StorePermission permission)
        {
            ManagerAppointment manager = null;
            var res = FindCoManager(store, otherUser);
            if (res.IsSuccess)
            {
                manager = res.Value;
                return manager.AddPermissions(permission);
            }
            return Result.Fail("user\'"+Username+"\' can not grant permissions to given manager");
        }

        public virtual Result RemovePermissionsToManager(Member member, Store store, User otherUser, StorePermission permission)
        {
            if (_appointedManagers.ContainsKey(store.StoreName))
            {
                ManagerAppointment manager = null;
                lock (dataLock)
                {
                    manager = _appointedManagers.KeyToValue(store.StoreName).FirstOrDefault((ma) => ma.User == otherUser);
                }

                if (manager != null)
                {
                    return manager.RemovePermission(permission);
                }
            }
            return Result.Fail("user\'"+Username+"\' can not remove permissions from given manager");
        }


        public virtual Result UpdatePermissionsToManager(Member member, Store store, User otherUser,
            IList<StorePermission> permissions)
        {
            if (permissions == null || permissions.Count == 0){
                return Result.Fail("Invalid permission list input");
            }
            if (_appointedManagers.ContainsKey(store.StoreName))
            {
                ManagerAppointment manager = null;
                lock (dataLock)
                {
                    manager = _appointedManagers.KeyToValue(store.StoreName).FirstOrDefault((ma) => ma.User == otherUser);
                }

                if (manager != null)
                {
                    return manager.UpdatePermissions(permissions);
                }
            }
            return Result.Fail("user\'"+Username+"\' can not remove permissions from given manager");
        }
        
        public virtual Result HasPermission(Member member, Store store, StorePermission permission)
        {
            if(_storesOwned.ContainsKey(store.StoreName))
            {
                return _storesOwned.KeyToValue(store.StoreName).HasPermission(permission);
            }

            if (_storesManaged.ContainsKey(store.StoreName))
            {
                return _storesManaged.KeyToValue(store.StoreName).HasPermission(permission);
            }
            
            return Result.Fail("user\'"+Username+"\' is not a stakeholder of the given store");
        }

        public virtual Result EnterRecordToHistory(Member member, PurchaseRecord record)
        {
            return _transHistory.EnterRecordToHistory(record);
        }
        

        public virtual Result<IList<PurchaseRecord>> GetUserHistory()
        {
            return _transHistory.GetUserHistory();
        }

        public virtual Result<IList<PurchaseRecord>> GetStoreHistory(Store store)
        {
            return store.GetPurchaseHistory(this);
        }
        
        public virtual Result<IList<User>> GetAllStoreStakeholders(Member member, Store store)
        {
            throw new NotImplementedException();
        }
        
        
    #endregion //Member Functions
        
    
    #region Test Oriented Functions

    public LDict<string, bool> StoresFounded => _storesFounded;

    public LDict<string, OwnerAppointment> StoresOwned => _storesOwned;
    
    public LDict<string, ManagerAppointment> StoresManaged => _storesManaged;
    
    public LLDict<string, OwnerAppointment> AppointedOwners => _appointedOwners;
    
    public LLDict<string,ManagerAppointment> AppointedManagers => _appointedManagers;

    #endregion

    
    #region DAL Oriented Functions

    private string _role;
    public string Role
    {
        get
        {
            return _role;
        }
        set
        {
            _role = value;
        }
    }

    public void setState()
    {
        if(_role.Equals(Member.State.GetRole()))
        {
            _systemState = Member.State;
        }
        else
        {
            _systemState = Admin.State;    
        }
    }
    public User()
    {
        _isRegistered = true;
        dataLock = new Object();
        _storesFounded = new LDict<string, bool>();
        _storesOwned = new LDict<string, OwnerAppointment>();
        _storesManaged = new LDict<string, ManagerAppointment>();
        _appointedOwners = new LLDict<string, OwnerAppointment>();
        _appointedManagers = new LLDict<string, ManagerAppointment>();
    }
    
    #endregion
    
    }
    
}