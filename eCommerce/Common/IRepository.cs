using System.Diagnostics.CodeAnalysis;
using eCommerce.Business;

namespace eCommerce.Common
{
    public interface IRepository<T>
    {
        /// <summary>
        /// Add the data to the repository if not exists
        /// </summary>
        /// <param name="data">The data</param>
        /// <returns>True if the data has been added</returns>
        public bool Add([NotNull] T data);

        /// <summary>
        /// GetDiscount the data from the repository by the id
        /// </summary>
        /// <param name="id">The id of the required data</param>
        /// <returns>The data if exists else null</returns>
        public T GetOrNull([NotNull] string id);
        
        /// <summary>
        /// Remove a user
        /// </summary>
        /// <param name="id">The user id</param>
        public void Remove([NotNull] string id);

        void Update(T data);
    }
}