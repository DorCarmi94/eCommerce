namespace eCommerce.Business
{
    public class UserBasicInfo
    {
        public string Username { get; set; }
        public bool IsLoggedIn { get; set; }

        public UserRole UserRole { get; set; }

        public UserBasicInfo(string username, bool isLoggedIn, UserRole userRole)
        {
            Username = username;
            IsLoggedIn = isLoggedIn;
            UserRole = userRole;
        }
    }
}