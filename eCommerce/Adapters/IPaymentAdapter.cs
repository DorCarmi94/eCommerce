using System.Threading.Tasks;
using eCommerce.Business;
using eCommerce.Common;

namespace eCommerce.Adapters
{
    public interface IPaymentAdapter
    {
        public Task<Result<int>> Charge(double price, string paymentInfoUserName, string paymentInfoIdNumber, 
            string paymentInfoCreditCardNumber, string paymentInfoCreditCardExpirationDate, string paymentInfoThreeDigitsOnBackOfCard);

        public Task<bool> CheckPaymentInfo(string paymentInfoUserName, string paymentInfoIdNumber,
            string paymentInfoCreditCardNumber, string paymentInfoCreditCardExpirationDate,
            string paymentInfoThreeDigitsOnBackOfCard);
        
        public Task<Result> Refund(int transactionId);
    }
}