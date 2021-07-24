using System;
using System.Collections.Generic;
using eCommerce.Business;

using eCommerce.Common;
using eCommerce.Publisher;

namespace Tests.Business.Mokups
{
    public class mokUser : User
    {
        private bool isLoggedIn = false;
        

        public mokUser(string username) : base(username)
        {
            this.Username = username;
        }

        public Result Login(UserToSystemState systemState)
        {
            if (!isLoggedIn)
            {
                this.isLoggedIn = true;
                return Result.Ok();
            }
            else
            {
                return Result.Fail("User already logged in");
            }
        }

        public Result Login()
        {
            throw new System.NotImplementedException();
        }

        public Result Logout()
        {
            if (isLoggedIn)
            {
                this.isLoggedIn = false;
                return Result.Ok();
            }
            else
            {
                return Result.Fail("User already logged in");
            }
        }


        public MemberInfo MemberInfo
        {
            get
            {
                return base.MemberInfo;
            }
            set
            {
                base.MemberInfo = value;
            }
        }

        public void SetMemberInfo(MemberInfo memberInfo)
        {
            this.MemberInfo = memberInfo;
        }

        public override Result OpenStore(Store store)
        {
            throw new System.NotImplementedException();
        }

        public override Result<List<string>> GetStoreIds()
        {
            throw new System.NotImplementedException();
        }

        public override Result<IList<string>> GetManagedStoreIds()
        {
            throw new NotImplementedException();
        }

        public override Result AddItemToCart(ItemInfo item)
        {
            return this._myCart.AddItemToCart(this, item);
        }

        Result<ICart> GetCartInfo()
        {
            throw new System.NotImplementedException();
        }

        public override Result EditCart(ItemInfo info)
        {
            throw new System.NotImplementedException();
        }

        public override Result AppointUserToOwner(Store store, User user)
        {
            throw new System.NotImplementedException();
        }

        public override Result AppointUserToManager(Store store, User user)
        {
            throw new System.NotImplementedException();
        }

        public override Result RemoveOwnerFromStore(Store store, User user)
        {
            throw new NotImplementedException();
        }

        public override Result EnterRecordToHistory(PurchaseRecord record)
        {
            return Result.Ok();
        }

        public override Result<OwnerAppointment> MakeOwner(Store store)
        {
            throw new System.NotImplementedException();
        }

        public override Result<ManagerAppointment> MakeManager(Store store)
        {
            throw new System.NotImplementedException();
        }

        public override Result<OwnerAppointment> RemoveOwner(Store store)
        {
            throw new NotImplementedException();
        }

        public override Result<ManagerAppointment> RemoveManager(Store store)
        {
            throw new NotImplementedException();
        }

        public override Result AnnexStakeholders(Store store, IList<OwnerAppointment> owners, IList<ManagerAppointment> managers)
        {
            throw new NotImplementedException();
        }

        public override Result PublishMessage(string message)
        {
            MainPublisher publisher = MainPublisher.Instance;
            if (publisher == null)
                return Result.Fail("user can not access publisher");
            //@TODO_sharon:: find out whether 'userID' or 'Username' sould be passed
            publisher.AddMessageToUser(this.Username, message);
            return Result.Ok();
        }

        public override UserToSystemState GetState()
        {
            throw new System.NotImplementedException();
        }

        public override Result BuyWholeCart(PaymentInfo paymentInfo)
        {
            throw new System.NotImplementedException();
        }

        public override Result UpdatePermissionsToManager(Store store, User user, IList<StorePermission> permission)
        {
            throw new System.NotImplementedException();
        }

        /*public override Result RemovePermissionsToManager(Store store, User user, StorePermission permission)
        {
            throw new System.NotImplementedException();
        }*/

        public override Result<IList<User>> GetAllStoreStakeholders(Member member, Store store)
        {
            throw new System.NotImplementedException();
        }

        public override Result<IList<PurchaseRecord>> GetUserPurchaseHistory()
        {
            throw new System.NotImplementedException();
        }

        public override Result<IList<PurchaseRecord>> GetUserPurchaseHistory(User otherUser)
        {
            throw new System.NotImplementedException();
        }

        public override Result<IList<PurchaseRecord>> GetStorePurchaseHistory(Store store)
        {
            throw new System.NotImplementedException();
        }

        public override Result HasPermission(Store store, StorePermission storePermission)
        {
            return Result.Ok();
        }

        public string Username { get; }
    }
}