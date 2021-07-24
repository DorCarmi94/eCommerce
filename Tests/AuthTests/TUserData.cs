namespace Tests.AuthTests
{
    public class TUserData
    {
        public string Username { get; }
        public string Password { get; }

        public TUserData(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}