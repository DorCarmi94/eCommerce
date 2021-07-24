using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using eCommerce.Common;

namespace eCommerce.Adapters
{
    public class SupplyProxy : ISupplyAdapter
    {
        public static ISupplyAdapter _adapter;
        
        public static int REAL_HITS = 0;
        public static int PROXY_HITS = 0;
        
        private static int _transactionId;

        public static void AssignSupplyService(ISupplyAdapter supplyAdapter)
        {
            _adapter = supplyAdapter;
        }

        public SupplyProxy()
        {
            _transactionId = 10000;
        }

        public async Task<Result<int>> SupplyProducts(string storeName, string[] itemsNames, string userAddress)
        {
            if (_adapter == null)
            {
                int transactionId = _transactionId;
                transactionId++;
                await Task.Delay(30);
                PROXY_HITS++;
                return Result.Ok(transactionId);
            }
            var ans= await _adapter.SupplyProducts(storeName, itemsNames, userAddress);
            if (ans.IsSuccess)
            {
                REAL_HITS++;
            }

            return ans;

        }

        public async Task<Result> CheckSupplyInfo(int transactionId)
        {
            if (_adapter == null)
            {
                await Task.Delay(30);
                return Result.Ok();
            }

            return await _adapter.CheckSupplyInfo(transactionId);
        }
    }
}