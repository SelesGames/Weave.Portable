using System;

namespace Common.Caching
{
    public class LRUEvictionEventArgs<TKey, TValue> : EventArgs
    {
        internal LRUEvictionEventArgs(TKey k, TValue v)
        {
            Key = k;
            Value = v;
        }

        public TKey Key { get; private set; }
        public TValue Value { get; private set; }
    }
}
