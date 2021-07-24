namespace eCommerce.Auth
{
    public class AuthData
    {
        public string Username { get; set; }
        
        public AuthData(string username)
        {
            Username = username;
        }

        public bool AllDataIsNotNull()
        {
            return Username != null;
        }
    }
}