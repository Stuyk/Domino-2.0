using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominoBlockchain
{
    public class ThreadSafeCollection<T> : IEnumerable<T>
    {
        private ConcurrentDictionary<T, T> _dict = new ConcurrentDictionary<T, T>();

        public void Add(T item)
        {
            _dict.TryAdd(item, item);
        }

        public bool Remove(T item)
        {
            return _dict.TryRemove(item, out _);
        }

        public void Clear()
        {
            _dict = new ConcurrentDictionary<T, T>();
        }

        public T[] ToArray()
        {
            var tmpList = new List<T>(_dict.Count);
            foreach (var item in _dict.Keys) tmpList.Add(item);
            return tmpList.ToArray();
        }

        public T[] ToArrayAndClear()
        {
            var tmpList = new List<T>(_dict.Count);
            foreach (var item in _dict.Keys) tmpList.Add(item);
            _dict.Clear();
            return tmpList.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _dict.Select(k => k.Key).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    //public class ThreadSafeList<T> : List<T>
    //{
    //    private readonly object _lockObject = new object();

    //    public new void AddRange(IEnumerable<T> collection)
    //    {
    //        lock (_lockObject) base.AddRange(collection);
    //    }

    //    public new void Add(T item)
    //    {
    //        lock (_lockObject) base.Add(item);
    //    }

    //    public new void Remove(T item)
    //    {
    //        lock (_lockObject) base.Remove(item);
    //    }

    //    public new int Count
    //    {
    //        get
    //        {
    //            int returnValue;
    //            lock (_lockObject) returnValue = base.Count;
    //            return returnValue;
    //        }
    //    }

    //    public new void Clear()
    //    {
    //        lock (_lockObject) base.Clear();
    //    }
    //}
}