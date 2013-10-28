using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Portable.Common.Collections
{
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        bool disablePropertyChanged = false;

        public void AddRange(IEnumerable<T> range)
        {
            var items = range.ToList();

            disablePropertyChanged = true;

            foreach (var o in items)
                this.Add(o);

            disablePropertyChanged = false;

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
        }

        public void ClearAndAddRange(IEnumerable<T> range)
        {
            var newItems = range.ToList();
            var oldItems = this.ToList();

            disablePropertyChanged = true;

            this.Clear();

            foreach (var o in newItems)
                this.Add(o);

            disablePropertyChanged = false;

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItems, oldItems));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (disablePropertyChanged)
                return;

            base.OnCollectionChanged(e);
        }
    }
}
