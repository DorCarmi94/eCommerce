using System.Collections;
using System.Collections.Generic;
using eCommerce.DataLayer;

namespace eCommerce.Business
{
    public class LDict<K,V> : IList<Pair<K,V>>
    {
        private IList<Pair<K, V>> _list;

        #region Dictionary Interface

        public LDict()
        {
            _list = new List<Pair<K, V>>();
        }
        
        public LDict(IList<Pair<K, V>> other)
        {
            _list = new List<Pair<K, V>>(other);
        }

        public List<K> Keys()
        {
            lock (_list)
            {
                List<K> keys = new List<K>();
                foreach (var pair in _list)
                {
                    keys.Add(pair.Key);
                }

                return keys;
            }
        }
        
        public List<V> Values()
        {
            lock (_list)
            {
                List<V> vals = new List<V>();
                foreach (var pair in _list)
                {
                    vals.Add(pair.Value);
                }

                return vals;
            }
        }
        
        public bool ContainsKey(K key)
        {
            lock (_list)
            {
                foreach (var pair in _list)
                {
                    if (pair.Key.Equals(key))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public V KeyToValue(K key)
        {
            lock (_list)
            {
                foreach (var pair in _list)
                {
                    if (pair.Key.Equals(key))
                    {
                        return pair.Value;
                    }
                }

                return default;
            }
        }

        public void Add(string HolderID,K pairKey,string KeyID, V pairValue)
        {
            lock (_list)
            {
                _list.Add(new Pair<K, V>() {HolderId = HolderID, Key = pairKey, KeyId = KeyID, Value = pairValue});
            }
        }

        public void Remove(K key)
        {
            lock (_list)
            {
                foreach (var pair in _list)
                {
                    if (pair.Key.Equals(key))
                    {
                        _list.Remove(pair);
                        DataFacade.Instance.RemovePair(pair);
                        return;
                    }
                }

                return;
            }
        }
        
        
        public bool TryAdd(string HolderID,K pairKey,string KeyID, V pairValue)
        {
            lock (_list)
            {
                if (ContainsKey(pairKey))
                {
                    return false;
                }

                _list.Add(new Pair<K, V>() {HolderId = HolderID, Key = pairKey, KeyId = KeyID, Value = pairValue});
                return true;
            }
        }
       

        #endregion
        
        #region IList<> Interface
        public IEnumerator<Pair<K, V>> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _list).GetEnumerator();
        }

        public void Add(Pair<K, V> item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(Pair<K, V> item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(Pair<K, V>[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(Pair<K, V> item)
        {
            return _list.Remove(item);
        }

        public int Count => _list.Count;

        public bool IsReadOnly => _list.IsReadOnly;

        public int IndexOf(Pair<K, V> item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, Pair<K, V> item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public Pair<K, V> this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }
        #endregion
    }
}