using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class Item
    {
        public String _name { get; private set; }
        public int _amount { get; private set; }
        [ForeignKey("Store")]
        public string StoreId { get; private set; }
        public Store _belongsToStore { get; private set; }
        public Category _category { get; private set; }
        public List<String> _keyWords { get; private set; }
        [NotMapped]
        public PurchaseStrategy _purchaseStrategy { get; private set; }
        public double _pricePerUnit { get; private set; }

        public double GetTotalPrice()
        {
            return _pricePerUnit * _amount;
        }

        public double GetPricePerUnit()
        {
            return this._pricePerUnit;
        }

        public Item()
        {
            _keyWords = new List<string>();
        }

        public Item(string name, int amount, Store belongsToStore, Category category, List<string> keyWords, double pricePerUnit)
        {
            _name = name;
            _amount = amount;
            _belongsToStore = belongsToStore;
            StoreId = _belongsToStore.StoreName;
            _category = category;
            _keyWords = keyWords;
            _pricePerUnit = pricePerUnit;
        }
        
        public Item(String name, Category category, Store store, int pricePer)
        {
            this._name = name;
            this._category = category;
            this._belongsToStore = store;
            StoreId = _belongsToStore.StoreName;
            _amount = 1;
            this._keyWords=new List<string>();
            this._pricePerUnit = pricePer;
            _purchaseStrategy = new DefaultPurchaseStrategy(_belongsToStore);
        }

        public Item(ItemInfo info, Store store)
        {
            this._name = info.name;
            this._amount = info.amount;
            this._category = new Category(info.category);
            this._belongsToStore = store;
            StoreId = _belongsToStore.StoreName;
            CopyKeyWords(info.keyWords);
            this._purchaseStrategy = new DefaultPurchaseStrategy(_belongsToStore);
            this._pricePerUnit = info.pricePerUnit;
            this._belongsToStore = store;
        }

        private void CopyKeyWords(IList<string> words)
        {
            this._keyWords = new List<string>();
            foreach (var ketWord in words)
            {
                _keyWords.Add(ketWord);
            }
        }

        public Result SetPrice(User user,int pricePerUnit)
        {
            if (!user.HasPermission(_belongsToStore, StorePermission.ChangeItemPrice).IsFailure)
            {
                if (pricePerUnit > 0)
                {
                    this._pricePerUnit = pricePerUnit;
                    return Result.Ok();
                }
                else
                {
                    return Result.Fail("Bad new price input");
                }
            }
            else
            {
                return Result.Ok("User doesn't have the permission for this store");
            }
        }

        public Store GetStore()
        {
            return this._belongsToStore;
        }

        public String GetName()
        {
            return this._name;
        }

        public int GetAmount()
        {
            return _amount;
        }

        public Category GetCategory()
        {
            return _category;
        }

        public Result EditCategory(User user,Category category)
        {
            if (user.HasPermission(this._belongsToStore, StorePermission.EditItemDetails).IsSuccess)
            {
                this._category = category;
                return Result.Ok();
            }
            else
            {
                return Result.Fail("User doesn't have permission to edit item");
            }
        }
        
        public Result AddKeyWord(User user,String keyWord)
        {
            
            if (!user.HasPermission(this._belongsToStore, StorePermission.EditItemDetails).IsFailure)
            {
                if (keyWord != null && keyWord.Length > 0)
                {
                    this._keyWords.Add(keyWord);
                    return Result.Ok();
                }
                else
                {
                    return Result.Fail("Bad input of key word");
                }
            }
            else
            {
                return Result.Fail("User doesn't have permission to edit item");
            }
        }

        public Result<bool> CheckPricesInBetween(double startPrice, double endPrice)
        {
            if (startPrice > 0 && startPrice <= endPrice)
            {
                if (this._pricePerUnit >= startPrice && this._pricePerUnit <= endPrice)
                {
                    return Result<bool>.Ok(true);
                }
                else
                {
                    return Result<bool>.Ok(false);
                }
            }
            else
            {
                return Result.Fail<bool>("Bad input- start or end price");
            }
        }


        public bool CheckForResemblance(string searchString)
        {
            if (this._name.Contains(searchString))
            {
                return true;
            }
            else if (_category.getName().Contains(searchString))
            {
                return true;
            }
            else if(this._keyWords.Contains(searchString))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ItemInfo ShowItem()
        {
            return GetItemInfo(this._amount);
        }

        public Result<bool> CheckItemAvailability(int amount)
        {
            if (amount <= 0)
            {
                return Result.Fail<bool>("Bad amount input");
            }
            else if (this._amount - amount <= 1)
            {
                return Result.Ok<bool>(false);
            }
            else
            {
                return Result.Ok<bool>(true);
            }
        }

        private ItemInfo GetItemInfo(int amount)
        {
            var info=new ItemInfo(amount, this._name, this._belongsToStore.GetStoreName(), this._category.getName(),
                this._pricePerUnit,this._keyWords,this);
            info.AssignStoreToItem(this._belongsToStore);
            return info;
        }

        public Result<ItemInfo> AquireItems(ItemInfo itemInfo)
        {
            return this._purchaseStrategy.AquireItems(itemInfo);
        }

        public Result<ItemInfo> GetItems(int amount)
        {
            //Use to get items to put in basket
            if (this._amount - amount <= 1)
            {
                return Result.Fail<ItemInfo>("There are no enough items to answer the requested amount");
            }
            else
            {
                
                return Result.Ok<ItemInfo>(GetItemInfo(amount));
            }
        }

        public Result FinalizeGetItems(int amount)
        {
            if (this._amount - amount < 1)
            {
                return Result.Fail("There are no enough items to answer the requested amount");
            }
            else
            {
                this._amount -= amount;
                return Result.Ok();
            }
        }

        public Result AddItems(int amount)
        {
            if (amount > 0)
            {
                this._amount += amount;
                return Result.Ok();
            }
            else
            {
                return Result.Fail("Bad input");
            }
        }
        
        
        public Result SubtractItems(User user,int amount)
        {
            if (this._amount-amount >= 1)
            {
                this._amount -= amount;
                return Result.Ok();
            }
            else
            {
                return Result.Fail("Bad input- amount can't stay lower than 1");
            }
        }

        

        public Result EditItem(ItemInfo newItem)
        {
            if (!newItem.name.Equals(this._name))
            {
                return Result.Fail("Item info is not for the same item");
            }

            if (!newItem.storeName.Equals(this._belongsToStore.GetStoreName()))
            {
                return Result.Fail("Item info is not about the same store");
            }

            if (newItem.amount < 0)
            {
                return Result.Fail("Item in store can't be negative");
            }

            if (newItem.pricePerUnit <= 0)
            {
                return Result.Fail("Item price can't be lower or equal to zero");
            }
            this._amount=newItem.amount;
            this._category = new Category(newItem.category);
            CopyKeyWords(newItem.keyWords);
            this._pricePerUnit = newItem.pricePerUnit;
            return Result.Ok();
        }
    }
}