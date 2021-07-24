using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.DiscountsAndPurchases.Purchases.RulesInfo;
using eCommerce.Business.Purchases;

using eCommerce.Common;
using eCommerce.DataLayer;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.Extensions.Logging;
using NLog.Time;

namespace eCommerce.Business
{
    public class Store
    {
        //Individual
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public String _storeName { get; private set; }

        private List<Pair<User, Bid>> _UsersBids;
        
        //Store's issues
        //Discounts and purchases
        private List<Composite> _myDiscountStrategies;
        private List<PurchaseStrategy> _myPurchaseStrategies;
        private PurchasePolicy _myPurchasePolicy;
        
        //History and inventory
        public StoreTransactionHistory _transactionHistory { get; set; }
        public ItemsInventory _inventory { get; private set; }

        //User issues
        [NotMapped] 
        public User _founder { get; set; }
        public string _founderName { get; set; }

        
        
        public string OwnersIds { get; set; }
        [NotMapped]
        public List<OwnerAppointment> _ownersAppointments { get;  set; }
        
        public string ManagersIds { get; set; }
        [NotMapped]
        public List<ManagerAppointment> _managersAppointments { get; set; }

        public string basketsIds { get; set; }

        [NotMapped] private List<Basket> _basketsOfThisStore;

        
        
        public List<Basket> GetBasketsOfMembers()
        {

            return _basketsOfThisStore.Where(x => !x._cart._cartHolder.GetRole().Equals(Guest.State.GetRole()))
                .ToList();
        }
        public void SetBasketsOfMembers(List<Basket> baskets)
        {
            _basketsOfThisStore = baskets;
        }
        public List<Basket> GetAllBaskets()
        {
            return _basketsOfThisStore;
        }
        

        // for ef
        public Store()
        {
            this._myDiscountStrategies = new List<Composite>();
            this._myPurchaseStrategies = new List<PurchaseStrategy>();
            this._myPurchasePolicy = new PurchasePolicy(this);
            this._UsersBids = new List<Pair<User, Bid>>();
        }

        public Store(String name, User founder)
        {
            this._storeName = name;
            this._myDiscountStrategies = new List<Composite>();
            this._myPurchaseStrategies = new List<PurchaseStrategy>();
            this._myPurchaseStrategies.Add(new DefaultPurchaseStrategy(this));

            _myPurchasePolicy = new PurchasePolicy(this);

            _transactionHistory = new StoreTransactionHistory(this);

            _inventory = new ItemsInventory(this);

            this._founder = founder;
            this._founderName = founder.Username;

            _ownersAppointments = new List<OwnerAppointment>();
            
            _managersAppointments = new List<ManagerAppointment>();
            
            _basketsOfThisStore = new List<Basket>();
            _UsersBids = new List<Pair<User, Bid>>();

        }

        public virtual IList<Item> GetAllItems()
        {
            return this._inventory.GetAllItemsInStore();
        }
        
        public virtual Result<Item> GetItem(ItemInfo item)
        {
            return _inventory.GetItem(item);
        }

        public Result<Item> GetItem(string itemId)
        {
            return _inventory.GetItem(itemId);
        }

