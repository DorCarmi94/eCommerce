using System.Threading.Tasks;

namespace eCommerce.Auth
{
    public interface IRegisteredUserRepo
    {
        /// <summary>
        /// Add a user to the repository if not already exists
        /// </summary>
        /// <param name="authUser">The user</param>
        /// <returns>True if the user have been added</returns>
        public Task<bool> Add(AuthUser authUser);

        /// <summary>
        /// GetDiscount the user
        /// </summary>
        /// <param name="username">The user name</param>
        /// <returns>Return a User of exists</returns>
        public Task<AuthUser> GetUserOrNull(string username);
    }
}