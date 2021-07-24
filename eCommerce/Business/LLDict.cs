using System.Collections;
using System.Collections.Generic;
using eCommerce.DataLayer;

namespace eCommerce.Business
{
     public class LLDict<K,V> : IList<ListPair<K,V>>
    {
        private IList<ListPair<K, V>> _list;

        #region Dictionary Interface

        public LLDict()
        {
            _list = new List<ListPair<K, V>>();
        }
        
        public LLDict(IList<ListPair<K, V>> other)
        {
            _list = new List<ListPair<K, V>>(other);
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
        
        public List<IList<V>> Values()
        {
            lock (_list)
            {
                List<IList<V>> vals = new List<IList<V>>();
                foreach (var pair in _list)
                {
                    vals.Add(pair.ValList);
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

        public IList<V> KeyToValue(K key)
        {
            lock (_list)
            {
                foreach (var pair in _list)
                {
                    if (pair.Key.Equals(key))
                    {
                        return pair.ValList;
                    }
                }

                return default;
            }
        }

        public void Add(K pairKey,List<V> pairValue)
        {
            lock (_list)
            {
                _list.Add(new ListPair<K, V>() {Key = pairKey, ValList = pairValue});
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
                        DataFacade.Instance.RemoveListPair(pair);
                        return;
                    }
                }

                return;
            }
        }
        public void RemoveFromList(K key, V value)
        {
            lock (_list)
            {
                foreach (var pair in _list)
                {
                    if (pair.Key.Equals(key))
                    {
                        pair.ValList.Remove(value);
                        if (pair.ValList.Count == 0)
                        {
                            DataFacade.Instance.RemoveListPair(pair);
                        }
                        return;
                    }
                }

                return;
            }
        }
        
        
        public bool TryAdd(K pairKey,IList<V> pairValue)
        {
            lock (_list)
            {
                if (ContainsKey(pairKey))
                {
                    return false;
                }

                _list.Add(new ListPair<K, V>() { Key = pairKey, ValList = pairValue});
                return true;
            }
        }
       

        #endregion
        
        #region IList<> Interface
        public IEnumerator<ListPair<K, V>> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _list).GetEnumerator();
        }

        public void Add(ListPair<K, V> item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(ListPair<K, V> item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(ListPair<K, V>[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(ListPair<K, V> item)
        {
            return _list.Remove(item);
        }

        public int Count => _list.Count;

        public bool IsReadOnly => _list.IsReadOnly;

        public int IndexOf(ListPair<K, V> item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, ListPair<K, V> item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public ListPair<K, V> this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }
        #endregion
    }
}