        public Result<IList<Tuple<string, IList<StorePermission>>>> GetStoreStaffAndTheirPermissions(User user)
        {
            IList<Tuple<string, IList<StorePermission>>> netasCrazyList = new List<Tuple<string, IList<StorePermission>>>();
            foreach (var manager in _managersAppointments)
            {
                List<StorePermission> mangPerm = new List<StorePermission>();
                foreach (var perm in Enum.GetValues(typeof(StorePermission)).Cast<StorePermission>())
                {
                    if (manager.HasPermission(perm).IsSuccess)
                    {
                        mangPerm.Add(perm);
                    }
                }
                netasCrazyList.Add(new Tuple<string, IList<StorePermission>>(manager.User.Username,mangPerm));
            }
            
            foreach (var owner in _ownersAppointments)
            {
                if (owner.User == _founder)
                {
                    continue;
                    //Founder permissions alone, next in this code
                }
                List<StorePermission> ownerPerm = new List<StorePermission>();
                foreach (var perm in Enum.GetValues(typeof(StorePermission)).Cast<StorePermission>())
                {
                    if (owner.HasPermission(perm).IsSuccess)
                    {
                        ownerPerm.Add(perm);
                    }
                }
                netasCrazyList.Add(new Tuple<string, IList<StorePermission>>(owner.User.Username,ownerPerm));
            }
            
            List<StorePermission> foundPerm = new List<StorePermission>();
            foreach (var perm in Enum.GetValues(typeof(StorePermission)).Cast<StorePermission>())
            {
                if (this._founder.HasPermission(this,perm).IsSuccess)
                {
                    foundPerm.Add(perm);
                }
            }
            netasCrazyList.Add(new Tuple<string, IList<StorePermission>>(_founder.Username,foundPerm));


            return Result.Ok<IList<Tuple<string, IList<StorePermission>>>>(netasCrazyList);
        }

        public virtual Result<IList<StorePermission>> GetPermissions(User user)
        {
            IList<StorePermission> permissions = null;
            if (user.Equals(_founder) | _ownersAppointments.Find(u => u.User.Equals(user)) != null)
            {
                return Result.Ok<IList<StorePermission>>(Enum.GetValues<StorePermission>());
            }

            if (_managersAppointments.Find(u => u.User.Equals(user)) == null)
            {
                return Result.Fail<IList<StorePermission>>("User is not a owner or manager of this store");
            }
            
            foreach (var managerAppointment in _managersAppointments)
            {
                if (managerAppointment.User.Equals(user))
                {
                    permissions = managerAppointment.GetAllPermissions();
                }
            }

            if (permissions == null)
            {
                //TODO log it as error
                return Result.Fail<IList<StorePermission>>("User is not a owner or manager of this store");
            }

            return Result.Ok(permissions);
        }

        public virtual List<Item> SearchItem(string stringSearch)
        {
            return this._inventory.SearchItem(stringSearch);
        }

        public virtual List<Item> SearchItemWithPriceFilter(string stringSearch, int startPrice, int endPrice)
        {
            return _inventory.SearchItemWithPriceFilter(stringSearch, startPrice, endPrice);
        }

        public virtual List<Item> SearchItemWithCategoryFilter(string stringSearch, string category)
        {
            return _inventory.SearchItemWithCategoryFilter(stringSearch, category);
        }


        public virtual Result AddBasketToStore(Basket basket)
        {
            lock (this)
            {
                if (this._basketsOfThisStore.FirstOrDefault(x => x.GetCart() == basket.GetCart()) != null)
                {
                    return Result.Fail("Store already contains basket for this cart");
                }
                else
                {
                    this._basketsOfThisStore.Add(basket);
                    return Result.Ok(basket);
                }
            }
        }
        public virtual Result CalculateBasketPrices(IBasket basket)
        {           
            foreach (var strategy in _myDiscountStrategies)
            {
                var res=basket.SetTotalPrice();
                if (res.IsFailure)
                {
                    return res;
                }
            }

            return Result.Ok();
        }

        public virtual Result CatchAllBasketProducts(IBasket basket)
        {
            foreach (var itemInfo in basket.GetAllItems().GetValue())
            {
                var res=this._inventory.GetItem(itemInfo);
                if (res.IsFailure)
                {
                    return res;
                }
                else
                {
                    res.GetValue().AquireItems(itemInfo);
                }
                    
            }
            return Result.Ok();
            
        }

        public virtual Result TryGetItems(ItemInfo item)
        {
            return this._inventory.TryGetItems(item);
        }

        public virtual Result FinishPurchaseOfBasket(IBasket basket)
        {
            foreach (var item in basket.GetAllItems().GetValue())
            {
                var res = this._inventory.GetItem(item);
                if (res.IsFailure)
                {
                    return res;
                }
                else
                {
                    var resFinish=res.GetValue().FinalizeGetItems(item.amount);
                    if (resFinish.IsFailure)
                    {
                        return resFinish;
                    }
                    
                    
                }
            }

            return Result.Ok();
        }

