namespace eCommerce.Publisher
{
    public interface PublisherObservable
    {
        public void Register(UserObserver userObserver);
        public void NotifyAll(string userid);
        
    }
}