using System.Collections;
using System.Collections.Generic;

namespace _Framework.Scripts.Util
{
    public class DefaultDictionary<TKey, TValue> : IDictionary<TKey,TValue>
    {
        private readonly Dictionary<TKey,TValue> dicionary;

        public DefaultDictionary()
        {
            this.dicionary = new Dictionary<TKey, TValue>();
        }

        public DefaultDictionary(Dictionary<TKey, TValue> dicionary)
        {
            this.dicionary = dicionary;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dicionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            dicionary.Clear();
        }

        public int Count => dicionary.Count;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return (dicionary as ICollection<KeyValuePair<TKey, TValue>>).IsReadOnly; }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            (dicionary as ICollection<KeyValuePair<TKey, TValue>>).Add(keyValuePair);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            return (dicionary as ICollection<KeyValuePair<TKey, TValue>>).Contains(keyValuePair);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            return (dicionary as ICollection<KeyValuePair<TKey, TValue>>).Remove(keyValuePair);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            (dicionary as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        }

        public void Add(TKey key, TValue value)
        {
            dicionary.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return dicionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return dicionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get { return ContainsKey(key) ? dicionary[key] : default(TValue); }
            set { dicionary[key] = value; }
        }

        public ICollection<TKey> Keys => dicionary.Keys;
        public ICollection<TValue> Values => dicionary.Values;
    }
}