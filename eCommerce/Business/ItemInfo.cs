using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using eCommerce.Common;
using eCommerce.Service;

namespace eCommerce.Business
{
    public class ItemInfo : IItem
    {
        [NotMapped]
        public int amount { get; set; }
        [NotMapped]
        public string name { get; set; }
        [NotMapped]
        public string storeName { get; set; }
        [NotMapped]
        public string category { get; set; }
        
        [NotMapped]
        public List<string> keyWords;

       
        [NotMapped]
        public double pricePerUnit { get; private set; }
        
        public Item theItem { get; private set; }
        public Store _store { get; private set; }

        public static ItemInfo AnyItem(string storeName) =>
            new ItemInfo(0, "ANY", storeName, "ALL", new List<string>(), 0);

        public ItemInfo()
        {
            this.keyWords = new List<string>();
        }
        public ItemInfo(int amount, string name, string storeName, string category,
            double pricePerUnit, List<string> keyWords,Item theItem)
        {
            this.amount = amount;
            this.name = name;
            this.storeName = storeName;
            this.category = category;
            this.keyWords = new List<string>();
            this.pricePerUnit = pricePerUnit;
            foreach (var word in keyWords)
            {
                if (word == null)
                {
                    throw new ArgumentException("Bad key word- null");
                }
                else
                {
                    this.keyWords.Add(String.Copy(word));
                }
                
            }

            this.theItem = theItem;
        }
        
        public ItemInfo(int amount, string name, string storeName, 
            string category, List<string> keyWords, double pricePerUnit)
        {
            this.amount = amount;
            this.name = name;
            this.storeName = storeName;
            this.category = category;
            this.keyWords = new List<string>();
            this.pricePerUnit = pricePerUnit;
            foreach (var word in keyWords)
            {
                if (word == null)
                {
                    throw new ArgumentException("Bad key word- null");
                }
                else
                {
                    this.keyWords.Add(String.Copy(word));
                }
                
            }
            this.theItem = null;
        }

        public ItemInfo(ItemInfo itemInf)
        {
            this.amount = itemInf.amount;
            this.category = itemInf.category;
            this.name = itemInf.name;
            this.keyWords = new List<string>();
            foreach (var keyWord in itemInf.keyWords)
            {
                this.keyWords.Add(keyWord);
            }

            this.storeName = itemInf.storeName;
            this.theItem = itemInf.theItem;
            this.pricePerUnit = itemInf.pricePerUnit;
        }

        public Result SetItemToStore(Store store)
        {
            if (store != null)
            {
                this._store = store;
                return Result.Ok();
            }
            else
            {
                return Result.Fail("Problem assigning store");
            }
        }
        

        public Result<Store> GetStore()
        {
            if (this._store == null)
            {
                return Result.Fail<Store>("No store assigned to item");
            }
            else
            {
                return Result.Ok(this._store);
            }
            
        }

        public Result AssignStoreToItem(Store store)
        {
            if (store.GetStoreName().Equals(this.storeName))
            {
                this._store = store;
                return Result.Ok();
            }
            else
            {
                return Result.Fail("Store assigned doesn't match item's store name");
            }
        }
        

        // ========== Properties ========== //
        
        // TODO check how to set store and other properties
        // that dont have setters
        
        [Key]
        public Guid ItemID { get; set; }
        
        public string ItemName { get => name;
            private set { this.name = value; }
        }
        public string StoreName { get => storeName;
            private set { this.storeName = value; }
        }

        public int Amount
        {
            get => amount;
            set => this.amount = value;
        }

        public string Category
        {
            get => category;
            set => this.category = value;
        }

        [NotMapped]
        public List<string> KeyWords
        {
            get => keyWords;
            set => keyWords = value;
        }
        public double PricePerUnit
        {
            get => pricePerUnit;
            set => this.pricePerUnit = value;
        }
        public string keyWordsString
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var keyWord in keyWords)
                {
                    stringBuilder.Append(keyWord + ";");
                }

                return stringBuilder.ToString();
            }
            set
            {
                string[] arr = value.Split(';',StringSplitOptions.RemoveEmptyEntries);
                this.keyWords=arr.ToList();
            }
        }
    }
}