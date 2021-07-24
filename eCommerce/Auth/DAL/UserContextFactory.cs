namespace eCommerce.Auth.DAL
{
    public class UserContextFactory
    {
        public UserContext Create()
        {
            return new UserContext();
        }
    }
}