        public virtual Result FinishPurchaseOfItems(ItemInfo item)
        {
            var res = this._inventory.GetItem(item);
            if (res.IsFailure)
            {
                return res;
            }
            else
            {
                res.GetValue().FinalizeGetItems(item.amount);
            }
            return Result.Ok();
        }

        public virtual Result AddItemToStore(ItemInfo newItem, User user)
        {
            if (user.HasPermission(this, StorePermission.AddItemToStore).IsFailure)
            {
                return Result.Fail("User has now permission to add items to store");
            }
            else
            {
                if (this._inventory.GetItem(newItem).IsFailure)
                {
                    return this._inventory.AddNewItem(user, newItem);
                }
                else
                {
                    return Result.Fail("Item already exist in store");
                    //return this._inventory.AddExistingItem(user, newItem.name, newItem.amount);
                }
            }
        }

        public virtual Result EditItemToStore(ItemInfo newItem, User user)
        {
            if (user.HasPermission(this, StorePermission.AddItemToStore).IsFailure)
            {
                return Result.Fail("User has now permission to add items to store");
            }
            else
            {
                var itemGet = this._inventory.GetItem(newItem);
                if (itemGet.IsFailure)
                {
                    return Result.Fail("Item doesn't exist");
                }
                else
                {
                    return itemGet.GetValue().EditItem(newItem);
                    
                }
            }
        }

        public virtual Result UpdateStock_AddItems(ItemInfo newItem, User user)
        {
            if (user.HasPermission(this, StorePermission.AddItemToStore).IsFailure)
            {
                return Result.Fail("User has now permission to add items to store");
            }
            else
            {
                return TheStockUpdate_Add(newItem);
            }
        }

        private Result TheStockUpdate_Add(ItemInfo newItem)
        {
            if (this._inventory.GetItem(newItem).IsFailure)
            {
                return Result.Fail("Item doesn't exist in store");
            }
            else
            {
                return this._inventory.AddExistingItem(newItem.name, newItem.amount);
            }
        }

        public virtual Result UpdateStock_SubtractItems(ItemInfo newItem, User user)
        {
            if (user.HasPermission(this, StorePermission.AddItemToStore).IsFailure)
            {
                return Result.Fail("User has now permission to add items to store");
            }
            else
            {
                if (this._inventory.GetItem(newItem).IsFailure)
                {
                    return Result.Fail("Item doesn't exist in store");
                }
                else
                {
                    return this._inventory.SubtractItems(user, newItem.name, newItem.amount);
                }
            }
        }

        // TODO implement this method
        public virtual Result RemoveItemToStore(string productName, User user)
        {
            var res=this._inventory.GetItem(productName);
            if (res.IsFailure)
            {
                return res;
            }
            else
            {
                return RemoveItemToStore(res.GetValue().ShowItem(), user);
            }
        }
        
        public virtual Result RemoveItemToStore(ItemInfo newItem, User user)
        {
            if (user.HasPermission(this, StorePermission.AddItemToStore).IsFailure)
            {
                return Result.Fail("User has now permission to add items to store");
            }
            else
            {
                var itemGet = this._inventory.GetItem(newItem);
                if (itemGet.IsFailure)
                {
                    return Result.Fail("Item doesn't exist");
                }
                else
                {
                    return this._inventory.RemoveItem(user, newItem);
                }
            }
        }

        public virtual Result AppointNewOwner(User user, OwnerAppointment ownerAppointment)
        {
            if (user.HasPermission(this, StorePermission.ControlStaffPermission).IsFailure)
            {
                return Result.Fail("User doesn't have the permission to add permissions to someone else");
            }
            else
            {
                this._ownersAppointments.Add(ownerAppointment);
                return Result.Ok();
            }
        }

        public virtual Result AppointNewManager(User user, ManagerAppointment managerAppointment)
        {
            if (user.HasPermission(this, StorePermission.ControlStaffPermission).IsFailure)
            {
                return Result.Fail("User doesn't have the permission to add permissions to someone else");
            }
            else
            {
                this._managersAppointments.Add(managerAppointment);
                return Result.Ok();
            }
        }

