using System.Threading.Tasks;
using eCommerce.Adapters;
using eCommerce.Common;

namespace Tests.Service
{
    public class mokPaymentService: IPaymentAdapter
    {
        private bool chargeAns;
        private bool checkAns;
        private bool refundAns;
        public mokPaymentService(bool chargeAnswer, bool checkAnswer, bool refundAns)
        {
            this.chargeAns = chargeAnswer;
            this.checkAns = checkAnswer;
            this.refundAns = refundAns;
        }

        public async Task<Result<int>> Charge(double price, string paymentInfoUserName, string paymentInfoIdNumber, string paymentInfoCreditCardNumber,
            string paymentInfoCreditCardExpirationDate, string paymentInfoThreeDigitsOnBackOfCard)
        {
            await Task.Delay(2000);
            if (chargeAns)
            {
                //TODO may generate id
                return Result.Ok(100000);
            }
            else
            {
                return Result.Fail<int>("Charge problem");
            }
        }

        public Task<bool> CheckPaymentInfo(string paymentInfoUserName, string paymentInfoIdNumber, string paymentInfoCreditCardNumber,
            string paymentInfoCreditCardExpirationDate, string paymentInfoThreeDigitsOnBackOfCard)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Result> Refund(int transactionId)
        {
            await Task.Delay(2000);
            return Result.Ok();
        }
    }
}