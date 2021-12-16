using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ModelGenerator
{
    public interface IObservableReadonlyList<T> : IReadOnlyList<T>, INotifyCollectionChanged { }

    internal class ObservableReadonlyListProxy<Tc, Tv> : IObservableReadonlyList<Tv>, INotifyCollectionChanged where Tc : IReadOnlyList<Tv>, INotifyCollectionChanged
    {
        private Tc collection;

        int IReadOnlyCollection<Tv>.Count => collection.Count;

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged { add { collection.CollectionChanged += value; } remove { collection.CollectionChanged -= value; } }

        public ObservableReadonlyListProxy(Tc collection)
        {
            this.collection = collection;
        }

        Tv IReadOnlyList<Tv>.this[int index] => collection[index];

        IEnumerator<Tv> IEnumerable<Tv>.GetEnumerator()
        {
            return ((IEnumerable<Tv>)collection).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)collection).GetEnumerator();
        }
    }

    internal static class ORLPUtility
    {
        public static ObservableReadonlyListProxy<ObservableCollection<T>, T> Get<T>(ObservableCollection<T> collection)
        {
            return new ObservableReadonlyListProxy<ObservableCollection<T>, T>(collection);
        }
    }
}