        public virtual Result RemoveOwnerFromStore(User theOneWhoFires, User theFired, OwnerAppointment ownerAppointment)
        {
            if (theOneWhoFires.HasPermission(this, StorePermission.RemoveStoreStaff).IsSuccess)
            {
                if (this._ownersAppointments.Contains(ownerAppointment))
                {
                    this._ownersAppointments.Remove(ownerAppointment);
                    return Result.Ok();
                }
                else
                {
                    return Result.Fail("Owner appointed not record in store");
                }
            }
            else
            {
                return Result.Fail("User doesn't have permissions to remove staff");
            }
        }
        
        
        public virtual Result RemoveManagerFromStore(User theOneWhoFires, User theFired, ManagerAppointment managerAppointment)
        {
            if (theOneWhoFires.HasPermission(this, StorePermission.RemoveStoreStaff).IsSuccess)
            {
                if (this._managersAppointments.Contains(managerAppointment))
                {
                    this._managersAppointments.Remove(managerAppointment);
                    return Result.Ok();
                }
                else
                {
                    return Result.Fail("Manager appointed not record in store");
                }
            }
            else
            {
                return Result.Fail("User doesn't have permissions to remove staff");
            }
        }
        
        public virtual Result<IList<PurchaseRecord>> GetPurchaseHistory(User user)
        {
            return this._transactionHistory.GetHistory(user);
        }

        public virtual Result EnterBasketToHistory(IBasket basket)
        {
            return this._transactionHistory.AddRecordToHistory(this, basket);
        }

        public virtual string GetStoreName()
        {
            return this._storeName;
        }
        
        public virtual bool TryAddNewCartToStore(ICart cart)
        {
            bool ans=true;
            foreach (var basket in this._basketsOfThisStore)
            {
                if (basket.GetCart() == cart)
                {
                    ans = false;
                }
            }

            return ans;

        }

        public virtual Result ConnectNewBasketToStore(Basket newBasket)
        {
            if (_basketsOfThisStore.Count == 0)
            {
                this._basketsOfThisStore.Add(newBasket);
                return Result.Ok();
            }
            else
            {
                foreach (var basket in _basketsOfThisStore)
                {
                    if (basket.GetCart() == newBasket.GetCart())
                    {
                        return Result.Fail("Two baskets for the same store in cart");
                    }
                }
                this._basketsOfThisStore.Add(newBasket);
                return Result.Ok();
            }
        }

