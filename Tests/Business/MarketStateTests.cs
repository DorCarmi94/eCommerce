using System;
using System.Threading;
using System.Threading.Tasks;
using eCommerce.Business;
using NUnit.Framework;

namespace Tests.Business
{
    [TestFixture]
    public class MarketStateTests
    {
        [Test]
        public void InitialValidTest()
        {
            MarketState marketState = MarketState.GetInstance();
            Assert.True(marketState.ValidState, "Init state value should be valid");
        }
        
        [Test]
        public async Task SetErrAndThenMakeValidTest()
        {
            MarketState marketState = MarketState.GetInstance();
            ServiceChecker serviceChecker = new ServiceChecker();
            
            marketState.SetErrorState("CHECKING MarketState", serviceChecker.CheckSerivce);
            Assert.False(marketState.ValidState, "State value should be invalid after set error");
            
            // wait for serviceChecker to be available 
            Thread.Sleep(TimeSpan.FromSeconds(12));
            
            Assert.True(marketState.ValidState, "The state should has been changed to be valid");
        }
    }

    public class ServiceChecker
    {
        private static int VALID_AT = 1;

        private int iterNumber;
        public ServiceChecker()
        {
            iterNumber = 0;
        }

        public bool CheckSerivce()
        {
            if (iterNumber == VALID_AT)
            {
                return true;
            }

            iterNumber++;
            return false;
        }
    }
}