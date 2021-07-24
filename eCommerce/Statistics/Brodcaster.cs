namespace eCommerce.Statistics
{
    public interface Brodcaster
    {
        public void Register(Reciver reciver);

        public void UnRegister(Reciver reciver);
        
        public void NotifyAll(string userType, int number);
    }
    
    public interface Reciver
    {
        public void ReciveBrodcast(string userType, int number);
    }
}