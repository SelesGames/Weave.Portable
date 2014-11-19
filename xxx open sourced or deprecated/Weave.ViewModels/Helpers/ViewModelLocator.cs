using System;
using System.Collections.Generic;
using System.Linq;

namespace Weave.ViewModels.Helpers
{
    public class ViewModelLocator
    {
        Dictionary<string, object> store = new Dictionary<string, object>();
        Stack<string> keyStack = new Stack<string>();

        public object Get(string key)
        {
            object result;

            if (store.TryGetValue(key, out result))
                return result;

            return null;
        }

        public void Push(string key, object val)
        {
            if (store.ContainsKey(key))
                throw new ArgumentException("key already present in the ViewModelLocator");

            store.Add(key, val);
            keyStack.Push(key);
        }

        public void Pop()
        {
            if (!keyStack.Any())
                return;

            var key = keyStack.Pop();
            store.Remove(key);
        }
    }
}
