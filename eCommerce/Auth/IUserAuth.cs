using System.Threading.Tasks;
using eCommerce.Common;

namespace eCommerce.Auth
{
    public interface IUserAuth
    {

        /// <summary>
        /// Init the Auth, must be called before use.
        /// </summary>
        /// <param name="config"></param>
        public void Init(AppConfig config);
        
        /// <summary>
        /// Register a new user to the system as a member.
        /// </summary>
        /// <param name="username">The user name</param>
        /// <param name="password">The user password</param>
        /// <returns>Successful Result if the user has been successfully registered</returns>
        public Task<Result> Register(string username, string password);
        
        /// <summary>
        /// Authenticate the username password.
        /// The user need to be registered.
        /// </summary>
        /// <param name="username">The user name</param>
        /// <param name="password">The user password</param>
        /// <returns>The result of the authentication</returns>
        public Task<Result> Authenticate(string username, string password);

        /// <summary>
        /// Generate an access token for the user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GenerateToken(string username);

        /// <summary>
        /// Check if the token is valid
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>True if the token is valid</returns>
        public bool IsValidToken(string token);
        
        /// <summary>
        /// GetDiscount the Auth data if valid
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>Authorization data</returns>
        public Result<AuthData> GetData(string token);

    }
}