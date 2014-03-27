using System;
using System.Collections;
using System.Collections.Generic;

namespace Common.Caching
{
    /// <summary>
    /// Implementation of a Least Recently Used Cache
    /// </summary>
    public class LRUCache<TKey, TValue> : IEnumerable<LRUCacheItem<TKey, TValue>>, IEnumerable
    {
        readonly int capacity;
        readonly object sync = new object();

        readonly LinkedList<LRUCacheItem<TKey, TValue>> list;
        readonly Dictionary<TKey, LinkedListNode<LRUCacheItem<TKey, TValue>>> cache;

        public event EventHandler<LRUEvictionEventArgs<TKey, TValue>> ItemEvicted;

        public LRUCache(int capacity)
        {
            if (capacity < 0) throw new ArgumentException("capacity");

            this.capacity = capacity;
            list = new LinkedList<LRUCacheItem<TKey, TValue>>();
            cache = new Dictionary<TKey, LinkedListNode<LRUCacheItem<TKey, TValue>>>(capacity + 1);
        }

        public bool ContainsKey(TKey key)
        {
            return cache.ContainsKey(key);
        }

        public TValue Get(TKey key)
        {
            lock (sync)
            {
                var node = cache[key];

                list.Remove(node);
                list.AddLast(node);

                return node.Value.Value;
            }
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            LRUCacheItem<TKey, TValue> evicted = null;

            lock (sync)
            {
                // if the cache already contains an entry for the key, update it's value and move it to the front of the list
                if (cache.ContainsKey(key))
                {
                    var node = cache[key];

                    list.Remove(node);
                    list.AddLast(node);

                    node.Value.Value = value;
                }
                else
                {
                    var cacheItem = new LRUCacheItem<TKey, TValue>(key, value);
                    var node = new LinkedListNode<LRUCacheItem<TKey, TValue>>(cacheItem);

                    cache.Add(key, node);
                    list.AddLast(node);
                }

                if (cache.Count > capacity)
                    evicted = EvictLRU();
            }

            if (evicted != null)
                OnEvicted(evicted);
        }

        /// <summary>
        /// Evicts the least recently used cache item
        /// </summary>
        /// <returns>Returns the value of the item that was evicted</returns>
        LRUCacheItem<TKey, TValue> EvictLRU()
        {
            var lru = list.First;

            cache.Remove(lru.Value.Key);
            list.Remove(lru);

            return lru.Value;
        }

        void OnEvicted(LRUCacheItem<TKey, TValue> evicted)
        {
            if (ItemEvicted != null)
                ItemEvicted(this, new LRUEvictionEventArgs<TKey,TValue>(evicted.Key, evicted.Value));
        }




        #region IEnumerable  implementation

        public IEnumerator<LRUCacheItem<TKey, TValue>> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion




        #region Partial Collection implementation

        public void Clear()
        {
            lock(sync)
            {
                list.Clear();
                cache.Clear();
            }
        }

        public int Count
        {
            get { return list.Count; }
        }

        public bool Remove(TKey key)
        {
            lock(sync)
            {
                if (cache.ContainsKey(key))
                {
                    var node = cache[key];

                    list.Remove(node);
                    return cache.Remove(key);
                }
                else
                    return false;
            }
        }

        #endregion
    }
}
