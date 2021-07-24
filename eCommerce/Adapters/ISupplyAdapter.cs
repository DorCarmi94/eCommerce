using System.Threading.Tasks;
using eCommerce.Common;

namespace eCommerce.Adapters
{
    public interface ISupplyAdapter
    {

        public Task<Result<int>> SupplyProducts(string storeName, string[] itemsNames, string userAddress);
        public Task<Result> CheckSupplyInfo(int transactionId);

    }
}