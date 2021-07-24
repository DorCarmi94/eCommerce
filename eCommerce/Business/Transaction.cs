using System.Collections.Generic;
using System.Linq;
using System.Text;
using eCommerce.Adapters;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class Transaction
    {
        private ICart _cart;
        private SupplyProxy _supply;
        private PaymentProxy _payment;

        public Transaction(ICart cart)
        {
            this._cart = cart;
            this._payment = new PaymentProxy();
            this._supply = new SupplyProxy();
        }

        public Result BuyWholeCart(PaymentInfo paymentInfo)
        {
            int paymentTransactionId;
            int supplyTransactionId;
            //Check with store policy
            foreach (var basket in this._cart.GetBaskets())
            {
                var res = basket.CheckWithStorePolicy();
                if (res.IsFailure)
                {
                    return Result.Fail("<Policy>"+res.Error);
                    
                }
            }
            
            //Calculate prices for each basket
            foreach (var basket in this._cart.GetBaskets())
            {
                var res=basket.CalculateBasketPrices();
                if (res.IsFailure)
                {
                    return Result.Fail("<CalculateBaskets>"+res.Error);
                }
            }
            //Finish buying items
            //$$$$$$$$$$$
            //Change 1: items stock
            //$$$$$$$$$$$
            foreach (var basket in this._cart.GetBaskets())
            {
                var res = basket.BuyWholeBasket();
                if (res.IsFailure)
                {
                    return Result.Fail("<GetBaskets>"+res.Error);
                }
            }

            double totalPriceForAllBaskets = 0;
            foreach (var basket in this._cart.GetBaskets())
            {
                totalPriceForAllBaskets += basket.GetTotalPrice().GetValue();
            }

            if (totalPriceForAllBaskets <= 0)
            {
                //retrieve ItemsToStock
                return Result.Fail("<TotalPrice>Problem with calculating prices: can't charge negative price");
            }
            
            
            //$$$$$$$$$$$
            //Change 2: Pay
            //$$$$$$$$$$$
            var payTask=this._payment.Charge(totalPriceForAllBaskets, paymentInfo.UserName, paymentInfo.IdNumber,
                paymentInfo.CreditCardNumber, paymentInfo.CreditCardExpirationDate,
                paymentInfo.ThreeDigitsOnBackOfCard);
            payTask.Wait();
            if (!payTask.Result.IsSuccess)
            {
                foreach (var basket in _cart.GetBaskets())
                {
                    basket.ReturnAllItemsToStore();
                }
                return Result.Fail("<Payment>Payment process didn't succeed");
            }

            paymentTransactionId = payTask.Result.Value;

            foreach (var basket in this._cart.GetBaskets())
            {
                List<string> itemNames = new List<string>();
                foreach (var item in basket.GetAllItems().GetValue())
                {
                    itemNames.Add(item.name);
                }

                var supply =
                    _supply.SupplyProducts(basket.GetStoreName(), itemNames.ToArray(), paymentInfo.FullAddress);
                var x = supply.Result;
                if (!supply.Result.IsSuccess)
                {
                    this._payment.Refund(paymentTransactionId);
                    foreach (var basket1 in _cart.GetBaskets())
                    {
                        basket.ReturnAllItemsToStore();
                    }
                    return Result.Fail("<Supply>Supply info found incorrect by supply system");
                }
            }
            //TODO: save in history transcation id
            

            foreach (var basket in _cart.GetBaskets())
            {
                var res=basket.AddBasketRecords();
                if (res.IsFailure)
                {
                    return res;
                }
            }

            this._cart.GetUser().FreeCart();

            return Result.Ok();
            
            
            
        }

        
    }
}