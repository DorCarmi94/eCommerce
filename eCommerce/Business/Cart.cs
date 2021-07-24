using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using eCommerce.Business.Repositories;
using eCommerce.Common;
using eCommerce.DataLayer;

namespace eCommerce.Business
{
    public class Cart : ICart
    {

        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string CardID { get; set; }
        
        public User _cartHolder { get; set; }
        
        public Transaction _performTransaction;
        
        
        public List<Pair<Store, Basket>> _baskets { get; set; }
        
        
        private double _totalPrice;

        public Cart()
        {
            _baskets = new List<Pair<Store, Basket>>();
            _totalPrice = 0;
        }

        public Cart(User user)
        {
            this._cartHolder = user;
            _baskets = new List<Pair<Store, Basket>>();
            _totalPrice = 0;
            // CartGuid = Guid.NewGuid();
            CardID = user.Username;
        }

        public bool CheckForCartHolder(User user)
        {
            return this._cartHolder == user;
        }

        public Result AddItemToCart(User user,ItemInfo item)
        {
            if (user == this._cartHolder)
            {
                var storeRes = item.GetStore();
                if (storeRes.IsFailure)
                {
                    return Result.Fail(storeRes.Error);
                }
                else
                {
                    if (!ListHelper<Store,Basket>.ContainsKey(_baskets,storeRes.Value))
                    {
                        if (storeRes.Value.TryAddNewCartToStore(this))
                        {
                            var newBasket = new Basket(this, storeRes.Value);
                            var basketRes=storeRes.Value.ConnectNewBasketToStore(newBasket);
                            
                            if (basketRes.IsFailure)
                            {
                                return Result.Fail(basketRes.Error);
                            }
                            else
                            {
                                ListHelper<Store,Basket>.Add(_baskets,this.CardID,storeRes.Value,storeRes.Value._storeName,newBasket);
                            }
                        }
                        else
                        {
                            return Result.Fail("Problem- multiple connection of the same cart to the same store");
                        }

                    }

                    return  ListHelper<Store,Basket>.KeyToValue(_baskets,storeRes.Value).AddItemToBasket(user, item);
                }

            }
            else
            {
                return Result.Fail("Only cart holder can edit cart");
            }
        }

        public Result EditCartItem(User user, ItemInfo item)
        {
            if (user == this._cartHolder)
            {
                var storeRes = item.GetStore();
                if (storeRes.IsFailure)
                {
                    return Result.Fail(storeRes.Error);
                }
                else
                {
                    if (ListHelper<Store,Basket>.ContainsKey(_baskets,storeRes.Value))
                    {
                        return  ListHelper<Store,Basket>.KeyToValue(_baskets,storeRes.Value).EditItemInBasket(user, item);
                    }
                    else
                    {
                        return Result.Fail("No basket exists for the store the item is from");
                    }
                }
            }
            else
            {
                return Result.Fail("Only cart holder can edit cart");
            }
        }


        public Result<double> CalculatePricesForCart()
        {
            this._totalPrice = 0;
            foreach (var storeBasket in _baskets)
            {
                var currAns = storeBasket.Key.CalculateBasketPrices(storeBasket.Value);
                if (currAns.IsFailure)
                {
                    return Result.Fail<double>(currAns.Error);
                }

                double currBasketPrice = storeBasket.Value.GetTotalPrice().GetValue();
                this._totalPrice += currBasketPrice;
            }
            return Result.Ok(_totalPrice);
        }

        public double GetCurrentCartPrice()
        {
            return this._totalPrice;
        }
        

        public Result BuyWholeCart(User user, PaymentInfo paymentInfo)
        {
            this._performTransaction = new Transaction(this);
            var result=_performTransaction.BuyWholeCart(paymentInfo);
            return result;
        }
        public List<Basket> GetBaskets()
        {
            return ListHelper<Store,Basket>.Values(_baskets);
        }

        public CartInfo GetCartInfo()
        {
            IList<BasketInfo> baskets = new List<BasketInfo>();
            foreach (var di_basket in _baskets)
            {
                baskets.Add(di_basket.Value.GetBasketInfo());
            }

            CalculatePricesForCart();
            CartInfo info = new CartInfo(baskets, this._totalPrice);
            return info;
        }

        public User GetUser()
        {
            return this._cartHolder;
        }

        public List<ItemInfo> GetAllItems()
        {
            List<ItemInfo> allItems = new List<ItemInfo>();
            foreach (var basket in _baskets)
            {
                allItems.AddRange(basket.Value.GetAllItems().Value);
            }

            return allItems;
        }

        public void Free()
        {
            var oldBaskets = new Queue<Pair<Store, Basket>>(_baskets);
            _baskets = new List<Pair<Store, Basket>>();
            
            while(oldBaskets.Count > 0)
            {
                var pair  = oldBaskets.Dequeue();
                pair.Value.Free();
                var basket = pair.Value;
                pair.Value = null;
                // DataFacade.Instance.RemoveEntity(basket);
                // DataFacade.Instance.RemoveEntity(pair);
            }

            _totalPrice = 0;
            _performTransaction = null;
            // this._baskets.Clear();
        }
    }
}