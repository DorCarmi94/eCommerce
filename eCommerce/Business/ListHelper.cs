using System.Collections.Generic;
using eCommerce.DataLayer;

namespace eCommerce.Business.Repositories
{
    public class ListHelper<K,V>
    {
        public static List<K> Keys(IList<Pair<K,V>> lst)
        {
            lock (lst)
            {
                List<K> keys = new List<K>();
                foreach (var pair in lst)
                {
                    keys.Add(pair.Key);
                }

                return keys;
            }
        }
        
        public static List<V> Values(IList<Pair<K,V>> lst)
        {
            lock (lst)
            {
                List<V> vals = new List<V>();
                foreach (var pair in lst)
                {
                    vals.Add(pair.Value);
                }

                return vals;
            }
        }
        
        public static bool ContainsKey(IList<Pair<K,V>> lst, K key)
        {
            lock (lst)
            {
                foreach (var pair in lst)
                {
                    if (pair.Key.Equals(key))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static V KeyToValue(IList<Pair<K, V>> lst, K key)
        {
            lock (lst)
            {
                foreach (var pair in lst)
                {
                    if (pair.Key.Equals(key))
                    {
                        return pair.Value;
                    }
                }

                return default;
            }
        }

        public static void Add(IList<Pair<K, V>> lst, string HolderID,K pairKey,string KeyID, V pairValue)
        {
            lock (lst)
            {
                lst.Add(new Pair<K, V>() {HolderId = HolderID, Key = pairKey, KeyId = KeyID, Value = pairValue});
            }
        }

        public static void Remove(IList<Pair<K, V>> lst, K key)
        {
            lock (lst)
            {
                foreach (var pair in lst)
                {
                    if (pair.Key.Equals(key))
                    {
                        lst.Remove(pair);
                        return;
                    }
                }

                return;
            }
        }
        
        
        public static bool TryAdd(IList<Pair<K, V>> lst, string HolderID,K pairKey,string KeyID, V pairValue)
        {
            lock (lst)
            {
                if (ContainsKey(lst, pairKey))
                {
                    return false;
                }

                lst.Add(new Pair<K, V>() {HolderId = HolderID, Key = pairKey, KeyId = KeyID, Value = pairValue});
                return true;
            }
        }
        
    }
}