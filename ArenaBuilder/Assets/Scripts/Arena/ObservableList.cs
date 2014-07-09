using System;
using System.Collections.Generic;

namespace Assets.Scripts.Arena
{
    [Serializable]
    public class ObservableList<T> : List<T>
    {
        public new T this[int index]
        {
            get { return base[index]; }
            set
            {
                if (!EqualityComparer<T>.Default.Equals(base[index], value))
                {
                    base[index] = value;
                    Changed(index);
                }
            }
        }

        public event Action<int> Changed = delegate { };
        public event Action Updated = delegate { };

        public new void Add(T item)
        {
            base.Add(item);
            Updated();
        }

        public new void Remove(T item)
        {
            base.Remove(item);
            Updated();
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            Updated();
        }

        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            Updated();
        }

        public new void Clear()
        {
            base.Clear();
            Updated();
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            Updated();
        }

        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            Updated();
        }

        public new void RemoveAll(Predicate<T> match)
        {
            base.RemoveAll(match);
            Updated();
        }
    }
}