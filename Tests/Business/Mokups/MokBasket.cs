using eCommerce.Business;


namespace Tests.Business.Mokups
{
    public class MokBasket : Basket
    {
        public MokBasket(Cart cart, Store store) : base(cart, store)
        {
        }
    }
}