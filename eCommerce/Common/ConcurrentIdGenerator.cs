using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace eCommerce.Common
{
    public class ConcurrentIdGenerator
    {
        private Mutex _mutex;
        private long _id;

        public ConcurrentIdGenerator(long startFromId)
        {
            _mutex = new Mutex();
            _id = startFromId;
        }
        
        public long MoveNext()
        {
            long prevValue = -1;
            _mutex.WaitOne();
            prevValue = _id;
            _id++;
            _mutex.ReleaseMutex();
            return prevValue;
        }
    }
}