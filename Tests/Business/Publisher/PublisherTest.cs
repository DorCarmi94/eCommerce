using System.Diagnostics;
using eCommerce.Publisher;
using NUnit.Framework;
using Tests.Business.Mokups;

namespace Tests.Business.Publisher
{
    public class PublisherTest
    {
        private MainPublisher _mainPublisher;
        private UserObserver _messageListener;
        
        

        public PublisherTest()
        {
            _mainPublisher = MainPublisher.Instance;
            _messageListener = new MokMessageHub();
            _mainPublisher.Register(_messageListener);
            
            
        }
        
        [SetUp] 
        
        public void Setup()
        {
            
            
        }

        [Test]
        public void Test1()
        {
            string USER_ID = "123";
            _mainPublisher.Connect(USER_ID);
            _mainPublisher.AddMessageToUser(USER_ID,"Hello how are you?");
            
        }
        
        [TearDown]
        public void TearDown()
        {
            
        }
    }
}