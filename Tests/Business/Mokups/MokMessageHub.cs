using System;
using System.Collections.Concurrent;
using eCommerce.Publisher;

namespace Tests.Business.Mokups
{
    public class MokMessageHub : UserObserver
    {
        private bool taken = false;
        public void Notify(string userName, ConcurrentQueue<string> message)
        {
            taken = true;
            while (!message.IsEmpty)
            {
                string currMes;
                message.TryDequeue(out currMes);
                Console.WriteLine(currMes);  
            }
        }
        
        public bool isTake =>  taken;
    }
}