using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eCommerce.Common;
using eCommerce.DataLayer;

namespace eCommerce.Business
{
    public class ItemsInventory
    {
        [NotMapped] //TODO is this field needed
        public List<Item> _aquiredItems { get; private set; }

        [Key] [ForeignKey("Store")]
        public string StoreId { get; private set; }
        public Store _belongsToStore { get; private set; }
        public List<Item> _itemsInStore { get; private set; }
        
        // for ef
        public ItemsInventory()
        {
            this._aquiredItems = new List<Item>();
        }

        public ItemsInventory(Store store)
        {
            this._belongsToStore = store;
            this._itemsInStore = new List<Item>();
            this._aquiredItems = new List<Item>();
        }

        public ItemsInventory(List<Item> itemsInStore, Store belongsToStore)
        {
            _itemsInStore = itemsInStore;
            _belongsToStore = belongsToStore;
            
        }

        //Searches

        public List<Item> SearchItem(string stringSearch)
        {
            List<Item> items = new List<Item>();
            foreach (var item in this._itemsInStore)
            {
                if (item.CheckForResemblance(stringSearch))
                {
                    items.Add(item);
                }
            }

            return items;
        }
        
        public List<Item> SearchItemWithPriceFilter(string stringSearch, int startPrice, int endPrice)
        {
            List<Item> items = new List<Item>();
            foreach (var item in this._itemsInStore)
            {
                if (item.CheckForResemblance(stringSearch)
                    &&item.CheckPricesInBetween(startPrice,endPrice).GetValue())
                {
                    items.Add(item);
                }
            }

            return items;
        }
        
        public List<Item> SearchItemWithCategoryFilter(string stringSearch, string category)
        {
            List<Item> items = new List<Item>();
            foreach (var item in this._itemsInStore)
            {
                if (item.CheckForResemblance(stringSearch)
                    && item.GetCategory().getName().Equals(category)) 
                {
                    items.Add(item);
                }
            }
            return items;
        }


        public List<Item> GetAllItemsInStore()
        {
            return this._itemsInStore;
        }

        public Result AddNewItem(User user, ItemInfo itemInfo)
        {
            var ans = user.HasPermission(this._belongsToStore, StorePermission.AddItemToStore);
            if(!ans.IsFailure)
            {
                if (HasItem(itemInfo.name))
                {
                    return Result.Fail("Item already exist in store");
                } else if(itemInfo.pricePerUnit <= 0)
                {
                    return Result.Fail("Price need to be positive");
                } else if (itemInfo.amount < 0)
                {
                    return Result.Fail("Amount need to be not negative");
                }
                else
                {
                    var newItem = new Item(itemInfo, _belongsToStore);

                    this._itemsInStore.Add(newItem);
                    return Result.Ok();
                }
            }
            else
            {
                return Result.Fail("User doesn't have permission to add item to store");
            }
        }


        public Result AddExistingItem(string itemName, int amount)
        {
            
            Item item = GetItemOrNull(itemName);
            if (item != null)
            {
                return item.AddItems(amount);
            }
            else
            {
                return Result.Fail("Item doesn't exist in store");
            }
                
        }


        public Result<ItemInfo> GetItems(string itemName, int amount)
        {
            Item item = GetItemOrNull(itemName);
            if (item != null)
            {
                if (item.GetAmount() - amount <= 1)
                {
                    return Result.Fail<ItemInfo>("Amount requested will leave store with under limit amount of items");
                }
                else
                {
                    return item.GetItems(amount);    
                }
            
                
            }
            else
            {
                return Result.Fail<ItemInfo>("Item doesn't exist in store");
            }
        }

        public Result FinalizeGetItems(string itemName, int amount)
        {
            Item item = GetItemOrNull(itemName);
            if (item != null)
            {
                if (item.GetAmount() - amount <= 1)
                {
                    return Result.Fail<ItemInfo>("Amount requested will leave store with under limit amount of items");
                }
                else
                {
                    return item.FinalizeGetItems(amount);    
                }
            }
            else
            {
                return Result.Fail<ItemInfo>("Item doesn't exist in store");
            }
        }

        public Result<Item> GetItem(ItemInfo item)
        {
            foreach (var curItem in _itemsInStore)
            {
                if (curItem.GetName().Equals(item.name))
                {
                    return Result.Ok(curItem);
                }
            }
            return Result.Fail<Item>("Couldn't find item in store's inventory");
        }
        
        public Result TryGetItems(ItemInfo item)
        {
            foreach (var curItem in _itemsInStore)
            {
                if (curItem.GetName().Equals(item.name))
                {
                    if (curItem.GetAmount() - item.amount < 1)
                    {
                        return Result.Fail("Not enough items in store");
                    }
                    else
                    {
                        return Result.Ok();
                    }
                }
            }

            return Result.Fail("Couldn't find item in store's inventory");
        }
        
        public Result<Item> GetItem(string itemID)
        {
            Item item = GetItemOrNull(itemID);
            if (item != null) 
            {
                return Result.Ok(item);
            }
            else
            {
                return Result.Fail<Item>("Item doens't exist in store's inventory");
            }
        }

        

        public Result RemoveItem(User user, ItemInfo newItem)
        {
            
            if (!user.HasPermission(_belongsToStore, StorePermission.AddItemToStore).IsFailure)
            {
                Item item = GetItemOrNull(newItem.name);
                if (item != null)
                {
                    this._itemsInStore.Remove(item);
                    // DataFacade.Instance.RemoveEntity(item);
                    return Result.Ok();
                }
                else
                {
                    return Result.Fail("Item doesn't exist in store");
                }
            }
            else
            {
                return Result.Fail("User doesn't have permission to add item to store");
            }
        }

        public Result SubtractItems(User user, string newItemName, int newItemAmount)
        {
            if (!user.HasPermission(_belongsToStore, StorePermission.AddItemToStore).IsFailure)
            {
                Item item = GetItemOrNull(newItemName);
                if (item != null)
                {
                    return item.SubtractItems(user, newItemAmount);
                }
                else
                {
                    return Result.Fail("Item doesn't exist in store");
                }
            }
            else
            {
                return Result.Fail("User doesn't have permission to add item to store");
            }
        }

        private bool HasItem(string itemName)
        {
            return GetItemOrNull(itemName) != null;
        }

        private Item GetItemOrNull(string itemName)
        {
            return _itemsInStore.Find(item => item._name.Equals(itemName));
        }
    }
}