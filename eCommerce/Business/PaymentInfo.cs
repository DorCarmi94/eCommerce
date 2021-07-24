namespace eCommerce.Business
{
    public class PaymentInfo
    {
        public string UserName { get; set; }
        public string IdNumber { get; set; }
        public string CreditCardNumber { get; set; }
        public string CreditCardExpirationDate { get; set; }
        public string ThreeDigitsOnBackOfCard { get; set; }
        public string FullAddress { get; set; }

        public PaymentInfo(string userName, string idNumber, string creditCardNumber, string creditCardExpirationDate,
            string threeDigitsOnBackOfCard, string fullAddress)
        {
            UserName = userName;
            IdNumber = idNumber;
            CreditCardNumber = creditCardNumber;
            CreditCardExpirationDate = creditCardExpirationDate;
            ThreeDigitsOnBackOfCard = threeDigitsOnBackOfCard;
            FullAddress = fullAddress;
        }

        public override string ToString()
        {
            return $"Username: {UserName}\nId number: {IdNumber}\nCredit card: {CreditCardNumber}\n" +
                   $"Card expiration date: {CreditCardExpirationDate}\nSecurity digits:{ThreeDigitsOnBackOfCard}\n";
        }
    }
}