        public virtual bool CheckConnectionToCart(ICart cart)
        {
            foreach (var basket in _basketsOfThisStore)
            {
                if (basket.GetCart() == cart)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual Result<double> CheckDiscount(Basket basket)
        {
            //No double discounts
            
            double minValue = basket.GetRegularTotalPrice();
            foreach (var discount in this._myDiscountStrategies)
            {
                var checkIfDiscount = discount.CheckIfDiscount();
                if (checkIfDiscount)
                {
                    var checkDiscount = discount.Check(basket, basket.GetCart().GetUser());
                    if (checkDiscount.Count>0)
                    {
                        
                        var price = discount.Get(basket, basket.GetCart().GetUser());
                        if (price.IsFailure)
                        {
                            return price;
                        }
                        else
                        {
                            if (price.GetValue() < minValue)
                            {
                                minValue = price.GetValue();
                            }
                        }
                    }
                }
            }
            return Result.Ok(minValue);
        }

        public virtual Result CheckWithStorePolicy(IBasket basket, User user)
        {
            return this._myPurchasePolicy.CheckWithStorePolicy(basket, user);
        }


        public virtual Result AddDiscountToStore(User user,DiscountInfoNode infoNode)
        {
            if (!user.HasPermission(this, StorePermission.EditStorePolicy).IsSuccess)
            {
                return Result.Fail("User doesn't have the permission to handle store discounts and policies");
            }
            var discountRes = DiscountHandler.HandleDiscount(infoNode);
            if (discountRes.IsFailure)
            {
                return discountRes;
            }
            this._myDiscountStrategies.Add(discountRes.Value);
            return Result.Ok();
        }

        public virtual Result AddRuleToStorePolicy(User user,RuleInfoNode ruleInfoNode)
        {
            var rule = RuleHandler.HandleRule(ruleInfoNode);
            if (rule.IsFailure)
            {
                return rule;
            }
            return this._myPurchasePolicy.AddRuleToStorePolicy(user, rule.Value);
        }

        public virtual Result<IList<RuleInfoNode>> GetStorePolicy(User user)
        {
            return this._myPurchasePolicy.GetPolicy(user);
        }

        public virtual Result<IList<DiscountInfoNode>> GetStoreDiscounts(User user)
        {
            if (!user.HasPermission(this, StorePermission.EditStorePolicy).IsSuccess)
            {
                return Result.Fail<IList<DiscountInfoNode>>("User doesn't have the permission to handle store discounts and policies");
            }
            IList<DiscountInfoNode> discountInfoNodes = new List<DiscountInfoNode>();
            foreach (var discount in this._myDiscountStrategies)
            {
                if (!discount.CheckIfDiscount())
                {
                    return Result.Fail<IList<DiscountInfoNode>>("Problem with one of the discounts in store");
                }

                var res = discount.GetDisocuntInfo();
                if (res.IsFailure)
                {
                    return Result.Fail<IList<DiscountInfoNode>>(res.Error);
                }
                discountInfoNodes.Add(res.Value);
            }

            return Result.Ok(discountInfoNodes);
        }

        public virtual Result ResetStorePolicy(User user)
        {
            if (!user.HasPermission(this, StorePermission.EditStorePolicy).IsSuccess)
            {
                return Result.Fail<IList<DiscountInfoNode>>("User doesn't have the permission to handle store discounts and policies");
            }

            return this._myPurchasePolicy.Reset(user);
        }

        public virtual Result ResetStoreDiscount(User user)
        {
            if (!user.HasPermission(this, StorePermission.EditStorePolicy).IsSuccess)
            {
                return Result.Fail<IList<DiscountInfoNode>>("User doesn't have the permission to handle store discounts and policies");
            }

            this._myDiscountStrategies = new List<Composite>();
            return Result.Ok();
        }
        
        public Result ReturnItemsToStore(ItemInfo itemInfo)
        {
            return this.TheStockUpdate_Add(itemInfo);
        }

        public void FreeBasket(Basket basket)
        {
            this._basketsOfThisStore.Remove(basket);
        }

        public virtual Result<PurchaseRecord> AddBasketRecordToStore(Basket basket)
        {
            var purchaseRecord=this._transactionHistory.AddRecordToHistory(this, basket);

            foreach (var ownerAppointment in _ownersAppointments)
            {
                User owner = ownerAppointment.User;
                foreach (var item in purchaseRecord.Value.BasketInfo.ItemsInBasket)
                {
                    owner.PublishMessage(String.Format("User: {0} , bought {1} items of {2} at {3}",
                        purchaseRecord.Value.Username, item.name, item.name, purchaseRecord.Value.GetDate()));
                }
            }
            return purchaseRecord;

        }

        public virtual Result<User> GetFounder()
        {
            return Result.Ok(this._founder);
        }

        public string StoreName
        {
            get => _storeName;
        }


        public Result AskToBidOnItem(User buyer,ItemInfo itemInfo, double newPricePerUnit, int amount)
        {
            if (_UsersBids == null)
            {
                _UsersBids = new List<Pair<User, Bid>>();
            }

            itemInfo.amount = amount;
            var resGetItems = TryGetItems(itemInfo);
            if (resGetItems.IsFailure)
            {
                return resGetItems;
            }

            if (this._UsersBids.Count > 0)
            {
                Pair<User, Bid> pairToRemove = _UsersBids[0];
                bool found = false;
                foreach (var usersBid in _UsersBids)
                {
                    if (buyer == usersBid.Key || buyer.Username == usersBid.Key.Username)
                    {
                        pairToRemove = usersBid;
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    _UsersBids.Remove(pairToRemove);
                }
            }

            var newPair = new Pair<User, Bid>()
            {
                HolderId = StoreName,
                Key = buyer,
                KeyId = buyer.Username,
                Value = new Bid(buyer,itemInfo, newPricePerUnit, amount, this._ownersAppointments,_founder)
            };
            this._UsersBids.Add(newPair);
            return Result.Ok();
        }

        public Result ApproveOrDissaproveBid(User owner, string BidID, bool shouldApprove)
        {
            bool found = false;
            if (_UsersBids.Count <= 0)
            {
                return Result.Fail("No bids to approve");
            }

            Pair<User, Bid> theBid = _UsersBids[0];
            foreach (var usersBid in _UsersBids)
            {
                if (usersBid.Value.GetID().Equals(BidID))
                {
                    found = true;
                    theBid = usersBid; 
                    break;
                }
            }

            if (found)
            {
                var approveRes = theBid.Value.ApproveOrDissapproveBid(owner.Username, shouldApprove);
                if (approveRes.IsFailure)
                {
                    return approveRes;
                }
                else
                {
                    if (approveRes.Value.Equals(Bid.BidState.Approved)|| approveRes.Value.Equals(Bid.BidState.NotApproved))
                    {
                        return finishBid(theBid,approveRes.Value);
                    }
                    else
                    {
                        return Result.Ok();
                    }
                }
            }
            else
            {
                return Result.Fail("Bid not found");
            }
        }

        private Result finishBid(Pair<User, Bid> theBid, Bid.BidState approveResValue)
        {
            var removed=this._UsersBids.Remove(theBid);

            Basket theBasket;
            if (removed == false)
            {
                return Result.Fail("Problem removing bid");
            }

            if (approveResValue.Equals(Bid.BidState.Approved))
            {
                bool foundBasket = false;
                foreach (var basket in this._basketsOfThisStore)
                {
                    if(basket._cart.GetUser().Username.Equals(theBid.Key.Username))
                    {
                        var resAddToBasket=basket.AddItemAfterBid(theBid.Value.GetItemInfoAfterBidApprove());
                        if (resAddToBasket.IsFailure)
                        {
                            return resAddToBasket;
                        }
                        foundBasket = true;
                        break;
                    }
                }

                ItemInfo theItemAfterApproval = theBid.Value.GetItemInfoAfterBidApprove();
                if (!foundBasket)
                {
                    theItemAfterApproval.AssignStoreToItem(this);
                    var resAddToCart = theBid.Key.AddItemToCart(theItemAfterApproval);
                    if (resAddToCart.IsFailure)
                    {
                        return resAddToCart;
                    }
                }

                theBid.Key.PublishMessage(
                    $"Your bid for the item {theItemAfterApproval.name} with price={theItemAfterApproval.pricePerUnit} was approved and appears now in your basket");
                return Result.Ok();
            }
            else
            {
                ItemInfo theItemAfterApproval = theBid.Value.GetItemInfoAfterBidApprove();
                theBid.Key.PublishMessage(
                    $"Your bid for the item {theItemAfterApproval.name} with price={theItemAfterApproval.pricePerUnit} was approved and appears now in your basket");
                return Result.Ok();
            }
        }

        public Result<List<BidInfo>> GetAllMyWaitingBids(User user)
        {
            bool found = false;
            foreach (var ownersAppointment in this._ownersAppointments)
            {
                if (ownersAppointment.Ownername.Equals(user.Username))
                {
                    found = true;
                    break;
                }
            }

            found = found ? found : _founder.Username.Equals(user.Username);
            if (!found)
            {
                return Result.Fail<List<BidInfo>>("Not an owner or founder of this store");
            }
            
            List<BidInfo> bids = new List<BidInfo>();
            foreach (var bid in this._UsersBids)  
            {
                if (bid.Value.CheckIfShouldApprove(user))
                {
                    bids.Add(bid.Value.GetBidInfo());
                }
            }

            return Result.Ok(bids);
        }
    }

    
}