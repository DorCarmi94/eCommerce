using System.Collections.Concurrent;
using System.Collections.Generic;

namespace eCommerce.Publisher
{
    public interface UserObserver
    {
        public void Notify(string userName, ConcurrentQueue<string> message);
    }
}